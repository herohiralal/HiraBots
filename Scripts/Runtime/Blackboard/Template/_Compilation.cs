using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace HiraBots
{
    internal unsafe interface IBlackboardKeyCompilerContext
    {
        byte* address { get; }
        ushort index { get; }
        ushort memoryOffset { get; }
        BlackboardKeyCompiledData compiledData { set; }
        string name { set; }
    }

    internal interface IBlackboardTemplateCompilerContext
    {
        void GenerateKeyCompilerContext(
            NativeArray<byte> template,
            Dictionary<string, ushort> keyNameToMemoryOffset,
            Dictionary<ushort, BlackboardKeyCompiledData> memoryOffsetToKeyData,
            ushort startingIndex,
            ushort startingMemoryOffset);

        IBlackboardKeyCompilerContext keyCompilerContext { get; }
        void UpdateKeyCompilerContext(ushort memoryOffsetDelta);
    }

    internal abstract partial class BlackboardKey
    {
        [NonSerialized] protected internal BlackboardKeyCompiledData m_CompiledDataInternal = BlackboardKeyCompiledData.none;
        internal BlackboardKeyCompiledData compiledData => m_CompiledDataInternal;

        internal bool isCompiled => m_CompiledDataInternal.isValid;

        internal void Compile(IBlackboardKeyCompilerContext context)
        {
            if (isCompiled)
                return;

            var traits = BlackboardKeyTraits.None
                         | (instanceSynced ? BlackboardKeyTraits.InstanceSynced : BlackboardKeyTraits.None)
                         | (essentialToDecisionMaking ? BlackboardKeyTraits.BroadcastEventOnUnexpectedChange : BlackboardKeyTraits.None);

            m_CompiledDataInternal = new BlackboardKeyCompiledData(context.memoryOffset, context.index, traits, m_KeyType);
            context.compiledData = m_CompiledDataInternal;
            context.name = name;
            CompileInternal(context);
        }

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
        protected override void CompileInternal(IBlackboardKeyCompilerContext context) =>
            BlackboardUnsafeHelpers.WriteBooleanValue(context.address, 0, defaultValue);
    }

    internal unsafe partial class EnumBlackboardKey
    {
        protected override void CompileInternal(IBlackboardKeyCompilerContext context) =>
            BlackboardUnsafeHelpers.WriteRawEnumValue(context.address, 0, defaultValue);
    }

    internal unsafe partial class FloatBlackboardKey
    {
        protected override void CompileInternal(IBlackboardKeyCompilerContext context) =>
            BlackboardUnsafeHelpers.WriteFloatValue(context.address, 0, defaultValue);
    }

    internal unsafe partial class IntegerBlackboardKey
    {
        protected override void CompileInternal(IBlackboardKeyCompilerContext context) =>
            BlackboardUnsafeHelpers.WriteIntegerValue(context.address, 0, defaultValue);
    }

    internal unsafe partial class ObjectBlackboardKey
    {
        protected override void CompileInternal(IBlackboardKeyCompilerContext context)
        {
            BlackboardUnsafeHelpers.WriteObjectValue(context.address, 0, defaultValue);
            BlackboardUnsafeHelpers.Pin(defaultValue);
        }

        protected override void FreeInternal()
        {
            BlackboardUnsafeHelpers.Release(defaultValue);
            var instanceID = defaultValue.GetInstanceID();
            BlackboardUnsafeHelpers.WriteObjectValue((byte*) &instanceID, 0, null);
        }
    }

    internal unsafe partial class QuaternionBlackboardKey
    {
        protected override void CompileInternal(IBlackboardKeyCompilerContext context) =>
            BlackboardUnsafeHelpers.WriteQuaternionValue(context.address, 0, Quaternion.Euler(defaultValue));
    }

    internal unsafe partial class VectorBlackboardKey
    {
        protected override void CompileInternal(IBlackboardKeyCompilerContext context) =>
            BlackboardUnsafeHelpers.WriteVectorValue(context.address, 0, defaultValue);
    }

    internal partial class BlackboardTemplate
    {
        internal BlackboardTemplateCompiledData compiledData { get; private set; } = null;
        internal bool isCompiled => compiledData != null;

        internal void Compile(IBlackboardTemplateCompilerContext context)
        {
            if (isCompiled)
                return;

            var parentCompiledData = (BlackboardTemplateCompiledData) null;
            if (parent != null)
            {
                if (!parent.isCompiled)
                {
                    Debug.LogError("Attempted to compile blackboard before its parent.", this);
                    return;
                }

                parentCompiledData = parent.compiledData;
            }

            ushort startingMemoryOffset = 0, startingIndex = 0;

            var totalTemplateSize = (ushort) 0;
            foreach (var key in keys) totalTemplateSize += key.sizeInBytes;

            var totalKeyCount = (ushort) keys.Length;

            if (parentCompiledData != null)
            {
                startingMemoryOffset = parentCompiledData.templateSize;
                startingIndex = parentCompiledData.m_KeyCount;

                totalTemplateSize += startingMemoryOffset;
                totalKeyCount += startingIndex;
            }

            var template = new NativeArray<byte>(totalTemplateSize, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            var keyNameToMemoryOffset = new Dictionary<string, ushort>(totalKeyCount);
            var memoryOffsetToKeyData = new Dictionary<ushort, BlackboardKeyCompiledData>(totalKeyCount);

            if (parentCompiledData != null)
            {
                parentCompiledData.CopyTemplateTo(template);
                foreach (var kvp in parentCompiledData.keyNameToMemoryOffset)
                    keyNameToMemoryOffset.Add(kvp.Key, kvp.Value);
                foreach (var kvp in parentCompiledData.memoryOffsetToKeyData)
                    memoryOffsetToKeyData.Add(kvp.Key, kvp.Value);
            }

            context.GenerateKeyCompilerContext(template, keyNameToMemoryOffset, memoryOffsetToKeyData, startingIndex, startingMemoryOffset);

            foreach (var key in keys.OrderBy(k => k.sizeInBytes))
            {
                key.Compile(context.keyCompilerContext);
                context.UpdateKeyCompilerContext(key.sizeInBytes);
            }

            compiledData = new BlackboardTemplateCompiledData(parentCompiledData, template, keyNameToMemoryOffset, memoryOffsetToKeyData, totalKeyCount);
        }

        internal void Free()
        {
            foreach (var key in keys) key.Free();
            compiledData = null;
        }
    }
}