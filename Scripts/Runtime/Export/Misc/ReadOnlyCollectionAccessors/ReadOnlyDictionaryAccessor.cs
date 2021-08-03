using System.Collections;
using System.Collections.Generic;

namespace UnityEngine
{
    public readonly struct ReadOnlyDictionaryAccessor<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        // private constructor, because there's already an implicit cast operator
        private ReadOnlyDictionaryAccessor(Dictionary<TKey, TValue> collection)
        {
            m_Collection = collection;
        }

        // the actual collection
        private readonly Dictionary<TKey, TValue> m_Collection;

        /// <summary>
        /// The number of key-value pairs in this collection.
        /// </summary>
        public int count => m_Collection.Count;

        /// <summary>
        /// Check whether the dictionary contains a key.
        /// </summary>
        public bool ContainsKey(TKey key)
        {
            return m_Collection.ContainsKey(key);
        }

        /// <summary>
        /// Access a value in this dictionary.
        /// </summary>
        public TValue this[TKey key] => m_Collection[key];

        /// <summary>
        /// Attempt to get a value from this dictionary.
        /// </summary>
        /// <returns>Whether the value  was found.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return m_Collection.TryGetValue(key, out value);
        }

        /// <summary>
        /// The collection of keys within this dictionary.
        /// </summary>
        public Dictionary<TKey, TValue>.KeyCollection keys => m_Collection.Keys;

        /// <summary>
        /// The collection of values within this dictionary.
        /// </summary>
        public Dictionary<TKey, TValue>.ValueCollection values => m_Collection.Values;

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            return m_Collection.GetEnumerator();
        }

        // IEnumerable<T> explicit implementation to prioritize the custom enumerator in a foreach loop.
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<TKey, TValue>>) m_Collection).GetEnumerator();
        }

        // IEnumerable explicit implementation to prioritize the custom enumerator in a foreach loop.
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) m_Collection).GetEnumerator();
        }

        /// <summary>
        /// Get the read-only interface for a dictionary.
        /// </summary>
        public static implicit operator ReadOnlyDictionaryAccessor<TKey, TValue>(Dictionary<TKey, TValue> i)
        {
            return new ReadOnlyDictionaryAccessor<TKey, TValue>(i);
        }
    }
}