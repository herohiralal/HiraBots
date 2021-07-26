using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace HiraBots
{
    internal unsafe class BlackboardKeyCompilerContext : IBlackboardKeyCompilerContext
    {
        internal BlackboardKeyCompilerContext(NativeArray<byte> rawData, Dictionary<string, ushort> keyNameToMemoryOffset,
            Dictionary<ushort, BlackboardKeyCompiledData> memoryOffsetToKeyData, ushort startingIndex, ushort startingOffset)
        {
            _rawData = rawData;
            _keyNameToMemoryOffset = keyNameToMemoryOffset;
            _memoryOffsetToKeyData = memoryOffsetToKeyData;
            _index = startingIndex;
            _memoryOffset = startingOffset;
        }

        private readonly NativeArray<byte> _rawData;
        private readonly Dictionary<ushort, BlackboardKeyCompiledData> _memoryOffsetToKeyData;
        private readonly Dictionary<string, ushort> _keyNameToMemoryOffset;
        private ushort _index;
        private ushort _memoryOffset;

        byte* IBlackboardKeyCompilerContext.Address => (byte*) _rawData.GetUnsafePtr() + _memoryOffset;
        ushort IBlackboardKeyCompilerContext.Index => _index;
        ushort IBlackboardKeyCompilerContext.MemoryOffset => _memoryOffset;

        BlackboardKeyCompiledData IBlackboardKeyCompilerContext.CompiledData
        {
            set => _memoryOffsetToKeyData.Add(_memoryOffset, value);
        }

        string IBlackboardKeyCompilerContext.Name
        {
            set => _keyNameToMemoryOffset.Add(value, _memoryOffset);
        }

        internal void Update(ushort offsetDelta)
        {
            _index++;
            _memoryOffset += offsetDelta;
        }
    }
}