using System.Runtime.CompilerServices;

namespace HiraBots
{
    /// <summary>
    /// Low-level representation of a blackboard function collection.
    /// </summary>
    internal readonly unsafe struct LowLevelBlackboardFunctionCollection
    {
        private readonly byte* m_Address;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private LowLevelBlackboardFunctionCollection(byte* address)
        {
            m_Address = address;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator LowLevelBlackboardFunctionCollection(byte* stream)
        {
            return new LowLevelBlackboardFunctionCollection(stream);
        }

        // no offset
        /// <summary>
        /// The size of this collection.
        /// </summary>
        internal int size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ByteStreamHelpers.JumpOverNothing(m_Address).AndAccess<int>();
        }

        // offset size
        /// <summary>
        /// The number of elements within this collection.
        /// </summary>
        internal int count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ByteStreamHelpers.JumpOver<int>(m_Address).AndAccess<int>();
        }

        // offset size & count
        /// <summary>
        /// Get an enumerator to iterate over this collection.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator GetEnumerator()
        {
            return new Enumerator(ByteStreamHelpers.JumpOver<int, int>(m_Address).AndGetAPointerOf<byte>(), count);
        }

        /// <summary>
        /// Enumerator to iterate over a low level blackboard function collection
        /// </summary>
        internal struct Enumerator
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(byte* current, int count)
            {
                m_Current = current;
                m_CurrentIndex = -1;
                m_Count = count;
            }

            private byte* m_Current;
            private int m_CurrentIndex;
            private readonly int m_Count;

            /// <summary>
            /// Move the iterator to the next index.
            /// </summary>
            /// <returns>Whether the element is valid.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal bool MoveNext()
            {
                if (++m_CurrentIndex >= m_Count)
                {
                    return false;
                }

                m_Current += ((LowLevelBlackboardFunction) m_Current).size;
                return true;

            }

            /// <summary>
            /// Current element in the iterator.
            /// </summary>
            internal LowLevelBlackboardFunction current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => m_Current;
            }
        }
    }
}