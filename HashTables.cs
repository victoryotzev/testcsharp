using System;
using System.Collections;

namespace HashTables // Note: actual namespace depends on the project name.
{
    public class HashTables<TKey, TValue> : IEnumerable<KeyValue<TKey, TValue>>
    {

        public HashTables(int capacity = DefaultCapacity)
        {
            this.slots = new LinkedList<KeyValue<TKey, TValue>>[capacity];
            this.Count = 0;
        }

        private LinkedList<KeyValue<TKey, TValue>>[] slots;

        public int Count { get; private set; }

        public int Capacity => this.slots.Length;

        private const int DefaultCapacity = 16;

        public void Add(TKey key, TValue value)
        {
            this.GrowIfNeeded();

            int slotIndex = this.GetSlotIndex(key);

            if (this.slots[slotIndex] == null)
            {
                this.slots[slotIndex] = new LinkedList<KeyValue<TKey, TValue>>();
            }

            foreach (KeyValue<TKey, TValue> item in this.slots[slotIndex])
            {
                if (item.Key.Equals(key))
                {
                    throw new ArgumentException();
                }
            }

            KeyValue<TKey, TValue> newElement = new KeyValue<TKey, TValue>(key, value);
            this.slots[slotIndex].AddLast(newElement);
            this.Count++;
        }

        public bool AddOrReplace(TKey key, TValue value)
        {
            this.GrowIfNeeded();

            int slotIndex = this.GetSlotIndex(key);

            if (this.slots[slotIndex] == null)
            {
                this.slots[slotIndex] = new LinkedList<KeyValue<TKey, TValue>>();
            }

            foreach (KeyValue<TKey, TValue> item in this.slots[slotIndex])
            {
                if (item.Key.Equals(key))
                {
                    item.Value = value;
                    return false;
                }
            }

            KeyValue<TKey, TValue> newElement = new KeyValue<TKey, TValue>(key, value);
            this.slots[slotIndex].AddLast(newElement);
            this.Count++;

            return true;
        }

        public KeyValue<TKey, TValue> Find(TKey key)
        {
            int slotIndex = this.GetSlotIndex(key);

            LinkedList<KeyValue<TKey, TValue>> slot = this.slots[slotIndex];

            if (slot != null)
            {
                foreach (KeyValue<TKey, TValue> item in slot)
                {
                    if (item.Key.Equals(key))
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        public TValue Get(TKey key)
        {
            KeyValue<TKey, TValue> element = this.Find(key);

            if (element == null)
            {
                throw new KeyNotFoundException();
            }

            return element.Value;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            KeyValue<TKey, TValue> element = this.Find(key);

            if (element != null)
            {
                value = element.Value;
                return true;
            }

            value = default(TValue);
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return this.Find(key) != null;
        }

        public bool Remove(TKey key)
        {
            int slotIndex = this.GetSlotIndex(key);

            LinkedList<KeyValue<TKey, TValue>> slot = this.slots[slotIndex];

            if (slot != null)
            {
                LinkedListNode<KeyValue<TKey, TValue>> currentNode = slot.First;

                while (currentNode != null)
                {
                    if (currentNode.Value.Key.Equals(key))
                    {
                        slot.Remove(currentNode);
                        this.Count--;

                        return true;
                    }

                    currentNode = currentNode.Next;
                }
            }

            return false;
        }

        public void Clear()
        {
            this.slots = new LinkedList<KeyValue<TKey, TValue>>[DefaultCapacity];
            this.Count = 0;
        }

        public TValue this[TKey key]
        {
            get => Get(key);
            set => AddOrReplace(key, value);
        }

        public IEnumerable<TKey> Keys => this.Select(e => e.Key);


        public IEnumerable<TValue> Values => this.Select(e => e.Value);


        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();


        public IEnumerator<KeyValue<TKey, TValue>> GetEnumerator()
        {
            foreach (LinkedList<KeyValue<TKey, TValue>> slot in this.slots)
            {
                if (slot != null)
                {
                    foreach (KeyValue<TKey, TValue> item in slot)
                    {
                        yield return item;
                    }
                }
            }
        }

        private int GetSlotIndex(TKey key)
        {
            return Math.Abs(key.GetHashCode()) % this.slots.Length;
        }

        private const float LoadFactor = 0.75f;

        private void GrowIfNeeded()
        {
            if ((float)(this.Count + 1) / this.Capacity > LoadFactor)
            {
                this.Grow();
            }
        }

        private void Grow()
        {
            HashTables<TKey, TValue> newValues = new HashTables<TKey, TValue>(Capacity * 2);

            foreach (var item in this)
            {
                newValues.Add(item.Key, item.Value);
            }

            this.slots = newValues.slots;
            this.Count = newValues.Count;
        }
    }
}