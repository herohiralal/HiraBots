using UnityEngine;

namespace HiraBots
{
    internal partial class BlackboardComponent
    {
        internal void SetBooleanValue(string key, bool value, bool expected = false)
        {
            ValidateInput(key, BlackboardKeyType.Boolean, out var data);
            SetBooleanValueWithoutValidation(in data, value, expected);
        }

        internal void SetBooleanValue(ushort key, bool value, bool expected = false)
        {
            ValidateInput(key, BlackboardKeyType.Boolean, out var data);
            SetBooleanValueWithoutValidation(in data, value, expected);
        }

        internal void SetEnumValue<T>(string key, T value, bool expected = false) where T : unmanaged, System.Enum
        {
            ValidateEnumType<T>();
            ValidateInput(key, BlackboardKeyType.Enum, out var data);
            SetEnumValueWithoutValidation<T>(in data, value, expected);
        }

        internal void SetEnumValue<T>(ushort key, T value, bool expected = false) where T : unmanaged, System.Enum
        {
            ValidateEnumType<T>();
            ValidateInput(key, BlackboardKeyType.Enum, out var data);
            SetEnumValueWithoutValidation<T>(in data, value, expected);
        }

        internal void SetFloatValue(string key, float value, bool expected = false)
        {
            ValidateInput(key, BlackboardKeyType.Float, out var data);
            SetFloatValueWithoutValidation(in data, value, expected);
        }

        internal void SetFloatValue(ushort key, float value, bool expected = false)
        {
            ValidateInput(key, BlackboardKeyType.Float, out var data);
            SetFloatValueWithoutValidation(in data, value, expected);
        }

        internal void SetIntegerValue(string key, int value, bool expected = false)
        {
            ValidateInput(key, BlackboardKeyType.Integer, out var data);
            SetIntegerValueWithoutValidation(in data, value, expected);
        }

        internal void SetIntegerValue(ushort key, int value, bool expected = false)
        {
            ValidateInput(key, BlackboardKeyType.Integer, out var data);
            SetIntegerValueWithoutValidation(in data, value, expected);
        }

        internal void SetObjectValue(string key, Object value, bool expected = false)
        {
            ValidateInput(key, BlackboardKeyType.Object, out var data);
            SetObjectValueWithoutValidation(in data, value, expected);
        }

        internal void SetObjectValue(ushort key, Object value, bool expected = false)
        {
            ValidateInput(key, BlackboardKeyType.Object, out var data);
            SetObjectValueWithoutValidation(in data, value, expected);
        }

        internal void SetVectorValue(string key, Vector3 value, bool expected = false)
        {
            ValidateInput(key, BlackboardKeyType.Vector, out var data);
            SetVectorValueWithoutValidation(in data, value, expected);
        }

        internal void SetVectorValue(ushort key, Vector3 value, bool expected = false)
        {
            ValidateInput(key, BlackboardKeyType.Vector, out var data);
            SetVectorValueWithoutValidation(in data, value, expected);
        }

        internal void SetQuaternionValue(string key, Quaternion value, bool expected = false)
        {
            ValidateInput(key, BlackboardKeyType.Quaternion, out var data);
            SetQuaternionValueWithoutValidation(in data, value, expected);
        }

        internal void SetQuaternionValue(ushort key, Quaternion value, bool expected = false)
        {
            ValidateInput(key, BlackboardKeyType.Quaternion, out var data);
            SetQuaternionValueWithoutValidation(in data, value, expected);
        }
    }
}