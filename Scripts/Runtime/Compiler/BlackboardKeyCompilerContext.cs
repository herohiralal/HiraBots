using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace HiraBots
{
    /// <summary>
    /// Context to compile a blackboard key.
    /// </summary>
    internal unsafe class BlackboardKeyCompilerContext : IBlackboardKeyCompilerContext
    {
        internal BlackboardKeyCompilerContext(NativeArray<byte> rawData, Dictionary<string, ushort> keyNameToMemoryOffset,
            Dictionary<ushort, BlackboardKeyCompiledData> memoryOffsetToKeyData, ushort startingIndex, ushort startingOffset)
        {
            m_RawData = rawData;
            m_KeyNameToMemoryOffset = keyNameToMemoryOffset;
            m_MemoryOffsetToKeyData = memoryOffsetToKeyData;
            m_Index = startingIndex;
            m_MemoryOffset = startingOffset;
        }

        private readonly NativeArray<byte> m_RawData;
        private readonly Dictionary<ushort, BlackboardKeyCompiledData> m_MemoryOffsetToKeyData;
        private readonly Dictionary<string, ushort> m_KeyNameToMemoryOffset;
        private ushort m_Index;
        private ushort m_MemoryOffset;

        byte* IBlackboardKeyCompilerContext.address => (byte*) m_RawData.GetUnsafePtr() + m_MemoryOffset;
        ushort IBlackboardKeyCompilerContext.index => m_Index;
        ushort IBlackboardKeyCompilerContext.memoryOffset => m_MemoryOffset;

        BlackboardKeyCompiledData IBlackboardKeyCompilerContext.compiledData
        {
            set => m_MemoryOffsetToKeyData.Add(m_MemoryOffset, value);
        }

        string IBlackboardKeyCompilerContext.name
        {
            set => m_KeyNameToMemoryOffset.Add(value, m_MemoryOffset);
        }

        /// <summary>
        /// Reset this object, for reuse with the next key.
        /// </summary>
        internal void Reset(ushort offsetDelta)
        {
            m_Index++;
            m_MemoryOffset += offsetDelta;
        }
    }
}