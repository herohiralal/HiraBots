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
        [SerializeField] internal string typeIdentifier;
#endif
        [SerializeField] internal byte value;

        public static implicit operator byte(DynamicEnum input) => input.value;

#if UNITY_EDITOR
        [InitializeOnLoad]
        internal static class Helpers
        {
            static Helpers()
            {
                var dynamicEnumTypes = TypeCache
                    .GetTypesWithAttribute<ExposedToHiraBotsAttribute>()
                    .Where(t => t.IsEnum && (t.GetEnumUnderlyingType() == typeof(byte) || t.GetEnumUnderlyingType() == typeof(sbyte)));

                var typeToIdentifier = new Dictionary<Type, string>();
                var identifierToType = new Dictionary<string, Type>();

                foreach (var type in dynamicEnumTypes)
                {
                    var identifier = type.GetCustomAttribute<ExposedToHiraBotsAttribute>().Identifier;
                    typeToIdentifier.Add(type, identifier);
                    identifierToType.Add(identifier, type);
                }

                TYPE_TO_IDENTIFIER = typeToIdentifier;
                IDENTIFIER_TO_TYPE = identifierToType;
            }

            public static readonly ReadOnlyDictionaryAccessor<Type, string> TYPE_TO_IDENTIFIER;
            public static readonly ReadOnlyDictionaryAccessor<string, Type> IDENTIFIER_TO_TYPE;
        }
#endif
    }
}