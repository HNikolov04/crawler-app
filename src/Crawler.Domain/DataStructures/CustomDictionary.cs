using System;
using System.Collections;
using System.Collections.Generic;
using Crawler.Domain.Entities;

namespace Crawler.Domain.DataStructures
{
    public class CustomDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private const int InitialSize = 16;
        private CustomLinkedList<CustomKeyValuePair<TKey, TValue>>[] _buckets;

        public int Count
        {
            get
            {
                int count = 0;
                foreach (var bucket in _buckets)
                {
                    if (bucket != null)
                    {
                        count += bucket.Count;
                    }
                }
                return count;
            }
        }

        // ReSharper disable once ConvertConstructorToMemberInitializers
        public CustomDictionary()
        {
            _buckets = new CustomLinkedList<CustomKeyValuePair<TKey, TValue>>[InitialSize];
        }

        private int GetBucketIndex(TKey key)
        {
            int hash = key.GetHashCode();
            return Math.Abs(hash % _buckets.Length);
        }

        public void Add(TKey key, TValue value)
        {
            int index = GetBucketIndex(key);
            if (_buckets[index] == null)
            {
                _buckets[index] = new CustomLinkedList<CustomKeyValuePair<TKey, TValue>>();
            }

            foreach (var kvp in _buckets[index])
            {
                if (EqualityComparer<TKey>.Default.Equals(kvp.Key, key))
                {
                    throw new ArgumentException("Key already exists");
                }
            }

            _buckets[index].AddLast(new CustomKeyValuePair<TKey, TValue>(key, value));
        }

        public TValue this[TKey key]
        {
            get
            {
                int index = GetBucketIndex(key);
                if (_buckets[index] != null)
                {
                    foreach (var kvp in _buckets[index])
                    {
                        if (EqualityComparer<TKey>.Default.Equals(kvp.Key, key))
                        {
                            return kvp.Value;
                        }
                    }
                }
                throw new KeyNotFoundException($"Key '{key}' not found.");
            }
            set
            {
                int index = GetBucketIndex(key);
                if (_buckets[index] == null)
                {
                    _buckets[index] = new CustomLinkedList<CustomKeyValuePair<TKey, TValue>>();
                }

                CustomLinkedListNode<CustomKeyValuePair<TKey, TValue>> current = _buckets[index].First;
                while (current != null)
                {
                    if (EqualityComparer<TKey>.Default.Equals(current.Value.Key, key))
                    {
                        current.Value = new CustomKeyValuePair<TKey, TValue>(key, value);
                        return;
                    }
                    current = current.Next;
                }

                _buckets[index].AddLast(new CustomKeyValuePair<TKey, TValue>(key, value));
            }
        }

        public bool ContainsKey(TKey key)
        {
            int index = GetBucketIndex(key);
            if (_buckets[index] != null)
            {
                foreach (var kvp in _buckets[index])
                {
                    if (EqualityComparer<TKey>.Default.Equals(kvp.Key, key))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var bucket in _buckets)
            {
                if (bucket != null)
                {
                    foreach (var kvp in bucket)
                    {
                        yield return new KeyValuePair<TKey, TValue>(kvp.Key, kvp.Value);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
