using System.Runtime.CompilerServices;

namespace HiraBots
{
    internal unsafe struct LowLevelBlackboardArray
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelBlackboardArray(byte* address, ushort count, ushort size)
        {
            m_Address = address;
            this.count = count;
            m_Size = size;
        }

        private byte* m_Address;
        internal ushort count { get; }
        private readonly ushort m_Size;

        /// <summary>
        /// Access a blackboard from the collection.
        /// </summary>
        internal LowLevelBlackboard this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var output = new LowLevelBlackboard(m_Address + (m_Size * index), m_Size);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                return index >= count
                    ? throw new System.IndexOutOfRangeException($"Index out of range[0-{count}) - " +
                                                                $"(index) {index}.")
                    : output;
#else
                        return output;
#endif
            }
        }
    }
}