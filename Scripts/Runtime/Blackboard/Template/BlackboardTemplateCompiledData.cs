﻿using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace HiraBots
{
    internal class BlackboardTemplateCompiledData
    {
        private NativeArray<byte> _template = default;
        private Dictionary<string, ushort> _keyNameToIndex;
        private BlackboardKeyCompiledData[] _keyData;

        internal BlackboardTemplateCompiledData(NativeArray<byte> template, Dictionary<string, ushort> keyNameToIndex, BlackboardKeyCompiledData[] keyData)
        {
            _template = template;
            _keyNameToIndex = keyNameToIndex;
            _keyData = keyData;
        }

        ~BlackboardTemplateCompiledData()
        {
            if (_template.IsCreated)
                _template.Dispose();

            _keyNameToIndex.Clear();
            _keyNameToIndex = null;
            _keyData = null;
        }

        internal unsafe void CopyTemplateTo(NativeArray<byte> otherTemplate) =>
            UnsafeUtility.MemCpy(otherTemplate.GetUnsafePtr(), _template.GetUnsafeReadOnlyPtr(), TemplateSize);

        internal ReadOnlyDictionaryAccessor<string, ushort> KeyNameToIndex => _keyNameToIndex;

        internal ReadOnlyArrayAccessor<BlackboardKeyCompiledData> KeyData => _keyData;

        internal ushort TemplateSize => (ushort) _template.Length;

        internal ushort KeyCount => (ushort) _keyData.Length;
    }
}