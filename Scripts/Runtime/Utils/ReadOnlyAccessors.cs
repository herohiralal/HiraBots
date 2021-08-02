using System.Collections;
using System.Collections.Generic;

namespace HiraBots
{
    public readonly struct ReadOnlyArrayAccessor<T> : IEnumerable<T>
    {
        private ReadOnlyArrayAccessor(T[] collection) => _collection = collection;
        private readonly T[] _collection;
        public int Count => _collection.Length;
        public T this[int index] => _collection[index];
        public Enumerator GetEnumerator() => new Enumerator(_collection);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>) _collection).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _collection).GetEnumerator();
        public static implicit operator ReadOnlyArrayAccessor<T>(T[] i) => new ReadOnlyArrayAccessor<T>(i);

        public struct Enumerator : IEnumerator<T>
        {
            public Enumerator(T[] collection)
            {
                _collection = collection;
                _currentIndex = -1;
            }

            private readonly T[] _collection;
            private int _currentIndex;

            public bool MoveNext()
            {
                ++_currentIndex;
                return _currentIndex < _collection.Length;
            }

            public void Reset()
            {
                _currentIndex = -1;
            }

            public T Current => _collection[_currentIndex];

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }
        }
    }

    public readonly struct ReadOnlyListAccessor<T> : IEnumerable<T>
    {
        private ReadOnlyListAccessor(List<T> collection) => _collection = collection;
        private readonly List<T> _collection;
        public int Count => _collection.Count;
        public T this[int index] => _collection[index];
        public List<T>.Enumerator GetEnumerator() => _collection.GetEnumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>) _collection).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _collection).GetEnumerator();
        public static implicit operator ReadOnlyListAccessor<T>(List<T> i) => new ReadOnlyListAccessor<T>(i);
    }

    public readonly struct ReadOnlyDictionaryAccessor<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private ReadOnlyDictionaryAccessor(Dictionary<TKey, TValue> collection) => _collection = collection;
        private readonly Dictionary<TKey, TValue> _collection;
        public int Count => _collection.Count;
        public bool ContainsKey(TKey key) => _collection.ContainsKey(key);
        public TValue this[TKey key] => _collection[key];
        public bool TryGetValue(TKey key, out TValue value) => _collection.TryGetValue(key, out value);
        public Dictionary<TKey, TValue>.KeyCollection Keys => _collection.Keys;
        public Dictionary<TKey, TValue>.ValueCollection Values => _collection.Values;
        public Dictionary<TKey, TValue>.Enumerator GetEnumerator() => _collection.GetEnumerator();
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => ((IEnumerable<KeyValuePair<TKey, TValue>>) _collection).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _collection).GetEnumerator();
        public static implicit operator ReadOnlyDictionaryAccessor<TKey, TValue>(Dictionary<TKey, TValue> i) => new ReadOnlyDictionaryAccessor<TKey, TValue>(i);
    }
}