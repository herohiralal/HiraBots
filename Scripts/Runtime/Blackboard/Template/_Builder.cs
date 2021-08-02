#if UNITY_EDITOR // ideally manual creation must only be done for testing purposes as it skips validation checks
using System.Reflection;
using UnityEngine;

namespace HiraBots
{
    internal abstract partial class BlackboardKey
    {
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
                    var exposedToHiraBotsAttribute = enumType.GetCustomAttribute<ExposedToHiraBotsAttribute>();
                    if (exposedToHiraBotsAttribute != null)
                        output.m_DefaultValue.m_TypeIdentifier = exposedToHiraBotsAttribute.m_Identifier;
                }
            }

            output.m_DefaultValue.m_Value = *(byte*) &defaultValue;
            return output;
        }
    }

    internal partial class FloatBlackboardKey
    {
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
        internal static QuaternionBlackboardKey Build<T>(string name, BlackboardKeyTraits traits, Vector3 defaultValue, HideFlags hideFlags = HideFlags.None)
            where T : QuaternionBlackboardKey
        {
            var output = BlackboardKey.Build<T>(name, traits, hideFlags);
            output.m_DefaultValue = defaultValue;
            return output;
        }
    }

    internal partial class VectorBlackboardKey
    {
        internal static VectorBlackboardKey Build<T>(string name, BlackboardKeyTraits traits, Vector3 defaultValue, HideFlags hideFlags = HideFlags.None)
            where T : VectorBlackboardKey
        {
            var output = BlackboardKey.Build<T>(name, traits, hideFlags);
            output.m_DefaultValue = defaultValue;
            return output;
        }
    }

    internal partial class BlackboardTemplate
    {
        internal static T Build<T>(string name, BlackboardTemplate parent, BlackboardKey[] keys, HideFlags hideFlags = HideFlags.None)
            where T : BlackboardTemplate
        {
            var output = CreateInstance<T>();
            output.hideFlags = hideFlags;
            output.name = name;
            output.m_Parent = parent;
            output.m_Keys = keys;
            return output;
        }
    }
}

#endif