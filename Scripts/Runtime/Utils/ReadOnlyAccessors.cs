using System.Collections;
using System.Collections.Generic;

namespace HiraBots
{
    internal readonly struct ReadOnlyArrayAccessor<T> : IEnumerable<T>
    {
        private ReadOnlyArrayAccessor(T[] collection) => _collection = collection;
        private readonly T[] _collection;
        internal int Count => _collection.Length;
        internal T this[int index] => _collection[index];
        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>) _collection).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _collection).GetEnumerator();
        public static implicit operator ReadOnlyArrayAccessor<T>(T[] i) => new ReadOnlyArrayAccessor<T>(i);
    }

    internal readonly struct ReadOnlyListAccessor<T> : IEnumerable<T>
    {
        private ReadOnlyListAccessor(List<T> collection) => _collection = collection;
        private readonly List<T> _collection;
        internal int Count => _collection.Count;
        internal T this[int index] => _collection[index];
        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>) _collection).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _collection).GetEnumerator();
        public static implicit operator ReadOnlyListAccessor<T>(List<T> i) => new ReadOnlyListAccessor<T>(i);
    }

    internal readonly struct ReadOnlyDictionaryAccessor<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private ReadOnlyDictionaryAccessor(Dictionary<TKey, TValue> collection) => _collection = collection;
        private readonly Dictionary<TKey, TValue> _collection;
        internal int Count => _collection.Count;
        internal bool ContainsKey(TKey key) => _collection.ContainsKey(key);
        internal TValue this[TKey key] => _collection[key];
        internal bool TryGetValue(TKey key, out TValue value) => _collection.TryGetValue(key, out value);
        internal IEnumerable<TKey> Keys => _collection.Keys;
        internal IEnumerable<TValue> Values => _collection.Values;
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => ((IEnumerable<KeyValuePair<TKey, TValue>>) _collection).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _collection).GetEnumerator();
        public static implicit operator ReadOnlyDictionaryAccessor<TKey, TValue>(Dictionary<TKey, TValue> i) => new ReadOnlyDictionaryAccessor<TKey, TValue>(i);
    }
}