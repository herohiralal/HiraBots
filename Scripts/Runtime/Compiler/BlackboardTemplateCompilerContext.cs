using System.Collections.Generic;
using Unity.Collections;

namespace HiraBots
{
    /// <summary>
    /// Context to compile a blackboard template.
    /// </summary>
    internal class BlackboardTemplateCompilerContext : IBlackboardTemplateCompilerContext
    {
        // the current key compiler context
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

        void IBlackboardTemplateCompilerContext.UpdateKeyCompilerContext(ushort memoryOffsetDelta)
        {
            m_KeyCompilerContext.Reset(memoryOffsetDelta);
        }

        /// <summary>
        /// Reset this object, for reuse.
        /// </summary>
        internal void Reset()
        {
            m_KeyCompilerContext = null;
        }
    }
}