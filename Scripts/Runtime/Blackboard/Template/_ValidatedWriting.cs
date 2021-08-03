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
        /// Set instance-synced Boolean value on the blackboard template using the memory offset.
        /// </summary>
        internal void SetInstanceSyncedBooleanValue(ushort key, bool value)
        {
            ValidateInput(key, BlackboardKeyType.Boolean, out var data);
            SetInstanceSyncedBooleanValueWithoutValidation(in data, value);
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
        /// Set instance-synced Enum value on the blackboard template using the memory offset.
        /// </summary>
        internal void SetInstanceSyncedEnumValue<T>(ushort key, T value) where T : unmanaged, System.Enum
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
        /// Set instance-synced Float value on the blackboard template using the memory offset.
        /// </summary>
        internal void SetInstanceSyncedFloatValue(ushort key, float value)
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
        /// Set instance-synced Integer value on the blackboard template using the memory offset.
        /// </summary>
        internal void SetInstanceSyncedIntegerValue(ushort key, int value)
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
        /// Set instance-synced Object value on the blackboard template using the memory offset.
        /// </summary>
        internal void SetInstanceSyncedObjectValue(ushort key, Object value)
        {
            ValidateInput(key, BlackboardKeyType.Object, out var data);
            SetInstanceSyncedObjectValueWithoutValidation(in data, value);
        }

        /// <summary>
        /// Set instance-synced Vector value on the blackboard template using the key name.
        /// </summary>
        internal void SetInstanceSyncedVectorValue(string key, Vector3 value)
        {
            ValidateInput(key, BlackboardKeyType.Vector, out var data);
            SetInstanceSyncedVectorValueWithoutValidation(in data, value);
        }

        /// <summary>
        /// Set instance-synced Vector value on the blackboard template using the memory offset.
        /// </summary>
        internal void SetInstanceSyncedVectorValue(ushort key, Vector3 value)
        {
            ValidateInput(key, BlackboardKeyType.Vector, out var data);
            SetInstanceSyncedVectorValueWithoutValidation(in data, value);
        }

        /// <summary>
        /// Set instance-synced Quaternion value on the blackboard template using the key name.
        /// </summary>
        internal void SetInstanceSyncedQuaternionValue(string key, Quaternion value)
        {
            ValidateInput(key, BlackboardKeyType.Quaternion, out var data);
            SetInstanceSyncedQuaternionValueWithoutValidation(in data, value);
        }

        /// <summary>
        /// Set instance-synced Quaternion value on the blackboard template using the memory offset.
        /// </summary>
        internal void SetInstanceSyncedQuaternionValue(ushort key, Quaternion value)
        {
            ValidateInput(key, BlackboardKeyType.Quaternion, out var data);
            SetInstanceSyncedQuaternionValueWithoutValidation(in data, value);
        }
    }
}