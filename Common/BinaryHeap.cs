using System;
using System.Collections;
using System.Collections.Generic;


namespace MyNet.Common
{
    public class BinaryHeap<T> : IEnumerable<T>
        where T : class
    {
        private readonly IComparer<T> comparer;
        private int count;
        private int capacity;
        private T[] items;

        public BinaryHeap():this(11)
        {
        }
        public BinaryHeap(int initcapac)
        {
            this.comparer = Comparer<T>.Default;
            this.capacity = initcapac;
            this.items = new T[this.capacity];
        }

        public int Count
        {
            get { return this.count; }
        }

        public T Dequeue()
        {
            T result = Peek();
            if (result == null)
            {
                return null;
            }

            int newCount = --this.count;
            T lastItem = this.items[newCount];
            this.items[newCount] = null;
            if (newCount > 0)
            {
                this.TrickleDown(0, lastItem);
            }
            return result;
        }

        public T Peek()
        {
            return this.count == 0 ? null : this.items[0];
        }

        public void Enqueue(T item)
        {
            int oldCount = this.count;
            if (oldCount == this.capacity)
            {
                this.GrowHeap();
            }
            this.count = oldCount + 1;
            this.BubbleUp(oldCount, item);
        }

        public void Remove(T item)
        {
            int index = Array.IndexOf(this.items, item);
            if (index == -1)
            {
                return;
            }

            this.count--;
            if (index == this.count)
            {
                this.items[index] = default(T);
            }
            else
            {
                T last = this.items[this.count];
                this.items[this.count] = default(T);
                this.TrickleDown(index, last);
                if (this.items[index] == last)
                {
                    this.BubbleUp(index, last);
                }
            }
        }

        void BubbleUp(int index, T item)
        {
            while (index > 0)
            {
                int parentIndex = (index - 1) >> 1;
                T parentItem = this.items[parentIndex];
                if (this.comparer.Compare(item, parentItem) >= 0)
                {
                    break;
                }
                this.items[index] = parentItem;
                index = parentIndex;
            }
            this.items[index] = item;
        }

        void GrowHeap()
        {
            int oldCapacity = this.capacity;
            this.capacity = oldCapacity + (oldCapacity <= 64 ? oldCapacity + 2 : (oldCapacity >> 1));
            T[] newHeap = new T[this.capacity];
            Array.Copy(this.items, 0, newHeap, 0, this.count);
            this.items = newHeap;
        }

        void TrickleDown(int index, T item)
        {
            int middleIndex = this.count >> 1;
            while (index < middleIndex)
            {
                int childIndex = (index << 1) + 1;
                T childItem = this.items[childIndex];
                int rightChildIndex = childIndex + 1;
                if (rightChildIndex < this.count
                    && this.comparer.Compare(childItem, this.items[rightChildIndex]) > 0)
                {
                    childIndex = rightChildIndex;
                    childItem = this.items[rightChildIndex];
                }
                if (this.comparer.Compare(item, childItem) <= 0)
                {
                    break;
                }
                this.items[index] = childItem;
                index = childIndex;
            }
            this.items[index] = item;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < this.count; i++)
            {
                yield return this.items[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Clear()
        {
            this.count = 0;
            Array.Clear(this.items, 0, 0);
        }
    }
}