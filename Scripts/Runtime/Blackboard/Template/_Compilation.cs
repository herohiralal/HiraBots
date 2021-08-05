using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// The context required to compile a blackboard template.
    /// </summary>
    internal interface IBlackboardTemplateCompilerContext
    {
        /// <summary>
        /// Generate a key compiler context.
        /// </summary>
        void GenerateKeyCompilerContext(
            NativeArray<byte> template,
            Dictionary<string, ushort> keyNameToMemoryOffset,
            Dictionary<ushort, BlackboardKeyCompiledData> memoryOffsetToKeyData,
            ushort startingIndex,
            ushort startingMemoryOffset);

        /// <summary>
        /// The key compiler context (if generated).
        /// </summary>
        IBlackboardKeyCompilerContext keyCompilerContext { get; }

        /// <summary>
        /// Update the key compiler context for use with a new key.
        /// </summary>
        /// <param name="memoryOffsetDelta">The delta to shift the memory offset by.</param>
        void UpdateKeyCompilerContext(ushort memoryOffsetDelta);
    }

    internal partial class BlackboardTemplate
    {
        /// <summary>
        /// The data compiled for this blackboard template.
        /// </summary>
        internal BlackboardTemplateCompiledData compiledData { get; private set; } = null;

        /// <summary>
        /// Whether the blackboard template has been compiled.
        /// </summary>
        internal bool isCompiled => compiledData != null;

        /// <summary>
        /// Compile this blackboard template.
        /// </summary>
        internal void Compile(IBlackboardTemplateCompilerContext context)
        {
            if (isCompiled)
            {
                // ignore if already compiled
                return;
            }

            var parentCompiledData = (BlackboardTemplateCompiledData) null;
            if (m_Parent != null)
            {
                if (!m_Parent.isCompiled)
                {
                    // the parent must be compiled before self
                    Debug.LogError("Attempted to compile blackboard before its parent.", this);
                    return;
                }

                parentCompiledData = m_Parent.compiledData;
            }

            ushort startingMemoryOffset = 0, startingIndex = 0;

            // calculate total template size
            var totalTemplateSize = (ushort) 0;
            foreach (var key in m_Keys)
            {
                totalTemplateSize += key.sizeInBytes;
            }

            // calculate total key count
            var totalKeyCount = (ushort) m_Keys.Length;

            // update the supposed values with parent blackboard template's data
            if (parentCompiledData != null)
            {
                startingMemoryOffset = parentCompiledData.templateSize;
                startingIndex = parentCompiledData.keyCount;

                totalTemplateSize += startingMemoryOffset;
                totalKeyCount += startingIndex;
            }

            var template = new NativeArray<byte>(UnsafeHelpers.GetAlignedSize(totalTemplateSize), Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            var keyNameToMemoryOffset = new Dictionary<string, ushort>(totalKeyCount);
            var memoryOffsetToKeyData = new Dictionary<ushort, BlackboardKeyCompiledData>(totalKeyCount);

            // update the created collections with the data from parent blackboard template
            if (parentCompiledData != null)
            {
                parentCompiledData.CopyTemplateTo(template);

                foreach (var kvp in parentCompiledData.keyNameToMemoryOffset)
                {
                    keyNameToMemoryOffset.Add(kvp.Key, kvp.Value);
                }

                foreach (var kvp in parentCompiledData.memoryOffsetToKeyData)
                {
                    memoryOffsetToKeyData.Add(kvp.Key, kvp.Value);
                }
            }

            // acquire key compiler context
            context.GenerateKeyCompilerContext(template, keyNameToMemoryOffset, memoryOffsetToKeyData, startingIndex, startingMemoryOffset);

            foreach (var key in m_Keys.OrderBy(k => k.sizeInBytes))
            {
                key.Compile(context.keyCompilerContext);
                context.UpdateKeyCompilerContext(key.sizeInBytes);
            }

            compiledData = new BlackboardTemplateCompiledData(parentCompiledData, template, keyNameToMemoryOffset, memoryOffsetToKeyData, totalKeyCount);
        }

        /// <summary>
        /// Free the compiled data.
        /// </summary>
        internal void Free()
        {
            foreach (var key in m_Keys) key.Free();
            compiledData = null;
        }
    }
}