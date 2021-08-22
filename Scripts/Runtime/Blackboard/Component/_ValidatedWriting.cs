using Unity.Mathematics;
using UnityEngine;

namespace HiraBots
{
    internal partial class BlackboardComponent
    {
        /// <summary>
        /// Set Boolean value on the blackboard using the key name.
        /// </summary>
        internal void SetBooleanValue(string key, bool value, bool expected = false)
        {
            ValidateInput(key, BlackboardKeyType.Boolean, out var data);
            SetBooleanValueWithoutValidation(in data, value, expected);
        }

        /// <summary>
        /// Set Enum value on the blackboard using the key name.
        /// </summary>
        internal void SetEnumValue<T>(string key, T value, bool expected = false) where T : unmanaged, System.Enum
        {
            ValidateEnumType<T>();
            ValidateInput(key, BlackboardKeyType.Enum, out var data);
            SetEnumValueWithoutValidation<T>(in data, value, expected);
        }

        /// <summary>
        /// Set Float value on the blackboard using the key name.
        /// </summary>
        internal void SetFloatValue(string key, float value, bool expected = false)
        {
            ValidateInput(key, BlackboardKeyType.Float, out var data);
            SetFloatValueWithoutValidation(in data, value, expected);
        }

        /// <summary>
        /// Set Integer value on the blackboard using the key name.
        /// </summary>
        internal void SetIntegerValue(string key, int value, bool expected = false)
        {
            ValidateInput(key, BlackboardKeyType.Integer, out var data);
            SetIntegerValueWithoutValidation(in data, value, expected);
        }

        /// <summary>
        /// Set Object value on the blackboard using the key name.
        /// </summary>
        internal void SetObjectValue(string key, Object value, bool expected = false)
        {
            ValidateInput(key, BlackboardKeyType.Object, out var data);
            SetObjectValueWithoutValidation(in data, value, expected);
        }

        /// <summary>
        /// Set Vector value on the blackboard using the key name.
        /// </summary>
        internal void SetVectorValue(string key, float3 value, bool expected = false)
        {
            ValidateInput(key, BlackboardKeyType.Vector, out var data);
            SetVectorValueWithoutValidation(in data, value, expected);
        }

        /// <summary>
        /// Set Quaternion value on the blackboard using the key name.
        /// </summary>
        internal void SetQuaternionValue(string key, quaternion value, bool expected = false)
        {
            ValidateInput(key, BlackboardKeyType.Quaternion, out var data);
            SetQuaternionValueWithoutValidation(in data, value, expected);
        }
    }
}