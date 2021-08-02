using UnityEngine;

namespace HiraBots
{
    internal unsafe partial class BlackboardTemplateCompiledData
    {
        internal bool GetInstanceSyncedBooleanValueWithoutValidation(ushort memoryOffset) =>
            BlackboardUnsafeHelpers.ReadBooleanValue(templateReadOnlyPtr, memoryOffset);

        internal T GetInstanceSyncedEnumValueWithoutValidation<T>(ushort memoryOffset) where T : unmanaged, System.Enum =>
            BlackboardUnsafeHelpers.ReadEnumValue<T>(templateReadOnlyPtr, memoryOffset);

        internal float GetInstanceSyncedFloatValueWithoutValidation(ushort memoryOffset) =>
            BlackboardUnsafeHelpers.ReadFloatValue(templateReadOnlyPtr, memoryOffset);

        internal int GetInstanceSyncedIntegerValueWithoutValidation(ushort memoryOffset) =>
            BlackboardUnsafeHelpers.ReadIntegerValue(templateReadOnlyPtr, memoryOffset);

        internal Object GetInstanceSyncedObjectValueWithoutValidation(ushort memoryOffset) =>
            BlackboardUnsafeHelpers.ReadObjectValue(templateReadOnlyPtr, memoryOffset);

        internal Vector3 GetInstanceSyncedVectorValueWithoutValidation(ushort memoryOffset) =>
            BlackboardUnsafeHelpers.ReadVectorValue(templateReadOnlyPtr, memoryOffset);

        internal Quaternion GetInstanceSyncedQuaternionValueWithoutValidation(ushort memoryOffset) =>
            BlackboardUnsafeHelpers.ReadQuaternionValue(templateReadOnlyPtr, memoryOffset);
    }
}