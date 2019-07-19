using MyNet.Handlers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MyNet.Common;
using MyNet.Channel;
using MyNet.Buffer;
using System.Net;
using System.IO;

namespace MyNet.Middleware.Http
{
    // <summary>
    // 请求解析器
    // </summary>
    internal static class HttpRequestParser
    {
        private static readonly string Cookie_Split = "; ";
        private static readonly char Cookie_EQ = '=';
        /// <summary>
        /// 空格
        /// </summary>
        private static readonly byte Space = 32;

        /// <summary>
        /// 换行
        /// </summary>
        private static readonly byte[] CRLF = Encoding.ASCII.GetBytes("\r\n");

        /// <summary>
        /// 获取双换行
        /// </summary>
        private static readonly byte[] DoubleCRLF = Encoding.ASCII.GetBytes("\r\n\r\n");

        /// <summary>
        /// 请求头键值分隔
        /// </summary>
        private static readonly byte[] KvSpliter = Encoding.ASCII.GetBytes(": ");

        /// <summary>
        /// http1.1
        /// </summary>
        private static readonly byte[] HttpVersion11 = Encoding.ASCII.GetBytes("HTTP/1.1");

        /// <summary>
        /// 支持的http方法
        /// </summary>
        private static readonly HashSet<string> MethodNames = new HashSet<string>(Enum.GetNames(typeof(HttpMethod)), StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 支持的http方法最大长度
        /// </summary>
        private static readonly int MedthodMaxLength = MethodNames.Max(m => m.Length);



        /// <summary>
        /// 解析连接请求信息        
        /// </summary>
        /// <param name="context">上下文</param>   
        /// <returns></returns>
        public static HttpRequestParseResult Parse(IContext context, IByteStream stream, bool isSSL)
        {
            HttpRequestParseResult result = ParseInternal(context, stream, isSSL);
            stream.SetReaderIndex(0);
            return result;
        }

        /// <summary>
        /// 解析连接请求信息        
        /// </summary>
        /// <param name="context">上下文</param>   
        /// <returns></returns>
        private static HttpRequestParseResult ParseInternal(IContext context, IByteStream stream, bool isSSL)
        {
            HttpRequest request;
            int headerLength;
            int contentLength;
            HttpRequestParseResult result = new HttpRequestParseResult
            {
                IsHttp = HttpRequestParser.TryGetRequest(context, stream, isSSL, out request, out headerLength, out contentLength)
            };

            result.ContentLength = contentLength;
            result.HeaderLength = headerLength;

            if (result.IsHttp == false)
            {
                return result;
            }

            if (request == null) // 数据未完整     
            {
                return result;
            }


            switch (request.HttpMethod)
            {
                case HttpMethod.GET:
                    request.Body = new byte[0];
                    request.InputStream = new MemoryStream(request.Body);
                    request.Form = new HttpNameValueCollection();
                    request.Files = new HttpFile[0];
                    break;

                default:
                    stream.SetReaderIndex(headerLength);
                    request.Body = stream.ReadBytes(contentLength);
                    request.InputStream = new MemoryStream(request.Body);
                    stream.SetReaderIndex(headerLength);
                    HttpRequestParser.GeneratePostFormAndFiles(request, context, stream);
                    break;
            }

            result.Request = request;
            result.PackageLength = headerLength + contentLength;
            return result;
        }


        /// <summary>
        /// 尝试当作http头解析，生成请求对象
        /// 如果不是http头则返回false
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="request">请求对象</param>
        /// <param name="headerLength">请求头长度</param>
        /// <param name="contentLength">请求内容长度</param>       
        /// <returns></returns>
        private static bool TryGetRequest(IContext context, IByteStream stream, bool isSSL, out HttpRequest request, out int headerLength, out int contentLength)
        {
            request = null;
            headerLength = 0;
            contentLength = 0;
            IByteStream reader = stream;
            ChannelBase channel = context.Channel;
            if (channel == null)
            {
                return false;
            }
            // HTTP Method
            reader.SetReaderIndex(0);
            int methodLength = reader.BytesBefore(Space);
            if (methodLength < 0 || methodLength > MedthodMaxLength)
            {
                return false;
            }
            string methodName = reader.ReadString(Encoding.ASCII, methodLength);
            if (MethodNames.Contains(methodName) == false)
            {
                return false;
            }
            HttpMethod httpMethod = (HttpMethod)Enum.Parse(typeof(HttpMethod), methodName, true);

            // HTTP Path
            reader.ReadSkip(1);
            int pathLength = reader.BytesBefore(Space);
            if (pathLength < 0)
            {
                return false;
            }
            string path = reader.ReadString(Encoding.ASCII, pathLength);


            // HTTP Version
            reader.ReadSkip(1);
            if (reader.StartWith(HttpVersion11) == false)
            {
                return false;
            }
            reader.ReadSkip(HttpVersion11.Length);
            if (reader.StartWith(CRLF) == false)
            {
                return false;
            }

            // HTTP Second line
            reader.ReadSkip(CRLF.Length);
            int endIndex = reader.BytesBefore(DoubleCRLF);
            if (endIndex < 0)
            {
                return false;
            }

            HttpNameValueCollection httpHeader = new HttpNameValueCollection();
            headerLength = reader.ReaderIndex + endIndex + DoubleCRLF.Length;


            while (reader.ReaderIndex < headerLength - CRLF.Length)
            {
                int keyLength = reader.BytesBefore(KvSpliter);
                if (keyLength <= 0)
                {
                    break;
                }
                string key = reader.ReadString(Encoding.ASCII, keyLength);

                reader.ReadSkip(KvSpliter.Length);
                int valueLength = reader.BytesBefore(CRLF);
                if (valueLength < 0)
                {
                    break;
                }
                string value = reader.ReadString(Encoding.ASCII, valueLength);

                if (reader.StartWith(CRLF) == false)
                {
                    break;
                }
                reader.ReadSkip(CRLF.Length);
                httpHeader.Add(key, value);
            }

            if (httpMethod != HttpMethod.GET)
            {
                contentLength = httpHeader.TryGet<int>("Content-Length");
                if (reader.Length - headerLength < contentLength)
                {
                    return true;// 数据未完整  
                }
            }


            request = new HttpRequest
            {
                LocalEndPoint = (IPEndPoint)channel.LocalEndPoint,
                RemoteEndPoint = (IPEndPoint)channel.RemoteEndPoint,
                HttpMethod = httpMethod,
                Headers = httpHeader
            };

            //获取请求cookie
            string reqcookies = httpHeader.TryGet<string>("Cookie");
            if (reqcookies != null)
            {
                GenerateCookies(request, reqcookies);
            }

            string scheme = isSSL ? "https" : "http";
            string host = httpHeader["Host"];
            if (string.IsNullOrEmpty(host) == true)
            {
                host = channel.LocalEndPoint.ToString();
            }
            request.Host = host;
            string referer = httpHeader.TryGet<string>("Referer");
            if (!string.IsNullOrEmpty(referer))
            {
                try
                {
                    request.RefererUrl = new Uri(referer);
                }
                catch { }
            }
            request.UserAgent = httpHeader.TryGet<string>("User-Agent", "");
            request.Url = new Uri(string.Format("{0}://{1}{2}", scheme, host, path));
            request.Path = request.Url.AbsolutePath;
            request.Query = HttpNameValueCollection.Parse(request.Url.Query.TrimStart('?'));
            return true;
        }

        private static void GenerateCookies(HttpRequest request, string cookies)
        {
            string[] citems = cookies.Split(new string[] { Cookie_Split }, StringSplitOptions.None);
            foreach (string s in citems)
            {
                int se = s.IndexOf(Cookie_EQ);
                if (se < 0)
                    continue;
                HttpCookie cookie = new HttpCookie(s.Substring(0, se), s.Substring(se, s.Length - se));
                request.Cookies.SetCookie(cookie);
            }
        }
        /// <summary>
        /// 生成Post得到的表单和文件
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="streamReader">数据读取器</param>      
        private static void GeneratePostFormAndFiles(HttpRequest request, IContext context, IByteStream stream)
        {
            string boundary = "";
            if (request.IsApplicationFormRequest() == true)
            {
                HttpRequestParser.GenerateApplicationForm(request);
            }
            else if (request.IsMultipartFormRequest(out boundary) == true)
            {
                if (request.Body.Length >= boundary.Length)
                {
                    HttpRequestParser.GenerateMultipartFormAndFiles(request, stream, context, boundary);
                }
            }


            if (request.Form == null)
            {
                request.Form = new HttpNameValueCollection();
            }

            if (request.Files == null)
            {
                request.Files = new HttpFile[0];
            }
        }

        /// <summary>
        /// 生成一般表单的Form
        /// </summary>
        /// <param name="request">请求对象</param>
        private static void GenerateApplicationForm(HttpRequest request)
        {
            string body = Encoding.UTF8.GetString(request.Body);
            request.Form = HttpNameValueCollection.Parse(body);
            request.Files = new HttpFile[0];
        }

        /// <summary>
        /// 生成表单和文件
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="streamReader">数据读取器</param>   
        /// <param name="boundary">边界</param>
        private static void GenerateMultipartFormAndFiles(HttpRequest request, IByteStream stream, IContext context, string boundary)
        {
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary);
            int maxPosition = stream.Length - Encoding.ASCII.GetBytes("--\r\n").Length;
            IByteStream reader = stream;
            List<HttpFile> files = new List<HttpFile>();
            HttpNameValueCollection form = new HttpNameValueCollection();

            reader.ReadSkip(boundaryBytes.Length);
            while (reader.ReaderIndex < maxPosition)
            {
                int headLength = reader.BytesBefore(DoubleCRLF) + DoubleCRLF.Length;
                if (headLength < DoubleCRLF.Length)
                {
                    break;
                }

                string head = reader.ReadString(Encoding.UTF8, headLength);
                int bodyLength = reader.BytesBefore(boundaryBytes);
                if (bodyLength < 0)
                {
                    break;
                }
                string fileName;
                MultipartHead mHead = new MultipartHead(head);
                if (mHead.TryGetFileName(out fileName) == true)
                {
                    byte[] bytes = reader.ReadBytes(bodyLength);
                    HttpFile file = new HttpFile(mHead.Name, fileName, bytes);
                    files.Add(file);
                }
                else
                {
                    byte[] byes = reader.ReadBytes(bodyLength);
                    string value = byes.Length == 0 ? null : Utility.UrlDecode(byes, Encoding.UTF8);
                    form.Add(mHead.Name, value);
                }
                reader.ReadSkip(boundaryBytes.Length);
            }

            request.Form = form;
            request.Files = files.ToArray();
        }
    }
}
