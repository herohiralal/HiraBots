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
            _index = startingIndex;
            _memoryOffset = startingOffset;
        }

        private readonly NativeArray<byte> _rawData;
        private readonly BlackboardKeyCompiledData[] _keyData;
        private readonly Dictionary<string, ushort> _keyNameToIndex;
        private ushort _index;
        private ushort _memoryOffset;

        byte* IBlackboardKeyCompilerContext.Address => (byte*) _rawData.GetUnsafePtr() + _memoryOffset;
        ushort IBlackboardKeyCompilerContext.Index => _index;
        ushort IBlackboardKeyCompilerContext.MemoryOffset => _memoryOffset;

        BlackboardKeyCompiledData IBlackboardKeyCompilerContext.CompiledData
        {
            set => _keyData[_index] = value;
        }

        string IBlackboardKeyCompilerContext.Name
        {
            set => _keyNameToIndex.Add(value, _index);
        }

        internal void Update(ushort offsetDelta)
        {
            _index++;
            _memoryOffset += offsetDelta;
        }
    }
}