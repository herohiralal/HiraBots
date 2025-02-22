using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

namespace UnityEngine
{
    public readonly struct ReadOnlyNativeArrayAccessor<T> : IEnumerable<T> where T : struct
    {
        // private constructor, because there's already an implicit cast operator
        private ReadOnlyNativeArrayAccessor(ref NativeArray<T> collection)
        {
            m_Collection = collection;
        }

        // the actual collection
        private readonly NativeArray<T> m_Collection;

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
        public NativeArray<T>.Enumerator GetEnumerator()
        {
            var collection = m_Collection;
            return new NativeArray<T>.Enumerator(ref collection);
        }

        // IEnumerable<T> explicit implementation to prioritize the custom enumerator in a foreach loop.
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>) m_Collection).GetEnumerator();

        // IEnumerable explicit implementation to prioritize the custom enumerator in a foreach loop.
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) m_Collection).GetEnumerator();

        /// <summary>
        /// Get the read-only interface for an array.
        /// </summary>
        public static implicit operator ReadOnlyNativeArrayAccessor<T>(NativeArray<T> i)
        {
            return new ReadOnlyNativeArrayAccessor<T>(ref i);
        }
    }
}