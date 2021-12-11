#if HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
using System.Reflection;
using UnityEngine;

namespace HiraBots
{
    internal partial class EnumOperatorEffectorBlackboardFunction
    {
        /// <summary>
        /// Build an EnumOperator function.
        /// </summary>
        internal unsafe void BuildEnumOperatorFunction<TEnumType>(BlackboardKey key, OperationType operationType, TEnumType value)
            where TEnumType : unmanaged, System.Enum
        {
            m_Key.selectedKey = key;
            m_OperationType = operationType;

#if UNITY_EDITOR // type identifier is not present outside the editor
            var enumType = typeof(TEnumType);
            if (enumType.IsEnum)
            {
                if (enumType.GetEnumUnderlyingType() == typeof(byte) || enumType.GetEnumUnderlyingType() == typeof(sbyte))
                {
                    var exposedToHiraBotsAttribute = enumType.GetCustomAttribute<ExposedToHiraBotsAttribute>();
                    if (exposedToHiraBotsAttribute != null)
                    {
                        m_Value.m_TypeIdentifier = exposedToHiraBotsAttribute.identifier;
                    }
                }
            }
#endif

            m_Value.m_Value = *(byte*) &value;
        }
    }

    internal partial class FloatOperatorEffectorBlackboardFunction
    {
        /// <summary>
        /// Build a FloatOperator function.
        /// </summary>
        internal void BuildFloatOperatorFunction(BlackboardKey key, OperationType operationType, float value)
        {
            m_Key.selectedKey = key;
            m_OperationType = operationType;
            m_Value = value;
        }
    }

    internal partial class IntegerOperatorEffectorBlackboardFunction
    {
        /// <summary>
        /// Build an IntegerOperator function.
        /// </summary>
        internal void BuildIntegerOperatorFunction(BlackboardKey key, OperationType operationType, int value)
        {
            m_Key.selectedKey = key;
            m_OperationType = operationType;
            m_Value = value;
        }
    }

    internal partial class IsSetEffectorBlackboardFunction
    {
        internal void BuildIsSetFunction(BlackboardKey key, OperationType operationType)
        {
            m_Key.selectedKey = key;
            m_OperationType = operationType;
        }
    }

    internal partial class ObjectEqualsEffectorBlackboardFunction
    {
        /// <summary>
        /// Build an ObjectEquals function.
        /// </summary>
        internal void BuildObjectEqualsFunction(BlackboardKey key, Object value)
        {
            m_Key.selectedKey = key;
            m_Value = value;
        }
    }
}
#endif