using MyNet.Channel;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Concurrent;
namespace MyNet
{
    public class ChannelGroup : ICollection<ChannelBase>
    {
        private object _datalock = new object();
        private readonly ConcurrentDictionary<ChannelID, ChannelBase> _serverChannels = new ConcurrentDictionary<ChannelID, ChannelBase>();
        private readonly ConcurrentDictionary<ChannelID, ChannelBase> _noserverChannels = new ConcurrentDictionary<ChannelID, ChannelBase>();
        public int Count
        {
            get
            {
                return _serverChannels.Count + _noserverChannels.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(ChannelBase item)
        {
            if (item is ServerChannel)
            {
                _serverChannels.TryAdd(item.Id, item);
            }
            else
            {
                _noserverChannels.TryAdd(item.Id, item);
            }
        }

        public void Clear()
        {
            _serverChannels.Clear();
            _noserverChannels.Clear();
        }

        public bool Contains(ChannelBase item)
        {
            if (item is ServerChannel)
            {
                return _serverChannels.ContainsKey(item.Id);
            }
            else
            {
                return _noserverChannels.ContainsKey(item.Id);
            }
        }
        public ChannelBase[] ToArray()
        {
            List<ChannelBase> channels = new List<ChannelBase>(this.Count);
            channels.AddRange(this._serverChannels.Values);
            channels.AddRange(this._noserverChannels.Values);
            return channels.ToArray();
        }
        public ChannelBase[] ServerArray()
        {
            KeyValuePair<ChannelID, ChannelBase>[] arr = _serverChannels.ToArray();
            ChannelBase[] rt = new ChannelBase[arr.Length];
            for (int i = 0; i < rt.Length; i++)
            {
                rt[i] = arr[i].Value;
            }
            return rt;
        }
        public ChannelBase[] ClientArray()
        {
            KeyValuePair<ChannelID, ChannelBase>[] arr = _noserverChannels.ToArray();
            ChannelBase[] rt = new ChannelBase[arr.Length];
            for (int i = 0; i < rt.Length; i++)
            {
                rt[i] = arr[i].Value;
            }
            return rt;
        }
        public void CopyTo(ChannelBase[] array, int arrayIndex)
        {
            this.ToArray().CopyTo(array, arrayIndex);
        }

        public IEnumerator<ChannelBase> GetEnumerator()
        {
            foreach (KeyValuePair<ChannelID, ChannelBase> kv in _serverChannels)
            {
                yield return kv.Value;
            }
            foreach (KeyValuePair<ChannelID, ChannelBase> kv in _noserverChannels)
            {
                yield return kv.Value;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (KeyValuePair<ChannelID, ChannelBase> kv in _serverChannels)
            {
                yield return kv.Value;
            }
            foreach (KeyValuePair<ChannelID, ChannelBase> kv in _noserverChannels)
            {
                yield return kv.Value;
            }
        }
        public bool Remove(ChannelBase item)
        {
            ChannelBase channel;
            if (item is ServerChannel)
            {
                return _serverChannels.TryRemove(item.Id, out channel);
            }
            else
            {
                return _noserverChannels.TryRemove(item.Id, out channel);
            }
        }
    }
}
