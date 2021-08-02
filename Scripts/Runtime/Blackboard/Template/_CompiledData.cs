using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace HiraBots
{
    internal readonly struct BlackboardKeyCompiledData
    {
        internal BlackboardKeyCompiledData(ushort memoryOffset, ushort index, BlackboardKeyTraits traits, BlackboardKeyType keyType)
        {
            m_MemoryOffset = memoryOffset;
            m_Index = index;
            m_Traits = traits;
            m_KeyType = keyType;
        }

        internal static BlackboardKeyCompiledData none
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new BlackboardKeyCompiledData(ushort.MaxValue, ushort.MaxValue, BlackboardKeyTraits.None, BlackboardKeyType.Invalid);
        }

        internal bool isValid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_KeyType != BlackboardKeyType.Invalid;
        }

        internal readonly ushort m_MemoryOffset;
        internal readonly ushort m_Index;
        internal readonly BlackboardKeyTraits m_Traits;
        internal readonly BlackboardKeyType m_KeyType;

        internal bool instanceSynced
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (m_Traits & BlackboardKeyTraits.InstanceSynced) != 0;
        }

        internal bool broadcastEventOnUnexpectedChange
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (m_Traits & BlackboardKeyTraits.BroadcastEventOnUnexpectedChange) != 0;
        }
    }

    internal unsafe partial class BlackboardTemplateCompiledData
    {
        private BlackboardTemplateCompiledData m_ParentCompiledData;
        private NativeArray<byte> m_Template = default;
        private readonly Dictionary<string, ushort> m_KeyNameToMemoryOffset;
        private readonly Dictionary<ushort, BlackboardKeyCompiledData> m_MemoryOffsetToKeyData;
        internal readonly ushort m_KeyCount;

        internal BlackboardTemplateCompiledData(BlackboardTemplateCompiledData parentCompiledData, NativeArray<byte> template,
            Dictionary<string, ushort> keyNameToMemoryOffset, Dictionary<ushort, BlackboardKeyCompiledData> memoryOffsetToKeyData, ushort keyCount)
        {
            m_ParentCompiledData = parentCompiledData;
            m_ParentCompiledData?.AddInstanceSyncListener(this);
            m_Template = template;
            m_KeyNameToMemoryOffset = keyNameToMemoryOffset;
            m_MemoryOffsetToKeyData = memoryOffsetToKeyData;
            m_KeyCount = keyCount;
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

        private byte* templatePtr => (byte*) m_Template.GetUnsafePtr();
        private byte* templateReadOnlyPtr => (byte*) m_Template.GetUnsafeReadOnlyPtr();

        internal void CopyTemplateTo(NativeArray<byte> otherTemplate) =>
            UnsafeUtility.MemCpy(otherTemplate.GetUnsafePtr(), templateReadOnlyPtr, templateSize);

        internal ReadOnlyDictionaryAccessor<string, ushort> keyNameToMemoryOffset =>
            m_KeyNameToMemoryOffset;

        internal ReadOnlyDictionaryAccessor<ushort, BlackboardKeyCompiledData> memoryOffsetToKeyData =>
            m_MemoryOffsetToKeyData;

        internal BlackboardKeyCompiledData this[string keyName] =>
            m_KeyNameToMemoryOffset.TryGetValue(keyName, out var memoryOffset)
            && m_MemoryOffsetToKeyData.TryGetValue(memoryOffset, out var output)
                ? output
                : BlackboardKeyCompiledData.none;

        internal ushort templateSize => (ushort) m_Template.Length;
    }
}