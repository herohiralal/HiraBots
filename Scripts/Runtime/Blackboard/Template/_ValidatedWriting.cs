﻿using Unity.Mathematics;
using UnityEngine;

namespace HiraBots
{
    internal partial class BlackboardTemplateCompiledData
    {
        /// <summary>
        /// Set instance-synced Boolean value on the blackboard template using the key name.
        /// </summary>
        internal void SetInstanceSyncedBooleanValue(string key, bool value)
        {
            ValidateInput(key, BlackboardKeyType.Boolean, out var data);
            SetInstanceSyncedBooleanValueWithoutValidation(in data, value);
        }

        /// <summary>
        /// Set instance-synced Enum value index on the blackboard template using the key name.
        /// </summary>
        internal void SetInstanceSyncedEnumValue(string key, byte value)
        {
            ValidateInput(key, BlackboardKeyType.Enum, out var data);
            SetInstanceSyncedEnumValueWithoutValidation(in data, value);
        }

        /// <summary>
        /// Set instance-synced Enum value on the blackboard template using the key name.
        /// </summary>
        internal void SetInstanceSyncedEnumValue<T>(string key, T value) where T : unmanaged, System.Enum
        {
            ValidateEnumType<T>();
            ValidateInput(key, BlackboardKeyType.Enum, out var data);
            SetInstanceSyncedEnumValueWithoutValidation<T>(in data, value);
        }

        /// <summary>
        /// Set instance-synced Float value on the blackboard template using the key name.
        /// </summary>
        internal void SetInstanceSyncedFloatValue(string key, float value)
        {
            ValidateInput(key, BlackboardKeyType.Float, out var data);
            SetInstanceSyncedFloatValueWithoutValidation(in data, value);
        }

        /// <summary>
        /// Set instance-synced Integer value on the blackboard template using the key name.
        /// </summary>
        internal void SetInstanceSyncedIntegerValue(string key, int value)
        {
            ValidateInput(key, BlackboardKeyType.Integer, out var data);
            SetInstanceSyncedIntegerValueWithoutValidation(in data, value);
        }

        /// <summary>
        /// Set instance-synced Object value on the blackboard template using the key name.
        /// </summary>
        internal void SetInstanceSyncedObjectValue(string key, Object value)
        {
            ValidateInput(key, BlackboardKeyType.Object, out var data);
            SetInstanceSyncedObjectValueWithoutValidation(in data, value);
        }

        /// <summary>
        /// Set instance-synced Vector value on the blackboard template using the key name.
        /// </summary>
        internal void SetInstanceSyncedVectorValue(string key, float3 value)
        {
            ValidateInput(key, BlackboardKeyType.Vector, out var data);
            SetInstanceSyncedVectorValueWithoutValidation(in data, value);
        }

        /// <summary>
        /// Set instance-synced Quaternion value on the blackboard template using the key name.
        /// </summary>
        internal void SetInstanceSyncedQuaternionValue(string key, quaternion value)
        {
            ValidateInput(key, BlackboardKeyType.Quaternion, out var data);
            SetInstanceSyncedQuaternionValueWithoutValidation(in data, value);
        }
    }
}