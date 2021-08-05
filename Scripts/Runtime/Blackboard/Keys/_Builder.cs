﻿#if HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
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
        protected static T Build<T>(string name, BlackboardKeyTraits traits, HideFlags hideFlags = HideFlags.None)
            where T : BlackboardKey
        {
            var output = CreateInstance<T>();
            output.hideFlags = hideFlags;
            output.name = name;
            output.m_InstanceSynced = (traits & BlackboardKeyTraits.InstanceSynced) != 0;
            output.m_EssentialToDecisionMaking = (traits & BlackboardKeyTraits.BroadcastEventOnUnexpectedChange) != 0;
            return output;
        }
    }

    internal partial class BooleanBlackboardKey
    {
        /// <summary>
        /// Build a BooleanBlackboardKey.
        /// </summary>
        internal static BooleanBlackboardKey Build<T>(string name, BlackboardKeyTraits traits, bool defaultValue, HideFlags hideFlags = HideFlags.None)
            where T : BooleanBlackboardKey
        {
            var output = BlackboardKey.Build<T>(name, traits, hideFlags);
            output.m_DefaultValue = defaultValue;
            return output;
        }
    }

    internal unsafe partial class EnumBlackboardKey
    {
        /// <summary>
        /// Build an EnumBlackboardKey.
        /// </summary>
        internal static EnumBlackboardKey Build<T, TEnumType>(string name, BlackboardKeyTraits traits, TEnumType defaultValue, HideFlags hideFlags = HideFlags.None)
            where T : EnumBlackboardKey where TEnumType : unmanaged, System.Enum
        {
            var output = BlackboardKey.Build<T>(name, traits, hideFlags);
            output.m_DefaultValue.m_TypeIdentifier = "";

            var enumType = typeof(TEnumType);
            if (enumType.IsEnum)
            {
                if (enumType.GetEnumUnderlyingType() == typeof(byte) || enumType.GetEnumUnderlyingType() == typeof(sbyte))
                {
#if UNITY_EDITOR // type identifier is not present outside the editor
                    var exposedToHiraBotsAttribute = enumType.GetCustomAttribute<ExposedToHiraBotsAttribute>();
                    if (exposedToHiraBotsAttribute != null)
                    {
                        output.m_DefaultValue.m_TypeIdentifier = exposedToHiraBotsAttribute.identifier;
                    }
#endif
                }
            }

            output.m_DefaultValue.m_Value = *(byte*) &defaultValue;
            return output;
        }
    }

    internal partial class FloatBlackboardKey
    {
        /// <summary>
        /// Build a FloatBlackboardKey.
        /// </summary>
        internal static FloatBlackboardKey Build<T>(string name, BlackboardKeyTraits traits, float defaultValue, HideFlags hideFlags = HideFlags.None)
            where T : FloatBlackboardKey
        {
            var output = BlackboardKey.Build<T>(name, traits, hideFlags);
            output.m_DefaultValue = defaultValue;
            return output;
        }
    }

    internal partial class IntegerBlackboardKey
    {
        /// <summary>
        /// Build a IntegerBlackboardKey.
        /// </summary>
        internal static IntegerBlackboardKey Build<T>(string name, BlackboardKeyTraits traits, int defaultValue, HideFlags hideFlags = HideFlags.None)
            where T : IntegerBlackboardKey
        {
            var output = BlackboardKey.Build<T>(name, traits, hideFlags);
            output.m_DefaultValue = defaultValue;
            return output;
        }
    }

    internal partial class ObjectBlackboardKey
    {
        /// <summary>
        /// Build a ObjectBlackboardKey.
        /// </summary>
        internal static ObjectBlackboardKey Build<T>(string name, BlackboardKeyTraits traits, Object defaultValue, HideFlags hideFlags = HideFlags.None)
            where T : ObjectBlackboardKey
        {
            var output = BlackboardKey.Build<T>(name, traits, hideFlags);
            output.m_DefaultValue = defaultValue;
            return output;
        }
    }

    internal partial class QuaternionBlackboardKey
    {
        /// <summary>
        /// Build a QuaternionBlackboardKey.
        /// </summary>
        internal static QuaternionBlackboardKey Build<T>(string name, BlackboardKeyTraits traits, float3 defaultValue, HideFlags hideFlags = HideFlags.None)
            where T : QuaternionBlackboardKey
        {
            var output = BlackboardKey.Build<T>(name, traits, hideFlags);
            output.m_DefaultValue = defaultValue;
            return output;
        }
    }

    internal partial class VectorBlackboardKey
    {
        /// <summary>
        /// Build a VectorBlackboardKey.
        /// </summary>
        internal static VectorBlackboardKey Build<T>(string name, BlackboardKeyTraits traits, float3 defaultValue, HideFlags hideFlags = HideFlags.None)
            where T : VectorBlackboardKey
        {
            var output = BlackboardKey.Build<T>(name, traits, hideFlags);
            output.m_DefaultValue = defaultValue;
            return output;
        }
    }
}
#endif