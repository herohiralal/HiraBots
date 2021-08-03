using UnityEngine;

namespace HiraBots
{
    internal unsafe partial class BlackboardComponent
    {
        /// <summary>
        /// Get Boolean value from blackboard using memory offset without validating any input.
        /// </summary>
        internal bool GetBooleanValueWithoutValidation(ushort memoryOffset)
        {
            return BlackboardUnsafeHelpers.ReadBooleanValue(dataReadOnlyPtr, memoryOffset);
        }

        /// <summary>
        /// Get Enum value from blackboard using memory offset without validating any input.
        /// </summary>
        internal T GetEnumValueWithoutValidation<T>(ushort memoryOffset) where T : unmanaged, System.Enum
        {
            return BlackboardUnsafeHelpers.ReadEnumValue<T>(dataReadOnlyPtr, memoryOffset);
        }

        /// <summary>
        /// Get Float value from blackboard using memory offset without validating any input.
        /// </summary>
        internal float GetFloatValueWithoutValidation(ushort memoryOffset)
        {
            return BlackboardUnsafeHelpers.ReadFloatValue(dataReadOnlyPtr, memoryOffset);
        }

        /// <summary>
        /// Get Integer value from blackboard using memory offset without validating any input.
        /// </summary>
        internal int GetIntegerValueWithoutValidation(ushort memoryOffset)
        {
            return BlackboardUnsafeHelpers.ReadIntegerValue(dataReadOnlyPtr, memoryOffset);
        }

        /// <summary>
        /// Get Object value from blackboard using memory offset without validating any input.
        /// </summary>
        internal Object GetObjectValueWithoutValidation(ushort memoryOffset)
        {
            return BlackboardUnsafeHelpers.ReadObjectValue(dataReadOnlyPtr, memoryOffset);
        }

        /// <summary>
        /// Get Vector value from blackboard using memory offset without validating any input.
        /// </summary>
        internal Vector3 GetVectorValueWithoutValidation(ushort memoryOffset)
        {
            return BlackboardUnsafeHelpers.ReadVectorValue(dataReadOnlyPtr, memoryOffset);
        }

        /// <summary>
        /// Get Quaternion value from blackboard using memory offset without validating any input.
        /// </summary>
        internal Quaternion GetQuaternionValueWithoutValidation(ushort memoryOffset)
        {
            return BlackboardUnsafeHelpers.ReadQuaternionValue(dataReadOnlyPtr, memoryOffset);
        }
    }
}