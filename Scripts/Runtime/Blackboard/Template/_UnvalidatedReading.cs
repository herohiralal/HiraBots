using UnityEngine;

namespace HiraBots
{
    internal unsafe partial class BlackboardTemplateCompiledData
    {
        internal bool GetInstanceSyncedBooleanValueWithoutValidation(ushort memoryOffset) =>
            BlackboardUnsafeHelpers.ReadBooleanValue(TemplateReadOnlyPtr, memoryOffset);

        internal T GetInstanceSyncedEnumValueWithoutValidation<T>(ushort memoryOffset) where T : unmanaged, System.Enum =>
            BlackboardUnsafeHelpers.ReadEnumValue<T>(TemplateReadOnlyPtr, memoryOffset);

        internal float GetInstanceSyncedFloatValueWithoutValidation(ushort memoryOffset) =>
            BlackboardUnsafeHelpers.ReadFloatValue(TemplateReadOnlyPtr, memoryOffset);

        internal int GetInstanceSyncedIntegerValueWithoutValidation(ushort memoryOffset) =>
            BlackboardUnsafeHelpers.ReadIntegerValue(TemplateReadOnlyPtr, memoryOffset);

        internal Object GetInstanceSyncedObjectValueWithoutValidation(ushort memoryOffset) =>
            BlackboardUnsafeHelpers.ReadObjectValue(TemplateReadOnlyPtr, memoryOffset);

        internal Vector3 GetInstanceSyncedVectorValueWithoutValidation(ushort memoryOffset) =>
            BlackboardUnsafeHelpers.ReadVectorValue(TemplateReadOnlyPtr, memoryOffset);

        internal Quaternion GetInstanceSyncedQuaternionValueWithoutValidation(ushort memoryOffset) =>
            BlackboardUnsafeHelpers.ReadQuaternionValue(TemplateReadOnlyPtr, memoryOffset);
    }
}