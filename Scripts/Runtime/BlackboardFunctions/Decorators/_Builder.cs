#if HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
using System.Reflection;
using UnityEngine;

namespace HiraBots
{
    internal partial class AlwaysSucceedDecoratorBlackboardFunction
    {
        /// <summary>
        /// Build an AlwaysSucceed function.
        /// </summary>
        internal void BuildAlwaysSucceedFunction()
        {
            // nothing lol
        }
    }

    internal partial class EnumHasFlagsDecoratorBlackboardFunction
    {
        /// <summary>
        /// Build an EnumHasFlags function.
        /// </summary>
        internal unsafe void BuildEnumHasFlagsFunction<TEnumType>(BlackboardKey key, TEnumType value)
            where TEnumType : unmanaged, System.Enum
        {
            m_Key.selectedKey = key;

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

    internal partial class IsSetDecoratorBlackboardFunction
    {
        /// <summary>
        /// Build an IsSet function.
        /// </summary>
        internal void BuildIsSetFunction(BlackboardKey key)
        {
            m_Key.selectedKey = key;
        }
    }

    internal partial class NumericalComparisonDecoratorBlackboardFunction
    {
        /// <summary>
        /// Build a NumericalComparison function.
        /// </summary>
        internal void BuildNumericalComparisonFunction(BlackboardKey key, ComparisonType comparisonType, float value, float equalityTolerance = 0)
        {
            m_Key.selectedKey = key;
            m_ComparisonType = comparisonType;
            m_Value = value;
            m_EqualityTolerance = equalityTolerance;
        }
    }

    internal partial class ObjectEqualsDecoratorBlackboardFunction
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