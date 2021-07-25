using System.Collections.Generic;
using Unity.Collections;

namespace HiraBots
{
    internal class BlackboardTemplateCompilerContext : IBlackboardTemplateCompilerContext
    {
        private BlackboardKeyCompilerContext _keyCompilerContext = null;
        IBlackboardKeyCompilerContext IBlackboardTemplateCompilerContext.KeyCompilerContext => _keyCompilerContext;

        void IBlackboardTemplateCompilerContext.GenerateKeyCompilerContext(
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

        void IBlackboardTemplateCompilerContext.UpdateKeyCompilerContext(ushort memoryOffsetDelta) =>
            _keyCompilerContext.Update(memoryOffsetDelta);

        internal void Update() => _keyCompilerContext = null;
    }
}