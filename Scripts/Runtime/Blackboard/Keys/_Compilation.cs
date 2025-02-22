using System;

namespace HiraBots
{
    /// <summary>
    /// The context required to compile a blackboard key.
    /// </summary>
    internal unsafe struct BlackboardKeyCompilerContext
    {
        /// <summary>
        /// The address to write the default value at.
        /// </summary>
        internal byte* address { get; set; }

        /// <summary>
        /// The index to cache.
        /// </summary>
        internal ushort index { get; set; }

        /// <summary>
        /// The memory offset to cache.
        /// </summary>
        internal ushort memoryOffset { get; set; }
    }

    internal abstract partial class BlackboardKey
    {
        [NonSerialized] private BlackboardKeyCompiledData m_CompiledDataInternal = BlackboardKeyCompiledData.none;

        /// <summary>
        /// The data compiled for this blackboard key.
        /// </summary>
        internal BlackboardKeyCompiledData compiledData => m_CompiledDataInternal;

        /// <summary>
        /// Whether the blackboard key has been compiled.
        /// </summary>
        internal bool isCompiled => m_CompiledDataInternal.isValid;

        /// <summary>
        /// Compile the blackboard template into more efficiently accessible runtime data.
        /// </summary>
        internal void Compile(ref BlackboardKeyCompilerContext context)
        {
            if (isCompiled)
            {
                // ignore if already compiled
                return;
            }

            // extract traits from the boolean properties
            var traits = BlackboardKeyTraits.None
                         | (m_InstanceSynced ? BlackboardKeyTraits.InstanceSynced : BlackboardKeyTraits.None)
                         | (m_EssentialToDecisionMaking ? BlackboardKeyTraits.BroadcastEventOnUnexpectedChange : BlackboardKeyTraits.None);

            // provide all the necessary data to the context
            m_CompiledDataInternal = new BlackboardKeyCompiledData(name, context.memoryOffset, context.index, traits, m_KeyType);
            CompileInternal(ref context);
        }

        /// <summary>
        /// Free the compiled data.
        /// </summary>
        internal void Free()
        {
            FreeInternal();
            m_CompiledDataInternal = BlackboardKeyCompiledData.none;
        }

        protected abstract void CompileInternal(ref BlackboardKeyCompilerContext context);

        protected virtual void FreeInternal()
        {
        }
    }

    internal unsafe partial class BooleanBlackboardKey
    {
        protected override void CompileInternal(ref BlackboardKeyCompilerContext context)
        {
            BlackboardUnsafeHelpers.WriteBooleanValue(context.address, 0, m_DefaultValue);
        }
    }

    internal unsafe partial class EnumBlackboardKey
    {
        /// <summary>
        /// Helper enum to convert a byte into an enum.
        /// </summary>
        private enum ValueWriterHelper : byte
        {
        }

        protected override void CompileInternal(ref BlackboardKeyCompilerContext context)
        {
            BlackboardUnsafeHelpers.WriteEnumValue<ValueWriterHelper>(context.address, 0, (ValueWriterHelper) m_DefaultValue.m_Value);
        }
    }

    internal unsafe partial class FloatBlackboardKey
    {
        protected override void CompileInternal(ref BlackboardKeyCompilerContext context)
        {
            BlackboardUnsafeHelpers.WriteFloatValue(context.address, 0, m_DefaultValue);
        }
    }

    internal unsafe partial class IntegerBlackboardKey
    {
        protected override void CompileInternal(ref BlackboardKeyCompilerContext context)
        {
            BlackboardUnsafeHelpers.WriteIntegerValue(context.address, 0, m_DefaultValue);
        }
    }

    internal unsafe partial class ObjectBlackboardKey
    {
        protected override void CompileInternal(ref BlackboardKeyCompilerContext context)
        {
            BlackboardUnsafeHelpers.WriteObjectValue(context.address, 0, null);
        }
    }

    internal unsafe partial class QuaternionBlackboardKey
    {
        protected override void CompileInternal(ref BlackboardKeyCompilerContext context)
        {
            BlackboardUnsafeHelpers.WriteQuaternionValue(context.address, 0, UnityEngine.Quaternion.Euler(m_DefaultValue));
        }
    }

    internal unsafe partial class VectorBlackboardKey
    {
        protected override void CompileInternal(ref BlackboardKeyCompilerContext context)
        {
            BlackboardUnsafeHelpers.WriteVectorValue(context.address, 0, m_DefaultValue);
        }
    }
}