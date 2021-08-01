using UnityEngine;

namespace HiraBots
{
    internal partial class BlackboardComponent
    {
        internal bool GetBooleanValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Boolean, out var data);
            return GetBooleanValueWithoutValidation(data.MemoryOffset);
        }

        internal bool GetBooleanValue(ushort key)
        {
            ValidateInput(key, BlackboardKeyType.Boolean, out var data);
            return GetBooleanValueWithoutValidation(data.MemoryOffset);
        }

        internal T GetEnumValue<T>(string key) where T : unmanaged, System.Enum
        {
            ValidateEnumType<T>();
            ValidateInput(key, BlackboardKeyType.Enum, out var data);
            return GetEnumValueWithoutValidation<T>(data.MemoryOffset);
        }

        internal T GetEnumValue<T>(ushort key) where T : unmanaged, System.Enum
        {
            ValidateEnumType<T>();
            ValidateInput(key, BlackboardKeyType.Enum, out var data);
            return GetEnumValueWithoutValidation<T>(data.MemoryOffset);
        }

        internal float GetFloatValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Float, out var data);
            return GetFloatValueWithoutValidation(data.MemoryOffset);
        }

        internal float GetFloatValue(ushort key)
        {
            ValidateInput(key, BlackboardKeyType.Float, out var data);
            return GetFloatValueWithoutValidation(data.MemoryOffset);
        }

        internal int GetIntegerValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Integer, out var data);
            return GetIntegerValueWithoutValidation(data.MemoryOffset);
        }

        internal int GetIntegerValue(ushort key)
        {
            ValidateInput(key, BlackboardKeyType.Integer, out var data);
            return GetIntegerValueWithoutValidation(data.MemoryOffset);
        }

        internal Object GetObjectValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Object, out var data);
            return GetObjectValueWithoutValidation(data.MemoryOffset);
        }

        internal Object GetObjectValue(ushort key)
        {
            ValidateInput(key, BlackboardKeyType.Object, out var data);
            return GetObjectValueWithoutValidation(data.MemoryOffset);
        }

        internal Vector3 GetVectorValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Vector, out var data);
            return GetVectorValueWithoutValidation(data.MemoryOffset);
        }

        internal Vector3 GetVectorValue(ushort key)
        {
            ValidateInput(key, BlackboardKeyType.Vector, out var data);
            return GetVectorValueWithoutValidation(data.MemoryOffset);
        }

        internal Quaternion GetQuaternionValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Quaternion, out var data);
            return GetQuaternionValueWithoutValidation(data.MemoryOffset);
        }

        internal Quaternion GetQuaternionValue(ushort key)
        {
            ValidateInput(key, BlackboardKeyType.Quaternion, out var data);
            return GetQuaternionValueWithoutValidation(data.MemoryOffset);
        }
    }
}