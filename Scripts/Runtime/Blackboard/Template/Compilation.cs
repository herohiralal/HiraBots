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
            Dictionary<string, ushort> keyNameToIndex,
            BlackboardKeyCompiledData[] keyData,
            ushort startingIndex,
            ushort startingMemoryOffset);

        IBlackboardKeyCompilerContext KeyCompilerContext { get; }
        void UpdateKeyCompilerContext(ushort memoryOffsetDelta);
    }

    internal abstract partial class BlackboardKey
    {
        [NonSerialized] protected internal BlackboardKeyCompiledData CompiledDataInternal = null;
        internal BlackboardKeyCompiledData CompiledData => CompiledDataInternal;

        internal bool IsCompiled => CompiledData != null;

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
            CompiledDataInternal = null;
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
        protected override void CompileInternal(IBlackboardKeyCompilerContext context) =>
            BlackboardUnsafeHelpers.WriteObjectValue(context.Address, 0, defaultValue);
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

            var totalTemplateSize = 0;
            foreach (var key in keys) totalTemplateSize += key.SizeInBytes;

            var totalKeyCount = keys.Length;

            if (parentCompiledData != null)
            {
                startingMemoryOffset = parentCompiledData.TemplateSize;
                startingIndex = parentCompiledData.KeyCount;

                totalTemplateSize += startingMemoryOffset;
                totalKeyCount += startingIndex;
            }

            var template = new NativeArray<byte>(totalTemplateSize, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            var keyNameToIndex = new Dictionary<string, ushort>();
            var keyData = new BlackboardKeyCompiledData[totalKeyCount];

            if (parentCompiledData != null)
            {
                parentCompiledData.CopyTemplateTo(template);
                foreach (var kvp in parentCompiledData.KeyNameToIndex) keyNameToIndex.Add(kvp.Key, kvp.Value);
                for (var i = 0; i < startingIndex; i++) keyData[i] = parentCompiledData.KeyData[i];
            }

            context.GenerateKeyCompilerContext(template, keyNameToIndex, keyData, startingIndex, startingMemoryOffset);

            foreach (var key in keys.OrderBy(k => k.SizeInBytes))
            {
                key.Compile(context.KeyCompilerContext);
                context.UpdateKeyCompilerContext(key.SizeInBytes);
            }

            CompiledData = new BlackboardTemplateCompiledData(template, keyNameToIndex, keyData);
        }

        internal void Free()
        {
            if (parent != null && !parent.IsCompiled)
                Debug.LogError("Parent blackboard decompiled before self.", this);

            foreach (var key in keys) key.Free();
            CompiledData = null;
        }
    }
}