using System;
using System.Collections.Generic;

namespace FrontierDispatch.Core
{
    // Assignment Phase 3. Implement as a binary min-heap.

    public sealed class MinPriorityQueue<T>
    {
        private readonly List<Entry> _heap = new List<Entry>();

        public int Count => _heap.Count;

        public void Enqueue(T item, int priority)
        {
            _heap.Add(new Entry(item, priority));
            SiftUp(_heap.Count - 1);
        }

        public T Peek()
        {
            if (_heap.Count == 0)
            {
                throw new InvalidOperationException("Cannot peek an empty priority queue.");
            }

            return _heap[0].Item;
        }

        public T Dequeue()
        {
            if (_heap.Count == 0)
            {
                throw new InvalidOperationException("Cannot dequeue from an empty priority queue.");
            }

            T minItem = _heap[0].Item;
            int lastIndex = _heap.Count - 1;

            // Move the last item to the root and remove the old tail
            _heap[0] = _heap[lastIndex];
            _heap.RemoveAt(lastIndex);

            if (_heap.Count > 0)
            {
                SiftDown(0);
            }

            return minItem;
        }

        private void SiftUp(int index)
        {
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;

                if (_heap[index].Priority >= _heap[parentIndex].Priority)
                {
                    break;
                }

                Swap(index, parentIndex);
                index = parentIndex;
            }
        }

        private void SiftDown(int index)
        {
            int count = _heap.Count;

            while (true)
            {
                int leftChild = 2 * index + 1;
                int rightChild = 2 * index + 2;
                int smallest = index;

                if (leftChild < count && _heap[leftChild].Priority < _heap[smallest].Priority)
                {
                    smallest = leftChild;
                }

                if (rightChild < count && _heap[rightChild].Priority < _heap[smallest].Priority)
                {
                    smallest = rightChild;
                }

                if (smallest == index)
                {
                    break;
                }

                Swap(index, smallest);
                index = smallest;
            }
        }

        private void Swap(int i, int j)
        {
            Entry temp = _heap[i];
            _heap[i] = _heap[j];
            _heap[j] = temp;
        }

        private readonly struct Entry
        {
            public Entry(T item, int priority)
            {
                Item = item;
                Priority = priority;
            }

            public T Item { get; }
            public int Priority { get; }
        }
    }
}

