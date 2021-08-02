using UnityEngine;

namespace HiraBots
{
    internal unsafe partial class BlackboardComponent
    {
        internal bool GetBooleanValueWithoutValidation(ushort memoryOffset) =>
            BlackboardUnsafeHelpers.ReadBooleanValue(dataReadOnlyPtr, memoryOffset);

        internal T GetEnumValueWithoutValidation<T>(ushort memoryOffset) where T : unmanaged, System.Enum =>
            BlackboardUnsafeHelpers.ReadEnumValue<T>(dataReadOnlyPtr, memoryOffset);

        internal float GetFloatValueWithoutValidation(ushort memoryOffset) =>
            BlackboardUnsafeHelpers.ReadFloatValue(dataReadOnlyPtr, memoryOffset);

        internal int GetIntegerValueWithoutValidation(ushort memoryOffset) =>
            BlackboardUnsafeHelpers.ReadIntegerValue(dataReadOnlyPtr, memoryOffset);

        internal Object GetObjectValueWithoutValidation(ushort memoryOffset) =>
            BlackboardUnsafeHelpers.ReadObjectValue(dataReadOnlyPtr, memoryOffset);

        internal Vector3 GetVectorValueWithoutValidation(ushort memoryOffset) =>
            BlackboardUnsafeHelpers.ReadVectorValue(dataReadOnlyPtr, memoryOffset);

        internal Quaternion GetQuaternionValueWithoutValidation(ushort memoryOffset) =>
            BlackboardUnsafeHelpers.ReadQuaternionValue(dataReadOnlyPtr, memoryOffset);
    }
}