using System;

#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
#endif

using UnityEngine;

namespace HiraBots
{
    [Serializable]
    internal struct DynamicEnum
    {
#if UNITY_EDITOR
        [SerializeField] internal string m_TypeIdentifier;
#endif
        [SerializeField] internal byte m_Value;

        public static implicit operator byte(DynamicEnum input) => input.m_Value;

#if UNITY_EDITOR
        [InitializeOnLoad]
        internal static class Helpers
        {
            static Helpers()
            {
                var dynamicEnumTypes = TypeCache
                    .GetTypesWithAttribute<ExposedToHiraBotsAttribute>()
                    .Where(t => t.IsEnum && (t.GetEnumUnderlyingType() == typeof(byte) || t.GetEnumUnderlyingType() == typeof(sbyte)));

                var dT2I = new Dictionary<Type, string>();
                var dI2T = new Dictionary<string, Type>();

                foreach (var type in dynamicEnumTypes)
                {
                    var identifier = type.GetCustomAttribute<ExposedToHiraBotsAttribute>().m_Identifier;
                    dT2I.Add(type, identifier);
                    dI2T.Add(identifier, type);
                }

                Helpers.typeToIdentifier = dT2I;
                Helpers.identifierToType = dI2T;
            }

            public static ReadOnlyDictionaryAccessor<Type, string> typeToIdentifier { get; }
            public static ReadOnlyDictionaryAccessor<string, Type> identifierToType { get; }
        }
#endif
    }
}