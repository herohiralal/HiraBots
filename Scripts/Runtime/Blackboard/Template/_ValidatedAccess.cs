using System;
using System.Collections.Generic;

namespace HiraBots
{
    internal partial class BlackboardTemplateCompiledData
    {
        // validate the input when provided with a key name
        private void ValidateInput(string keyName, BlackboardKeyType keyType, out BlackboardKeyCompiledData data)
        {
            data = this[keyName];

            if (!data.isValid)
            {
                throw new KeyNotFoundException($"Invalid key name: {keyName}");
            }

            if (data.keyType != keyType)
            {
                throw new InvalidCastException($"Type mismatch: {keyName}. Requested - {keyType}. Actual - {data.keyType}");
            }

            if (!data.instanceSynced)
            {
                throw new InvalidOperationException($"Instanced value requested without an instance: {keyName}.");
            }
        }

        // validate the input when provided with a memory offset
        private void ValidateInput(ushort memoryOffset, BlackboardKeyType keyType, out BlackboardKeyCompiledData data)
        {
            if (!m_MemoryOffsetToKeyData.TryGetValue(memoryOffset, out data))
            {
                throw new KeyNotFoundException($"Invalid memory offset: {memoryOffset}.");
            }

            if (data.keyType != keyType)
            {
                throw new InvalidCastException($"Type mismatch: {memoryOffset}. Requested - {keyType}. Actual - {data.keyType}");
            }

            if (!data.instanceSynced)
            {
                throw new InvalidOperationException($"Instanced value requested without an instance: {memoryOffset}.");
            }
        }

        // validate the enum type
        private static unsafe void ValidateEnumType<T>() where T : unmanaged, Enum
        {
            if (sizeof(T) != 1)
            {
                throw new OverflowException($"Invalid enum type: {typeof(T).FullName}. Only 8-bit enums are allowed.");
            }
        }
    }
}