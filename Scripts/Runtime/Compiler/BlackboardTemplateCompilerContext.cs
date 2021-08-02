﻿using System.Collections.Generic;
using Unity.Collections;

namespace HiraBots
{
    internal class BlackboardTemplateCompilerContext : IBlackboardTemplateCompilerContext
    {
        private BlackboardKeyCompilerContext m_KeyCompilerContext = null;
        IBlackboardKeyCompilerContext IBlackboardTemplateCompilerContext.keyCompilerContext => m_KeyCompilerContext;

        void IBlackboardTemplateCompilerContext.GenerateKeyCompilerContext(
            NativeArray<byte> template,
            Dictionary<string, ushort> keyNameToMemoryOffset,
            Dictionary<ushort, BlackboardKeyCompiledData> memoryOffsetToKeyData,
            ushort startingIndex,
            ushort startingMemoryOffset)
        {
            m_KeyCompilerContext = new BlackboardKeyCompilerContext(
                template,
                keyNameToMemoryOffset,
                memoryOffsetToKeyData,
                startingIndex,
                startingMemoryOffset);
        }

        void IBlackboardTemplateCompilerContext.UpdateKeyCompilerContext(ushort memoryOffsetDelta) =>
            m_KeyCompilerContext.Update(memoryOffsetDelta);

        internal void Update() => m_KeyCompilerContext = null;
    }
}