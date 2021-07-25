using System.Collections.Generic;
using Unity.Collections;

namespace HiraBots
{
    internal interface IBlackboardTemplateCompilerContext
    {
        void GenerateKeyCompilerContext(
            NativeArray<byte> template,
            Dictionary<string,ushort> keyNameToIndex,
            BlackboardKeyCompiledData[] keyData,
            ushort startingIndex,
            ushort startingMemoryOffset);

        IBlackboardKeyCompilerContext KeyCompilerContext { get; }
        void UpdateKeyCompilerContext(ushort memoryOffsetDelta);
    }
}