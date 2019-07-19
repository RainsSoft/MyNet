using System;
using System.Collections.Generic;
namespace MyNet.Buffer
{
    public class MinChunk : AbstractChunk
    {
        private int mCurOffset;
        private byte[] mBufferBlock;
        private int mBuffSize;
        private int mBuffNum;
        private int mTotalCapacity;
        private Stack<int> mFreeOffsetPool;
        public MinChunk(int buffcount, int buffsize)
        {
            mCurOffset = 0;
            mBuffSize = buffsize;
            mTotalCapacity = buffcount * buffsize;
            mBufferBlock = new byte[mTotalCapacity];
            mFreeOffsetPool = new Stack<int>(buffcount);
            mBuffNum = buffcount;
        }


        public override int BufferSize
        {
            get { return mBuffSize; }
        }

        public override byte[] BufferBlock
        {
            get { return mBufferBlock; }
        }
        public override int AllocBuffer()
        {
            lock (mFreeOffsetPool)
            {
                if (mFreeOffsetPool.Count > 0)
                {
                    return mFreeOffsetPool.Pop();
                }
                else
                {
                    if (mTotalCapacity <= this.mCurOffset)
                    {
                        return -1;
                    }
                    int offset = this.mCurOffset;
                    this.mCurOffset += this.mBuffSize;
                    return offset;
                }
            }
        }

        public override void FreeBuffer(int offset)
        {
            lock (mFreeOffsetPool)
            {
                mFreeOffsetPool.Push(offset);
                if (mFreeOffsetPool.Count == mBuffNum && this.Next == null)
                {
                    mBufferBlock = null;
                    mFreeOffsetPool = null;
                    this.Pre.Next = null;
                }
            }
       
        }
    }
}
