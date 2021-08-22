using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace HiraBots
{
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
        internal unsafe void Compile()
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

            var template = new NativeArray<byte>(UnsafeHelpers.GetAlignedSize(totalTemplateSize),
                Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            var keyNameToKeyData = new Dictionary<string, BlackboardKeyCompiledData>(totalKeyCount);

            // update the created collections with the data from parent blackboard template
            if (parentCompiledData != null)
            {
                parentCompiledData.CopyTemplateTo(template);

                foreach (var kvp in parentCompiledData.keyNameToKeyData)
                {
                    keyNameToKeyData.Add(kvp.Key, kvp.Value);
                }
            }

            var templateAddress = (byte*) template.GetUnsafePtr();
            var memoryOffset = startingMemoryOffset;
            var index = startingIndex;

            var keyCompilerContext = new BlackboardKeyCompilerContext();

            foreach (var key in m_Keys.OrderBy(k => k.sizeInBytes))
            {
                keyCompilerContext.address = templateAddress + memoryOffset;
                keyCompilerContext.index = index;
                keyCompilerContext.memoryOffset = memoryOffset;

                key.Compile(ref keyCompilerContext);

                keyNameToKeyData.Add(key.name, key.compiledData);

                memoryOffset += key.sizeInBytes;
                index++;
            }

            compiledData = new BlackboardTemplateCompiledData(parentCompiledData,
                template, keyNameToKeyData, totalKeyCount);
        }

        /// <summary>
        /// Free the compiled data.
        /// </summary>
        internal void Free()
        {
            foreach (var key in m_Keys)
            {
                key.Free();
            }

            compiledData = null;
        }
    }
}