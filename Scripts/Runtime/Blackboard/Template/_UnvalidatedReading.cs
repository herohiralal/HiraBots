using Unity.Mathematics;
using UnityEngine;

namespace HiraBots
{
    internal unsafe partial class BlackboardTemplateCompiledData
    {
        /// <summary>
        /// Get instance-synced Boolean value from blackboard template using memory offset without validating any input.
        /// </summary>
        internal bool GetInstanceSyncedBooleanValueWithoutValidation(ushort memoryOffset)
        {
            return BlackboardUnsafeHelpers.ReadBooleanValue(templateReadOnlyPtr, memoryOffset);
        }

        /// <summary>
        /// Get instance-synced Enum value from blackboard template using memory offset without validating any input.
        /// </summary>
        internal T GetInstanceSyncedEnumValueWithoutValidation<T>(ushort memoryOffset) where T : unmanaged, System.Enum
        {
            return BlackboardUnsafeHelpers.ReadEnumValue<T>(templateReadOnlyPtr, memoryOffset);
        }

        /// <summary>
        /// Get instance-synced Float value from blackboard template using memory offset without validating any input.
        /// </summary>
        internal float GetInstanceSyncedFloatValueWithoutValidation(ushort memoryOffset)
        {
            return BlackboardUnsafeHelpers.ReadFloatValue(templateReadOnlyPtr, memoryOffset);
        }

        /// <summary>
        /// Get instance-synced Integer value from blackboard template using memory offset without validating any input.
        /// </summary>
        internal int GetInstanceSyncedIntegerValueWithoutValidation(ushort memoryOffset)
        {
            return BlackboardUnsafeHelpers.ReadIntegerValue(templateReadOnlyPtr, memoryOffset);
        }

        /// <summary>
        /// Get instance-synced Object value from blackboard template using memory offset without validating any input.
        /// </summary>
        internal Object GetInstanceSyncedObjectValueWithoutValidation(ushort memoryOffset)
        {
            return BlackboardUnsafeHelpers.ReadObjectValue(templateReadOnlyPtr, memoryOffset);
        }

        /// <summary>
        /// Get instance-synced Vector value from blackboard template using memory offset without validating any input.
        /// </summary>
        internal float3 GetInstanceSyncedVectorValueWithoutValidation(ushort memoryOffset)
        {
            return BlackboardUnsafeHelpers.ReadVectorValue(templateReadOnlyPtr, memoryOffset);
        }

        /// <summary>
        /// Get instance-synced Quaternion value from blackboard template using memory offset without validating any input.
        /// </summary>
        internal quaternion GetInstanceSyncedQuaternionValueWithoutValidation(ushort memoryOffset)
        {
            return BlackboardUnsafeHelpers.ReadQuaternionValue(templateReadOnlyPtr, memoryOffset);
        }
    }
}