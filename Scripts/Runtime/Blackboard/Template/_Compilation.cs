using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace HiraBots
{
    internal unsafe interface IBlackboardKeyCompilerContext
    {
        byte* Address { get; }
        ushort Index { get; }
        ushort MemoryOffset { get; }
        BlackboardKeyCompiledData CompiledData { set; }
        string Name { set; }
    }

    internal interface IBlackboardTemplateCompilerContext
    {
        void GenerateKeyCompilerContext(
            NativeArray<byte> template,
            Dictionary<string, ushort> keyNameToMemoryOffset,
            Dictionary<ushort, BlackboardKeyCompiledData> memoryOffsetToKeyData,
            ushort startingIndex,
            ushort startingMemoryOffset);

        IBlackboardKeyCompilerContext KeyCompilerContext { get; }
        void UpdateKeyCompilerContext(ushort memoryOffsetDelta);
    }

    internal abstract partial class BlackboardKey
    {
        [NonSerialized] protected internal BlackboardKeyCompiledData CompiledDataInternal = BlackboardKeyCompiledData.None;
        internal BlackboardKeyCompiledData CompiledData => CompiledDataInternal;

        internal bool IsCompiled => CompiledDataInternal.IsValid;

        internal void Compile(IBlackboardKeyCompilerContext context)
        {
            if (IsCompiled)
                return;

            var traits = BlackboardKeyTraits.None
                         | (instanceSynced ? BlackboardKeyTraits.InstanceSynced : BlackboardKeyTraits.None)
                         | (essentialToDecisionMaking ? BlackboardKeyTraits.BroadcastEventOnUnexpectedChange : BlackboardKeyTraits.None);

            CompiledDataInternal = new BlackboardKeyCompiledData(context.MemoryOffset, context.Index, traits, KeyType);
            context.CompiledData = CompiledDataInternal;
            context.Name = name;
            CompileInternal(context);
        }

        internal void Free()
        {
            FreeInternal();
            CompiledDataInternal = BlackboardKeyCompiledData.None;
        }

        protected abstract void CompileInternal(IBlackboardKeyCompilerContext context);

        protected virtual void FreeInternal()
        {
        }
    }

    internal unsafe partial class BooleanBlackboardKey
    {
        protected override void CompileInternal(IBlackboardKeyCompilerContext context) =>
            BlackboardUnsafeHelpers.WriteBooleanValue(context.Address, 0, defaultValue);
    }

    internal unsafe partial class FloatBlackboardKey
    {
        protected override void CompileInternal(IBlackboardKeyCompilerContext context) =>
            BlackboardUnsafeHelpers.WriteFloatValue(context.Address, 0, defaultValue);
    }

    internal unsafe partial class IntegerBlackboardKey
    {
        protected override void CompileInternal(IBlackboardKeyCompilerContext context) =>
            BlackboardUnsafeHelpers.WriteIntegerValue(context.Address, 0, defaultValue);
    }

    internal unsafe partial class ObjectBlackboardKey
    {
        protected override void CompileInternal(IBlackboardKeyCompilerContext context)
        {
            BlackboardUnsafeHelpers.WriteObjectValue(context.Address, 0, defaultValue);
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
            BlackboardUnsafeHelpers.WriteQuaternionValue(context.Address, 0, Quaternion.Euler(defaultValue));
    }

    internal unsafe partial class VectorBlackboardKey
    {
        protected override void CompileInternal(IBlackboardKeyCompilerContext context) =>
            BlackboardUnsafeHelpers.WriteVectorValue(context.Address, 0, defaultValue);
    }

    internal partial class BlackboardTemplate
    {
        internal BlackboardTemplateCompiledData CompiledData { get; private set; } = null;
        internal bool IsCompiled => CompiledData != null;

        internal void Compile(IBlackboardTemplateCompilerContext context)
        {
            if (IsCompiled)
                return;

            var parentCompiledData = (BlackboardTemplateCompiledData) null;
            if (parent != null)
            {
                if (!parent.IsCompiled)
                {
                    Debug.LogError("Attempted to compile blackboard before its parent.", this);
                    return;
                }

                parentCompiledData = parent.CompiledData;
            }

            ushort startingMemoryOffset = 0, startingIndex = 0;

            var totalTemplateSize = (ushort) 0;
            foreach (var key in keys) totalTemplateSize += key.SizeInBytes;

            var totalKeyCount = (ushort) keys.Length;

            if (parentCompiledData != null)
            {
                startingMemoryOffset = parentCompiledData.TemplateSize;
                startingIndex = parentCompiledData.KeyCount;

                totalTemplateSize += startingMemoryOffset;
                totalKeyCount += startingIndex;
            }

            var template = new NativeArray<byte>(totalTemplateSize, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            var keyNameToMemoryOffset = new Dictionary<string, ushort>(totalKeyCount);
            var memoryOffsetToKeyData = new Dictionary<ushort, BlackboardKeyCompiledData>(totalKeyCount);

            if (parentCompiledData != null)
            {
                parentCompiledData.CopyTemplateTo(template);
                foreach (var kvp in parentCompiledData.KeyNameToMemoryOffset)
                    keyNameToMemoryOffset.Add(kvp.Key, kvp.Value);
                foreach (var kvp in parentCompiledData.MemoryOffsetToKeyData)
                    memoryOffsetToKeyData.Add(kvp.Key, kvp.Value);
            }

            context.GenerateKeyCompilerContext(template, keyNameToMemoryOffset, memoryOffsetToKeyData, startingIndex, startingMemoryOffset);

            foreach (var key in keys.OrderBy(k => k.SizeInBytes))
            {
                key.Compile(context.KeyCompilerContext);
                context.UpdateKeyCompilerContext(key.SizeInBytes);
            }

            CompiledData = new BlackboardTemplateCompiledData(parentCompiledData, template, keyNameToMemoryOffset, memoryOffsetToKeyData, totalKeyCount);
        }

        internal void Free()
        {
            foreach (var key in keys) key.Free();
            CompiledData = null;
        }
    }
}