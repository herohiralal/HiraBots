using System.Collections.Generic;
using Unity.Collections;

namespace HiraBots
{
    internal class BlackboardTemplateCompilerContext : IBlackboardTemplateCompilerContext
    {
        private BlackboardKeyCompilerContext _keyCompilerContext = null;
        public IBlackboardKeyCompilerContext KeyCompilerContext => _keyCompilerContext;

        public void GenerateKeyCompilerContext(
            NativeArray<byte> template,
            Dictionary<string, ushort> keyNameToIndex,
            BlackboardKeyCompiledData[] keyData,
            ushort startingIndex,
            ushort startingMemoryOffset)
        {
            _keyCompilerContext = new BlackboardKeyCompilerContext(
                template,
                keyNameToIndex,
                keyData,
                startingIndex,
                startingMemoryOffset);
        }

        public void UpdateKeyCompilerContext(ushort memoryOffsetDelta) =>
            _keyCompilerContext.Update(memoryOffsetDelta);

        internal void Update() => _keyCompilerContext = null;
    }
}