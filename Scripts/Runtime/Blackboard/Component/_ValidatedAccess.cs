using System;
using System.Collections.Generic;

namespace HiraBots
{
    internal partial class BlackboardComponent
    {
        private void ValidateInput(string keyName, BlackboardKeyType keyType, out BlackboardKeyCompiledData data)
        {
            data = m_Template[keyName];

            if (!data.isValid)
            {
                throw new KeyNotFoundException($"Invalid key name: {keyName}");
            }

            if (data.m_KeyType != keyType)
            {
                throw new InvalidCastException($"Type mismatch: {keyName}. Requested - {keyType}. Actual - {data.m_KeyType}");
            }
        }

        private void ValidateInput(ushort memoryOffset, BlackboardKeyType keyType, out BlackboardKeyCompiledData data)
        {
            if (!m_Template.memoryOffsetToKeyData.TryGetValue(memoryOffset, out data))
            {
                throw new KeyNotFoundException($"Invalid memory offset: {memoryOffset}");
            }

            if (data.m_KeyType != keyType)
            {
                throw new InvalidCastException($"Type mismatch: {memoryOffset}. Requested - {keyType}. Actual - {data.m_KeyType}");
            }
        }

        private static unsafe void ValidateEnumType<T>() where T : unmanaged, Enum
        {
            if (sizeof(T) != 1)
            {
                throw new OverflowException($"Invalid enum type: {typeof(T).FullName}. Only 8-bit enums are allowed.");
            }
        }
    }
}