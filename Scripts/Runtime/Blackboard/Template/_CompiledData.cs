using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace HiraBots
{
    internal class BlackboardKeyCompiledData
    {
        internal BlackboardKeyCompiledData(ushort memoryOffset, ushort index, BlackboardKeyTraits traits, BlackboardKeyType keyType)
        {
            MemoryOffset = memoryOffset;
            Index = index;
            Traits = traits;
            KeyType = keyType;
        }

        internal readonly ushort MemoryOffset;
        internal readonly ushort Index;
        internal readonly BlackboardKeyTraits Traits;
        internal readonly BlackboardKeyType KeyType;

        internal bool InstanceSynced => (Traits & BlackboardKeyTraits.InstanceSynced) != 0;
        internal bool BroadcastEventOnUnexpectedChange => (Traits & BlackboardKeyTraits.BroadcastEventOnUnexpectedChange) != 0;
    }

    internal unsafe partial class BlackboardTemplateCompiledData
    {
        private BlackboardTemplateCompiledData _parentCompiledData;
        private NativeArray<byte> _template = default;
        private readonly Dictionary<string, ushort> _keyNameToMemoryOffset;
        private readonly Dictionary<ushort, BlackboardKeyCompiledData> _memoryOffsetToKeyData;
        internal readonly ushort KeyCount;

        internal BlackboardTemplateCompiledData(BlackboardTemplateCompiledData parentCompiledData, NativeArray<byte> template,
            Dictionary<string, ushort> keyNameToMemoryOffset, Dictionary<ushort, BlackboardKeyCompiledData> memoryOffsetToKeyData, ushort keyCount)
        {
            _parentCompiledData = parentCompiledData;
            _parentCompiledData?.AddInstanceSyncListener(this);
            _template = template;
            _keyNameToMemoryOffset = keyNameToMemoryOffset;
            _memoryOffsetToKeyData = memoryOffsetToKeyData;
            KeyCount = keyCount;
        }

        ~BlackboardTemplateCompiledData()
        {
            _parentCompiledData?.RemoveInstanceSyncListener(this);
            _parentCompiledData = null;

            if (_template.IsCreated)
                _template.Dispose();

            _keyNameToMemoryOffset.Clear();
            _memoryOffsetToKeyData.Clear();
        }

        private byte* TemplatePtr => (byte*) _template.GetUnsafePtr();
        private byte* TemplateReadOnlyPtr => (byte*) _template.GetUnsafeReadOnlyPtr();

        internal void CopyTemplateTo(NativeArray<byte> otherTemplate) =>
            UnsafeUtility.MemCpy(otherTemplate.GetUnsafePtr(), TemplateReadOnlyPtr, TemplateSize);

        internal ReadOnlyDictionaryAccessor<string, ushort> KeyNameToMemoryOffset =>
            _keyNameToMemoryOffset;

        internal ReadOnlyDictionaryAccessor<ushort, BlackboardKeyCompiledData> MemoryOffsetToKeyData =>
            _memoryOffsetToKeyData;

        internal BlackboardKeyCompiledData this[string keyName] =>
            _keyNameToMemoryOffset.TryGetValue(keyName, out var memoryOffset)
            && _memoryOffsetToKeyData.TryGetValue(memoryOffset, out var output)
                ? output
                : null;

        internal ushort TemplateSize => (ushort) _template.Length;
    }
}