using UnityEngine;

namespace HiraBots
{
    internal partial class BlackboardTemplateCompiledData
    {
        internal bool GetInstanceSyncedBooleanValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Boolean, out var data);
            return GetInstanceSyncedBooleanValueWithoutValidation(data.MemoryOffset);
        }

        internal bool GetInstanceSyncedBooleanValue(ushort key)
        {
            ValidateInput(key, BlackboardKeyType.Boolean, out var data);
            return GetInstanceSyncedBooleanValueWithoutValidation(data.MemoryOffset);
        }

        internal T GetInstanceSyncedEnumValue<T>(string key) where T : unmanaged, System.Enum
        {
            ValidateEnumType<T>();
            ValidateInput(key, BlackboardKeyType.Enum, out var data);
            return GetInstanceSyncedEnumValueWithoutValidation<T>(data.MemoryOffset);
        }

        internal T GetInstanceSyncedEnumValue<T>(ushort key) where T : unmanaged, System.Enum
        {
            ValidateEnumType<T>();
            ValidateInput(key, BlackboardKeyType.Enum, out var data);
            return GetInstanceSyncedEnumValueWithoutValidation<T>(data.MemoryOffset);
        }

        internal float GetInstanceSyncedFloatValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Float, out var data);
            return GetInstanceSyncedFloatValueWithoutValidation(data.MemoryOffset);
        }

        internal float GetInstanceSyncedFloatValue(ushort key)
        {
            ValidateInput(key, BlackboardKeyType.Float, out var data);
            return GetInstanceSyncedFloatValueWithoutValidation(data.MemoryOffset);
        }

        internal int GetInstanceSyncedIntegerValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Integer, out var data);
            return GetInstanceSyncedIntegerValueWithoutValidation(data.MemoryOffset);
        }

        internal int GetInstanceSyncedIntegerValue(ushort key)
        {
            ValidateInput(key, BlackboardKeyType.Integer, out var data);
            return GetInstanceSyncedIntegerValueWithoutValidation(data.MemoryOffset);
        }

        internal Object GetInstanceSyncedObjectValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Object, out var data);
            return GetInstanceSyncedObjectValueWithoutValidation(data.MemoryOffset);
        }

        internal Object GetInstanceSyncedObjectValue(ushort key)
        {
            ValidateInput(key, BlackboardKeyType.Object, out var data);
            return GetInstanceSyncedObjectValueWithoutValidation(data.MemoryOffset);
        }

        internal Vector3 GetInstanceSyncedVectorValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Vector, out var data);
            return GetInstanceSyncedVectorValueWithoutValidation(data.MemoryOffset);
        }

        internal Vector3 GetInstanceSyncedVectorValue(ushort key)
        {
            ValidateInput(key, BlackboardKeyType.Vector, out var data);
            return GetInstanceSyncedVectorValueWithoutValidation(data.MemoryOffset);
        }

        internal Quaternion GetInstanceSyncedQuaternionValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Quaternion, out var data);
            return GetInstanceSyncedQuaternionValueWithoutValidation(data.MemoryOffset);
        }

        internal Quaternion GetInstanceSyncedQuaternionValue(ushort key)
        {
            ValidateInput(key, BlackboardKeyType.Quaternion, out var data);
            return GetInstanceSyncedQuaternionValueWithoutValidation(data.MemoryOffset);
        }
    }
}