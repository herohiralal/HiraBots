using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HiraBots
{
    internal unsafe partial class BlackboardComponent
    {
        internal bool GetBooleanValueWithoutValidation(ushort memoryOffset) =>
            BlackboardUnsafeHelpers.ReadBooleanValue(DataReadOnlyPtr, memoryOffset);

        internal T GetEnumValueWithoutValidation<T>(ushort memoryOffset) where T : unmanaged, Enum =>
            BlackboardUnsafeHelpers.ReadEnumValue<T>(DataReadOnlyPtr, memoryOffset);

        internal float GetFloatValueWithoutValidation(ushort memoryOffset) =>
            BlackboardUnsafeHelpers.ReadFloatValue(DataReadOnlyPtr, memoryOffset);

        internal int GetIntegerValueWithoutValidation(ushort memoryOffset) =>
            BlackboardUnsafeHelpers.ReadIntegerValue(DataReadOnlyPtr, memoryOffset);

        internal Object GetObjectValueWithoutValidation(ushort memoryOffset) =>
            BlackboardUnsafeHelpers.ReadObjectValue(DataReadOnlyPtr, memoryOffset);

        internal Vector3 GetVectorValueWithoutValidation(ushort memoryOffset) =>
            BlackboardUnsafeHelpers.ReadVectorValue(DataReadOnlyPtr, memoryOffset);

        internal Quaternion GetQuaternionValueWithoutValidation(ushort memoryOffset) =>
            BlackboardUnsafeHelpers.ReadQuaternionValue(DataReadOnlyPtr, memoryOffset);
    }
}