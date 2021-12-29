using System.Runtime.CompilerServices;

namespace HiraBots
{
    internal readonly unsafe struct LowLevelBlackboard
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelBlackboard(byte* address, ushort length)
        {
            m_Address = address;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            m_Length = length;
#endif
        }

        // the address of the raw blackboard data
        private readonly byte* m_Address;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        // the size of the blackboard
        private readonly ushort m_Length;
#endif

        /// <summary>
        /// Access a variable from the blackboard.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref T Access<T>(int memoryOffset) where T : unmanaged
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (memoryOffset >= m_Length || memoryOffset + sizeof(T) > m_Length)
            {
                throw new System.IndexOutOfRangeException($"Index out of range [0-{m_Length}) - " +
                                                          $"(offset) {memoryOffset} + (size) {sizeof(T)}");
            }
#endif

            return ref *(T*) (m_Address + memoryOffset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator UnityEngine.BlackboardComponent.LowLevel(LowLevelBlackboard lbb)
        {
            return new UnityEngine.BlackboardComponent.LowLevel(lbb);
        }
    }
}