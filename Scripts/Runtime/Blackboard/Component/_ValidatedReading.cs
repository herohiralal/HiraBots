using UnityEngine;

namespace HiraBots
{
    internal partial class BlackboardComponent
    {
        internal bool GetBooleanValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Boolean, out var data);
            return GetBooleanValueWithoutValidation(data.m_MemoryOffset);
        }

        internal bool GetBooleanValue(ushort key)
        {
            ValidateInput(key, BlackboardKeyType.Boolean, out var data);
            return GetBooleanValueWithoutValidation(data.m_MemoryOffset);
        }

        internal T GetEnumValue<T>(string key) where T : unmanaged, System.Enum
        {
            ValidateEnumType<T>();
            ValidateInput(key, BlackboardKeyType.Enum, out var data);
            return GetEnumValueWithoutValidation<T>(data.m_MemoryOffset);
        }

        internal T GetEnumValue<T>(ushort key) where T : unmanaged, System.Enum
        {
            ValidateEnumType<T>();
            ValidateInput(key, BlackboardKeyType.Enum, out var data);
            return GetEnumValueWithoutValidation<T>(data.m_MemoryOffset);
        }

        internal float GetFloatValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Float, out var data);
            return GetFloatValueWithoutValidation(data.m_MemoryOffset);
        }

        internal float GetFloatValue(ushort key)
        {
            ValidateInput(key, BlackboardKeyType.Float, out var data);
            return GetFloatValueWithoutValidation(data.m_MemoryOffset);
        }

        internal int GetIntegerValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Integer, out var data);
            return GetIntegerValueWithoutValidation(data.m_MemoryOffset);
        }

        internal int GetIntegerValue(ushort key)
        {
            ValidateInput(key, BlackboardKeyType.Integer, out var data);
            return GetIntegerValueWithoutValidation(data.m_MemoryOffset);
        }

        internal Object GetObjectValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Object, out var data);
            return GetObjectValueWithoutValidation(data.m_MemoryOffset);
        }

        internal Object GetObjectValue(ushort key)
        {
            ValidateInput(key, BlackboardKeyType.Object, out var data);
            return GetObjectValueWithoutValidation(data.m_MemoryOffset);
        }

        internal Vector3 GetVectorValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Vector, out var data);
            return GetVectorValueWithoutValidation(data.m_MemoryOffset);
        }

        internal Vector3 GetVectorValue(ushort key)
        {
            ValidateInput(key, BlackboardKeyType.Vector, out var data);
            return GetVectorValueWithoutValidation(data.m_MemoryOffset);
        }

        internal Quaternion GetQuaternionValue(string key)
        {
            ValidateInput(key, BlackboardKeyType.Quaternion, out var data);
            return GetQuaternionValueWithoutValidation(data.m_MemoryOffset);
        }

        internal Quaternion GetQuaternionValue(ushort key)
        {
            ValidateInput(key, BlackboardKeyType.Quaternion, out var data);
            return GetQuaternionValueWithoutValidation(data.m_MemoryOffset);
        }
    }
}