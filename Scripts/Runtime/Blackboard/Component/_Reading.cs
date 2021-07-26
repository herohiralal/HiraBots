using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HiraBots
{
    internal unsafe partial class BlackboardComponent
    {
        internal bool GetBooleanValueWithoutValidation(ushort keyIndex) =>
            BlackboardUnsafeHelpers.ReadBooleanValue(DataReadOnlyPtr, _template.KeyData[keyIndex].MemoryOffset);

        internal T GetEnumValueWithoutValidation<T>(ushort keyIndex) where T : unmanaged, Enum =>
            BlackboardUnsafeHelpers.ReadEnumValue<T>(DataReadOnlyPtr, _template.KeyData[keyIndex].MemoryOffset);

        internal float GetFloatValueWithoutValidation(ushort keyIndex) =>
            BlackboardUnsafeHelpers.ReadFloatValue(DataReadOnlyPtr, _template.KeyData[keyIndex].MemoryOffset);

        internal int GetIntegerValueWithoutValidation(ushort keyIndex) =>
            BlackboardUnsafeHelpers.ReadIntegerValue(DataReadOnlyPtr, _template.KeyData[keyIndex].MemoryOffset);

        internal Object GetObjectValueWithoutValidation(ushort keyIndex) =>
            BlackboardUnsafeHelpers.ReadObjectValue(DataReadOnlyPtr, _template.KeyData[keyIndex].MemoryOffset);

        internal Vector3 GetVectorValueWithoutValidation(ushort keyIndex) =>
            BlackboardUnsafeHelpers.ReadVectorValue(DataReadOnlyPtr, _template.KeyData[keyIndex].MemoryOffset);

        internal Quaternion GetQuaternionValueWithoutValidation(ushort keyIndex) =>
            BlackboardUnsafeHelpers.ReadQuaternionValue(DataReadOnlyPtr, _template.KeyData[keyIndex].MemoryOffset);
    }
}