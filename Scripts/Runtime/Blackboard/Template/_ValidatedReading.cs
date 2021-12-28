using Unity.Mathematics;
using UnityEngine;

namespace HiraBots
{
    internal partial class BlackboardTemplateCompiledData
    {
        /// <summary>
        /// Get instance-synced Boolean value from the blackboard template using the key name.
        /// </summary>
        internal bool GetInstanceSyncedBooleanValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Boolean, out var data);
            return GetInstanceSyncedBooleanValueWithoutValidation(data.memoryOffset);
        }

        /// <summary>
        /// Get instance-synced Enum value index from the blackboard template using the key name.
        /// </summary>
        internal byte GetInstanceSyncedEnumValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Enum, out var data);
            return GetInstanceSyncedEnumValueWithoutValidation(data.memoryOffset);
        }

        /// <summary>
        /// Get instance-synced Enum value from the blackboard template using the key name.
        /// </summary>
        internal T GetInstanceSyncedEnumValue<T>(string key) where T : unmanaged, System.Enum
        {
            ValidateEnumType<T>();
            ValidateInput(key, BlackboardKeyType.Enum, out var data);
            return GetInstanceSyncedEnumValueWithoutValidation<T>(data.memoryOffset);
        }

        /// <summary>
        /// Get instance-synced Float value from the blackboard template using the key name.
        /// </summary>
        internal float GetInstanceSyncedFloatValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Float, out var data);
            return GetInstanceSyncedFloatValueWithoutValidation(data.memoryOffset);
        }

        /// <summary>
        /// Get instance-synced Integer value from the blackboard template using the key name.
        /// </summary>
        internal int GetInstanceSyncedIntegerValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Integer, out var data);
            return GetInstanceSyncedIntegerValueWithoutValidation(data.memoryOffset);
        }

        /// <summary>
        /// Get instance-synced Object value from the blackboard template using the key name.
        /// </summary>
        internal Object GetInstanceSyncedObjectValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Object, out var data);
            return GetInstanceSyncedObjectValueWithoutValidation(data.memoryOffset);
        }

        /// <summary>
        /// Get instance-synced Vector value from the blackboard template using the key name.
        /// </summary>
        internal float3 GetInstanceSyncedVectorValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Vector, out var data);
            return GetInstanceSyncedVectorValueWithoutValidation(data.memoryOffset);
        }

        /// <summary>
        /// Get instance-synced Quaternion value from the blackboard template using the key name.
        /// </summary>
        internal quaternion GetInstanceSyncedQuaternionValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Quaternion, out var data);
            return GetInstanceSyncedQuaternionValueWithoutValidation(data.memoryOffset);
        }
    }
}