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
    /// <summary>
    /// An enum value which can have its type selected dynamically from a dropdown in the inspector.
    /// The type identification data is not present in a built player.
    /// </summary>
    [Serializable]
    internal struct DynamicEnum
    {
#if UNITY_EDITOR
        [SerializeField] internal string m_TypeIdentifier;
#endif
        [SerializeField] internal byte m_Value;

        public static implicit operator byte(DynamicEnum input) => input.m_Value;

#if UNITY_EDITOR

        /// <summary>
        /// Helper class for DynamicEnum.
        /// </summary>
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
                    var identifier = type.GetCustomAttribute<ExposedToHiraBotsAttribute>().identifier;
                    dT2I.Add(type, identifier);
                    dI2T.Add(identifier, type);
                }

                typeToIdentifier = dT2I;
                identifierToType = dI2T;
            }

            /// <summary>
            /// A map from a Type to its corresponding type identifier.
            /// </summary>
            internal static ReadOnlyDictionaryAccessor<Type, string> typeToIdentifier { get; }

            /// <summary>
            /// A map from a type identifier to its corresponding Type.
            /// </summary>
            internal static ReadOnlyDictionaryAccessor<string, Type> identifierToType { get; }
        }
#endif
    }
}