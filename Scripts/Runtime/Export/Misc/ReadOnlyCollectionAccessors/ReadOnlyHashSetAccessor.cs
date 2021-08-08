using System.Collections;
using System.Collections.Generic;

namespace UnityEngine
{
    /// <summary>
    /// Read-only accessor for a hash-set.
    /// </summary>
    public readonly struct ReadOnlyHashSetAccessor<T> : IEnumerable<T>
    {
        // private constructor, because there's already an implicit cast operator
        private ReadOnlyHashSetAccessor(HashSet<T> collection)
        {
            m_Collection = collection;
        }

        // the actual collection
        private readonly HashSet<T> m_Collection;

        /// <summary>
        /// The number of elements in this hash-set.
        /// </summary>
        public int count => m_Collection.Count;

        /// <summary>
        /// Check whether the hash-set contains an element.
        /// </summary>
        public bool Contains(T item) => m_Collection.Contains(item);

        /// <summary>
        /// Enumerate over this hash-set.
        /// </summary>
        public HashSet<T>.Enumerator GetEnumerator()
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
        public static implicit operator ReadOnlyHashSetAccessor<T>(HashSet<T> i)
        {
            return new ReadOnlyHashSetAccessor<T>(i);
        }
    }
}