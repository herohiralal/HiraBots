﻿using Unity.Mathematics;
using UnityEngine;

namespace HiraBots
{
    internal partial class BlackboardComponent
    {
        /// <summary>
        /// Get Boolean value from the blackboard using the key name.
        /// </summary>
        internal bool GetBooleanValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Boolean, out var data);
            return GetBooleanValueWithoutValidation(data.memoryOffset);
        }

        /// <summary>
        /// Get Enum value index from the blackboard using the key name.
        /// </summary>
        internal byte GetEnumValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Enum, out var data);
            return GetEnumValueWithoutValidation(data.memoryOffset);
        }

        /// <summary>
        /// Get Enum value from the blackboard using the key name.
        /// </summary>
        internal T GetEnumValue<T>(string key) where T : unmanaged, System.Enum
        {
            ValidateEnumType<T>();
            ValidateInput(key, BlackboardKeyType.Enum, out var data);
            return GetEnumValueWithoutValidation<T>(data.memoryOffset);
        }

        /// <summary>
        /// Get Float value from the blackboard using the key name.
        /// </summary>
        internal float GetFloatValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Float, out var data);
            return GetFloatValueWithoutValidation(data.memoryOffset);
        }

        /// <summary>
        /// Get Integer value from the blackboard using the key name.
        /// </summary>
        internal int GetIntegerValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Integer, out var data);
            return GetIntegerValueWithoutValidation(data.memoryOffset);
        }

        /// <summary>
        /// Get Object value from the blackboard using the key name.
        /// </summary>
        internal Object GetObjectValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Object, out var data);
            return GetObjectValueWithoutValidation(data.memoryOffset);
        }

        /// <summary>
        /// Get Vector value from the blackboard using the key name.
        /// </summary>
        internal float3 GetVectorValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Vector, out var data);
            return GetVectorValueWithoutValidation(data.memoryOffset);
        }

        /// <summary>
        /// Get Quaternion value from the blackboard using the key name.
        /// </summary>
        internal quaternion GetQuaternionValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Quaternion, out var data);
            return GetQuaternionValueWithoutValidation(data.memoryOffset);
        }
    }
}