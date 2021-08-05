using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// Data compiled from a blackboard template.
    /// </summary>
    internal unsafe partial class BlackboardTemplateCompiledData
    {
        private BlackboardTemplateCompiledData m_ParentCompiledData;
        private NativeArray<byte> m_Template = default;
        private readonly Dictionary<string, ushort> m_KeyNameToMemoryOffset;
        private readonly Dictionary<ushort, BlackboardKeyCompiledData> m_MemoryOffsetToKeyData;

        /// <summary>
        /// The number of keys within the blackboard template.
        /// </summary>
        internal ushort keyCount { get; }

        internal BlackboardTemplateCompiledData(BlackboardTemplateCompiledData parentCompiledData, NativeArray<byte> template,
            Dictionary<string, ushort> keyNameToMemoryOffset, Dictionary<ushort, BlackboardKeyCompiledData> memoryOffsetToKeyData, ushort keyCount)
        {
            m_ParentCompiledData = parentCompiledData;
            m_ParentCompiledData?.AddInstanceSyncListener(this);
            m_Template = template;
            m_KeyNameToMemoryOffset = keyNameToMemoryOffset;
            m_MemoryOffsetToKeyData = memoryOffsetToKeyData;
            this.keyCount = keyCount;
        }

        ~BlackboardTemplateCompiledData()
        {
            m_ParentCompiledData?.RemoveInstanceSyncListener(this);
            m_ParentCompiledData = null;

            if (m_Template.IsCreated)
                m_Template.Dispose();

            m_KeyNameToMemoryOffset.Clear();
            m_MemoryOffsetToKeyData.Clear();
        }

        // pointers to the template
        private byte* templatePtr => (byte*) m_Template.GetUnsafePtr();
        private byte* templateReadOnlyPtr => (byte*) m_Template.GetUnsafeReadOnlyPtr();

        /// <summary>
        /// Copy template to a different NativeArray.
        /// </summary>
        internal void CopyTemplateTo(NativeArray<byte> otherTemplate)
        {
            UnsafeUtility.MemCpy(otherTemplate.GetUnsafePtr(), templateReadOnlyPtr, templateSize);
        }

        /// <summary>
        /// Map from key name to memory offset.
        /// </summary>
        internal ReadOnlyDictionaryAccessor<string, ushort> keyNameToMemoryOffset =>
            m_KeyNameToMemoryOffset;

        /// <summary>
        /// Map from memory offset to key compiled data.
        /// </summary>
        internal ReadOnlyDictionaryAccessor<ushort, BlackboardKeyCompiledData> memoryOffsetToKeyData =>
            m_MemoryOffsetToKeyData;

        /// <summary>
        /// Get key compiled data from a key name.
        /// </summary>
        internal BlackboardKeyCompiledData this[string keyName] =>
            m_KeyNameToMemoryOffset.TryGetValue(keyName, out var memoryOffset)
            && m_MemoryOffsetToKeyData.TryGetValue(memoryOffset, out var output)
                ? output
                : BlackboardKeyCompiledData.none;

        /// <summary>
        /// The size of the template in bytes.
        /// </summary>
        internal ushort templateSize => (ushort) m_Template.Length;
    }
}