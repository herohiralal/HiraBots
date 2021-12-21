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
        private readonly Dictionary<string, BlackboardKeyCompiledData> m_KeyNameToKeyData;

        private readonly HashSet<NativeArray<byte>> m_BlackboardComponents = new HashSet<NativeArray<byte>>();

        /// <summary>
        /// The number of keys within the blackboard template.
        /// </summary>
        internal ushort keyCount { get; }

        internal BlackboardTemplateCompiledData(BlackboardTemplateCompiledData parentCompiledData, NativeArray<byte> template,
            Dictionary<string, BlackboardKeyCompiledData> keyNameToKeyData, ushort keyCount)
        {
            m_ParentCompiledData = parentCompiledData;
            m_ParentCompiledData?.AddInstanceSyncListener(this);
            m_Template = template;
            m_KeyNameToKeyData = keyNameToKeyData;
            this.keyCount = keyCount;
        }

        internal void Dispose()
        {
            m_Listeners.Clear();

            foreach (var blackboard in m_BlackboardComponents)
            {
                blackboard.Dispose();
            }

            m_BlackboardComponents.Clear();

            m_ParentCompiledData?.RemoveInstanceSyncListener(this);
            m_ParentCompiledData = null;

            if (m_Template.IsCreated)
                m_Template.Dispose();

            m_KeyNameToKeyData.Clear();
        }

        // pointers to the template
        private byte* templatePtr => (byte*) m_Template.GetUnsafePtr();
        private byte* templateReadOnlyPtr => (byte*) m_Template.GetUnsafeReadOnlyPtr();

        /// <summary>
        /// Initialize a blackboard component based on this template.
        /// </summary>
        internal void InitializeBlackboardComponent(out NativeArray<byte> blackboard)
        {
            blackboard = new NativeArray<byte>(m_Template, Allocator.Persistent);
            m_BlackboardComponents.Add(blackboard);
        }

        /// <summary>
        /// Dispose a blackboard component based on this template.
        /// </summary>
        internal void DisposeBlackboardComponent(NativeArray<byte> blackboard)
        {
            if (m_BlackboardComponents.Remove(blackboard))
            {
                blackboard.Dispose();
            }
        }

        /// <summary>
        /// Copy template to a different NativeArray.
        /// </summary>
        internal void CopyTemplateTo(NativeArray<byte> otherTemplate)
        {
            UnsafeUtility.MemCpy(otherTemplate.GetUnsafePtr(), templateReadOnlyPtr, templateSize);
        }

        /// <summary>
        /// Map from memory offset to key compiled data.
        /// </summary>
        internal ReadOnlyDictionaryAccessor<string, BlackboardKeyCompiledData> keyNameToKeyData =>
            m_KeyNameToKeyData;

        /// <summary>
        /// Get key compiled data from a key name.
        /// </summary>
        internal BlackboardKeyCompiledData this[string keyName] =>
            m_KeyNameToKeyData.TryGetValue(keyName, out var output)
                ? output
                : BlackboardKeyCompiledData.none;

        /// <summary>
        /// The size of the template in bytes.
        /// </summary>
        internal ushort templateSize => (ushort) m_Template.Length;
    }
}