using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace HiraBots
{
    internal unsafe class BlackboardKeyCompilerContext : IBlackboardKeyCompilerContext
    {
        internal BlackboardKeyCompilerContext(NativeArray<byte> rawData, Dictionary<string, ushort> keyNameToIndex,
            BlackboardKeyCompiledData[] keyData, ushort startingIndex, ushort startingOffset)
        {
            _rawData = rawData;
            _keyNameToIndex = keyNameToIndex;
            _keyData = keyData;
            Index = startingIndex;
            MemoryOffset = startingOffset;
        }

        private readonly NativeArray<byte> _rawData;
        private readonly BlackboardKeyCompiledData[] _keyData;
        private readonly Dictionary<string, ushort> _keyNameToIndex;
        public byte* Address => (byte*) _rawData.GetUnsafePtr() + MemoryOffset;
        public ushort Index { get; private set; }
        public ushort MemoryOffset { get; private set; }

        public BlackboardKeyCompiledData CompiledData
        {
            set => _keyData[Index] = value;
        }

        public string Name
        {
            set => _keyNameToIndex.Add(value, Index);
        }

        internal void Update(ushort offsetDelta)
        {
            Index++;
            MemoryOffset += offsetDelta;
        }
    }
}