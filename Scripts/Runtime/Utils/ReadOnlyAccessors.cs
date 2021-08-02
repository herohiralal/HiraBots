using System.Collections;
using System.Collections.Generic;

namespace HiraBots
{
    public readonly struct ReadOnlyArrayAccessor<T> : IEnumerable<T>
    {
        private ReadOnlyArrayAccessor(T[] collection) => m_Collection = collection;
        private readonly T[] m_Collection;
        public int count => m_Collection.Length;
        public T this[int index] => m_Collection[index];
        public Enumerator GetEnumerator() => new Enumerator(m_Collection);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>) m_Collection).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) m_Collection).GetEnumerator();
        public static implicit operator ReadOnlyArrayAccessor<T>(T[] i) => new ReadOnlyArrayAccessor<T>(i);

        public struct Enumerator : IEnumerator<T>
        {
            public Enumerator(T[] collection)
            {
                m_Collection = collection;
                m_CurrentIndex = -1;
            }

            private readonly T[] m_Collection;
            private int m_CurrentIndex;

            public bool MoveNext()
            {
                ++m_CurrentIndex;
                return m_CurrentIndex < m_Collection.Length;
            }

            public void Reset()
            {
                m_CurrentIndex = -1;
            }

            public T Current => m_Collection[m_CurrentIndex];

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }
        }
    }

    public readonly struct ReadOnlyListAccessor<T> : IEnumerable<T>
    {
        private ReadOnlyListAccessor(List<T> collection) => m_Collection = collection;
        private readonly List<T> m_Collection;
        public int count => m_Collection.Count;
        public T this[int index] => m_Collection[index];
        public List<T>.Enumerator GetEnumerator() => m_Collection.GetEnumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>) m_Collection).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) m_Collection).GetEnumerator();
        public static implicit operator ReadOnlyListAccessor<T>(List<T> i) => new ReadOnlyListAccessor<T>(i);
    }

    public readonly struct ReadOnlyDictionaryAccessor<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private ReadOnlyDictionaryAccessor(Dictionary<TKey, TValue> collection) => m_Collection = collection;
        private readonly Dictionary<TKey, TValue> m_Collection;
        public int count => m_Collection.Count;
        public bool ContainsKey(TKey key) => m_Collection.ContainsKey(key);
        public TValue this[TKey key] => m_Collection[key];
        public bool TryGetValue(TKey key, out TValue value) => m_Collection.TryGetValue(key, out value);
        public Dictionary<TKey, TValue>.KeyCollection keys => m_Collection.Keys;
        public Dictionary<TKey, TValue>.ValueCollection values => m_Collection.Values;
        public Dictionary<TKey, TValue>.Enumerator GetEnumerator() => m_Collection.GetEnumerator();
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => ((IEnumerable<KeyValuePair<TKey, TValue>>) m_Collection).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) m_Collection).GetEnumerator();
        public static implicit operator ReadOnlyDictionaryAccessor<TKey, TValue>(Dictionary<TKey, TValue> i) => new ReadOnlyDictionaryAccessor<TKey, TValue>(i);
    }
}