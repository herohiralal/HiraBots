using System;
using Unity.Mathematics;

namespace HiraBots
{
    /// <summary>
    /// The context required to compile a blackboard key.
    /// </summary>
    internal unsafe interface IBlackboardKeyCompilerContext
    {
        /// <summary>
        /// The address to write the default value at.
        /// </summary>
        byte* address { get; }

        /// <summary>
        /// The index to cache.
        /// </summary>
        ushort index { get; }

        /// <summary>
        /// The memory offset to cache.
        /// </summary>
        ushort memoryOffset { get; }

        /// <summary>
        /// The compiled data to provide.
        /// </summary>
        BlackboardKeyCompiledData compiledData { set; }

        /// <summary>
        /// The name of the key to provide.
        /// </summary>
        string name { set; }
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
        internal void Compile(IBlackboardKeyCompilerContext context)
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
            m_CompiledDataInternal = new BlackboardKeyCompiledData(context.memoryOffset, context.index, traits, m_KeyType);
            context.compiledData = m_CompiledDataInternal;
            context.name = name;
            CompileInternal(context);
        }

        /// <summary>
        /// Free the compiled data.
        /// </summary>
        internal void Free()
        {
            FreeInternal();
            m_CompiledDataInternal = BlackboardKeyCompiledData.none;
        }

        protected abstract void CompileInternal(IBlackboardKeyCompilerContext context);

        protected virtual void FreeInternal()
        {
        }
    }

    internal unsafe partial class BooleanBlackboardKey
    {
        protected override void CompileInternal(IBlackboardKeyCompilerContext context)
        {
            BlackboardUnsafeHelpers.WriteBooleanValue(context.address, 0, m_DefaultValue);
        }
    }

    internal unsafe partial class EnumBlackboardKey
    {
        private enum ValueWriterHelper : byte
        {
        }

        protected override void CompileInternal(IBlackboardKeyCompilerContext context)
        {
            BlackboardUnsafeHelpers.WriteEnumValue<ValueWriterHelper>(context.address, 0, (ValueWriterHelper) m_DefaultValue.m_Value);
        }
    }

    internal unsafe partial class FloatBlackboardKey
    {
        protected override void CompileInternal(IBlackboardKeyCompilerContext context)
        {
            BlackboardUnsafeHelpers.WriteFloatValue(context.address, 0, m_DefaultValue);
        }
    }

    internal unsafe partial class IntegerBlackboardKey
    {
        protected override void CompileInternal(IBlackboardKeyCompilerContext context)
        {
            BlackboardUnsafeHelpers.WriteIntegerValue(context.address, 0, m_DefaultValue);
        }
    }

    internal unsafe partial class ObjectBlackboardKey
    {
        protected override void CompileInternal(IBlackboardKeyCompilerContext context)
        {
            // write the object value as default value to register it in the object cache
            // and then pin the object to freeze its count
            BlackboardUnsafeHelpers.WriteObjectValue(context.address, 0, m_DefaultValue);
            BlackboardUnsafeHelpers.Pin(m_DefaultValue);
        }

        protected override void FreeInternal()
        {
            // release the object to unfreeze its count
            // and then write the object value as null to unregister it from the object cache
            BlackboardUnsafeHelpers.Release(m_DefaultValue);
            var instanceID = m_DefaultValue.GetInstanceID();
            BlackboardUnsafeHelpers.WriteObjectValue((byte*) &instanceID, 0, null);
        }
    }

    internal unsafe partial class QuaternionBlackboardKey
    {
        protected override void CompileInternal(IBlackboardKeyCompilerContext context)
        {
            BlackboardUnsafeHelpers.WriteQuaternionValue(context.address, 0, quaternion.Euler(m_DefaultValue));
        }
    }

    internal unsafe partial class VectorBlackboardKey
    {
        protected override void CompileInternal(IBlackboardKeyCompilerContext context)
        {
            BlackboardUnsafeHelpers.WriteVectorValue(context.address, 0, m_DefaultValue);
        }
    }
}