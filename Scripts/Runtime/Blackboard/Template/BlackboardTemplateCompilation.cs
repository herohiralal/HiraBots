using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace HiraBots
{
    internal partial class BlackboardTemplate
    {
        internal BlackboardTemplateCompiledData CompiledData { get; private set; } = null;
        internal bool IsCompiled => CompiledData != null;

        public void Compile()
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

            var keyCompilerContext = new BlackboardKeyCompilerContext(template, keyNameToIndex, keyData,
                startingIndex, startingMemoryOffset);

            foreach (var key in keys.OrderBy(k => k.SizeInBytes))
            {
                key.Compile(keyCompilerContext);
                keyCompilerContext.Update(key.SizeInBytes);
            }

            CompiledData = new BlackboardTemplateCompiledData(template, keyNameToIndex, keyData);
        }

        public void Free()
        {
            if (parent != null && !parent.IsCompiled)
                Debug.LogError("Parent blackboard decompiled before self.", this);

            foreach (var key in keys) key.Free();
            CompiledData = null;
        }
    }
}