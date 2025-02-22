using System.Collections;
using System.Collections.Generic;

namespace UnityEngine
{
    public readonly struct ReadOnlyListAccessor<T> : IEnumerable<T>
    {
        // private constructor, because there's already an implicit cast operator
        private ReadOnlyListAccessor(List<T> collection)
        {
            m_Collection = collection;
        }

        // the actual collection
        private readonly List<T> m_Collection;

        /// <summary>
        /// The number of elements in this list.
        /// </summary>
        public int count => m_Collection.Count;

        /// <summary>
        /// Access an element in the list.
        /// </summary>
        public T this[int index] => m_Collection[index];

        /// <summary>
        /// Enumerate over this list.
        /// </summary>
        public List<T>.Enumerator GetEnumerator()
        {
            return m_Collection.GetEnumerator();
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
        /// Get the read-only interface for an list.
        /// </summary>
        public static implicit operator ReadOnlyListAccessor<T>(List<T> i)
        {
            return new ReadOnlyListAccessor<T>(i);
        }
    }
}