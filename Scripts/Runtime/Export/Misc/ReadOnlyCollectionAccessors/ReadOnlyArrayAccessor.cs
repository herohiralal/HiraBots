using System.Collections;
using System.Collections.Generic;

namespace UnityEngine
{
    /// <summary>
    /// Read-only accessor for an array.
    /// </summary>
    public readonly struct ReadOnlyArrayAccessor<T> : IEnumerable<T>
    {
        // private constructor, because there's already an implicit cast operator
        private ReadOnlyArrayAccessor(T[] collection)
        {
            m_Collection = collection;
        }

        // the actual collection
        private readonly T[] m_Collection;

        /// <summary>
        /// The length of this array.
        /// </summary>
        public int count => m_Collection.Length;

        /// <summary>
        /// Access an element in the array.
        /// </summary>
        public T this[int index] => m_Collection[index];

        /// <summary>
        /// Enumerate over this array.
        /// </summary>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(m_Collection);
        }

        // IEnumerable<T> explicit implementation to prioritize the custom enumerator in a foreach loop.
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>) m_Collection).GetEnumerator();
        }

        // IEnumerable explicit implementation to prioritize the custom enumerator in a foreach loop.
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) m_Collection).GetEnumerator();
        }

        /// <summary>
        /// Get the read-only interface for an array.
        /// </summary>
        public static implicit operator ReadOnlyArrayAccessor<T>(T[] i)
        {
            return new ReadOnlyArrayAccessor<T>(i);
        }

        /// <summary>
        /// Custom struct-based enumerator to prevent garbage allocation.
        /// </summary>
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
}