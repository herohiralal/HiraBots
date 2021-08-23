#if HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
using System.Reflection;
using Unity.Mathematics;
using UnityEngine;

namespace HiraBots
{
    internal abstract partial class BlackboardKey
    {
        /// <summary>
        /// Build a BlackboardKey.
        /// </summary>
        internal void BuildBlackboardKey(BlackboardKeyTraits traits)
        {
            m_InstanceSynced = (traits & BlackboardKeyTraits.InstanceSynced) != 0;
            m_EssentialToDecisionMaking = (traits & BlackboardKeyTraits.BroadcastEventOnUnexpectedChange) != 0;
        }
    }

    internal partial class BooleanBlackboardKey
    {
        /// <summary>
        /// Build a BooleanBlackboardKey.
        /// </summary>
        internal void BuildBooleanBlackboardKey(bool defaultValue)
        {
            m_DefaultValue = defaultValue;
        }
    }

    internal unsafe partial class EnumBlackboardKey
    {
        /// <summary>
        /// Build an EnumBlackboardKey.
        /// </summary>
        internal void BuildEnumBlackboardKey<TEnumType>(TEnumType defaultValue)
            where TEnumType : unmanaged, System.Enum
        {
            var enumType = typeof(TEnumType);
            if (enumType.IsEnum)
            {
                if (enumType.GetEnumUnderlyingType() == typeof(byte) || enumType.GetEnumUnderlyingType() == typeof(sbyte))
                {
#if UNITY_EDITOR // type identifier is not present outside the editor
                    var exposedToHiraBotsAttribute = enumType.GetCustomAttribute<ExposedToHiraBotsAttribute>();
                    if (exposedToHiraBotsAttribute != null)
                    {
                        m_DefaultValue.m_TypeIdentifier = exposedToHiraBotsAttribute.identifier;
                    }
#endif
                }
            }

            m_DefaultValue.m_Value = *(byte*) &defaultValue;
        }
    }

    internal partial class FloatBlackboardKey
    {
        /// <summary>
        /// Build a FloatBlackboardKey.
        /// </summary>
        internal void BuildFloatBlackboardKey(float defaultValue)
        {
            m_DefaultValue = defaultValue;
        }
    }

    internal partial class IntegerBlackboardKey
    {
        /// <summary>
        /// Build an IntegerBlackboardKey.
        /// </summary>
        internal void BuildIntegerBlackboardKey(int defaultValue)
        {
            m_DefaultValue = defaultValue;
        }
    }

    internal partial class ObjectBlackboardKey
    {
        /// <summary>
        /// Build an ObjectBlackboardKey.
        /// </summary>
        internal void BuildObjectBlackboardKey()
        {
        }
    }

    internal partial class QuaternionBlackboardKey
    {
        /// <summary>
        /// Build a QuaternionBlackboardKey.
        /// </summary>
        internal void BuildQuaternionBlackboardKey(float3 defaultValue)
        {
            m_DefaultValue = defaultValue;
        }
    }

    internal partial class VectorBlackboardKey
    {
        /// <summary>
        /// Build a VectorBlackboardKey.
        /// </summary>
        internal void BuildVectorBlackboardKey(float3 defaultValue)
        {
            m_DefaultValue = defaultValue;
        }
    }
}
#endif