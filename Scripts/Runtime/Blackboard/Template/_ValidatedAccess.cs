using System;
using System.Collections.Generic;

namespace HiraBots
{
    internal partial class BlackboardTemplateCompiledData
    {
        private void ValidateInput(string keyName, BlackboardKeyType keyType, out BlackboardKeyCompiledData data)
        {
            data = this[keyName];

            if (!data.IsValid)
            {
                throw new KeyNotFoundException($"Invalid key name: {keyName}");
            }

            if (data.KeyType != keyType)
            {
                throw new InvalidCastException($"Type mismatch: {keyName}. Requested - {keyType}. Actual - {data.KeyType}");
            }

            if (!data.InstanceSynced)
            {
                throw new InvalidOperationException($"Instanced value requested without an instance: {keyName}.");
            }
        }

        private void ValidateInput(ushort memoryOffset, BlackboardKeyType keyType, out BlackboardKeyCompiledData data)
        {
            if (!_memoryOffsetToKeyData.TryGetValue(memoryOffset, out data))
            {
                throw new KeyNotFoundException($"Invalid memory offset: {memoryOffset}.");
            }

            if (data.KeyType != keyType)
            {
                throw new InvalidCastException($"Type mismatch: {memoryOffset}. Requested - {keyType}. Actual - {data.KeyType}");
            }

            if (!data.InstanceSynced)
            {
                throw new InvalidOperationException($"Instanced value requested without an instance: {memoryOffset}.");
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