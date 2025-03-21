﻿using System;
using System.Collections.Generic;

namespace HiraBots
{
    internal partial class BlackboardComponent
    {
        // validate the input when provided with a key name
        private void ValidateInput(string keyName, BlackboardKeyType keyType, out BlackboardKeyCompiledData data)
        {
            data = m_Template[keyName];

            if (!data.isValid)
            {
                throw new KeyNotFoundException($"Invalid key name: {keyName}");
            }

            if (data.keyType != keyType)
            {
                throw new InvalidCastException($"Type mismatch: {keyName}. Requested - {keyType}. Actual - {data.keyType}");
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