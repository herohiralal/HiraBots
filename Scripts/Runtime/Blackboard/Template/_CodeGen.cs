#if UNITY_EDITOR
using System;
using System.Linq;

namespace HiraBots
{
    internal partial class BlackboardTemplate
    {
        public string allGeneratedCode
        {
            get
            {
                var selfKeys = sortedKeysExcludingInherited.ToArray();
                var allKeys = sortedKeysIncludingInherited.ToArray();

                var fields = string.Join(Environment.NewLine, allKeys.Select(k => k.unmanagedFieldGeneratedCode));

                var accessors = string.Join(Environment.NewLine + Environment.NewLine, selfKeys.Select(k => k.accessorGeneratedCode));

                var staticCleaners = string.Join(Environment.NewLine, selfKeys.Select(k => k.staticCleanerGeneratedCode));

                var initializers = string.Join(',' + Environment.NewLine, allKeys.Select(k => k.initializerGeneratedCode));

                var defaultInitializers = string.Join(Environment.NewLine, selfKeys.Select(k => k.defaultInitializerGeneratedCode));

                return CodeGenHelpers.ReadTemplate("Blackboard/Blackboard",
                    ("<BLACKBOARD-NAME>", name),
                    ("<BLACKBOARD-PARENT-NAME>", m_Parent == null ? "Base" : m_Parent.name),
                    ("<BLACKBOARD-KEY-FIELDS>", fields),
                    ("<BLACKBOARD-STATIC-CLEANERS>", staticCleaners),
                    ("<BLACKBOARD-DEFAULT-VALUE-INITIALIZERS>", defaultInitializers),
                    ("<BLACKBOARD-KEY-ACCESSORS>", accessors),
                    ("<BLACKBOARD-INITIALIZERS>", initializers),
                    ("<BLACKBOARD-STRING-ACCESSORS>", GetStringAccessorGeneratedCode(allKeys)));
            }
        }

        private static string GetStringAccessorGeneratedCode(BlackboardKey[] allKeys)
        {
            return string.Join(Environment.NewLine + Environment.NewLine,
                GetStringAccessorGeneratedCode(allKeys, BlackboardKeyType.Boolean),
                GetStringAccessorGeneratedCode(allKeys, BlackboardKeyType.Enum),
                GetStringAccessorGeneratedCode(allKeys, BlackboardKeyType.Float),
                GetStringAccessorGeneratedCode(allKeys, BlackboardKeyType.Integer),
                GetStringAccessorGeneratedCode(allKeys, BlackboardKeyType.Object),
                GetStringAccessorGeneratedCode(allKeys, BlackboardKeyType.Quaternion),
                GetStringAccessorGeneratedCode(allKeys, BlackboardKeyType.Vector));
        }

        private static string GetStringAccessorGeneratedCode(BlackboardKey[] allKeys, BlackboardKeyType type)
        {
            return string.Join(Environment.NewLine + Environment.NewLine,
                GetStringAccessorGeneratedCode(allKeys, type, false),
                GetStringAccessorGeneratedCode(allKeys, type, true));
        }

        private static string GetStringAccessorGeneratedCode(BlackboardKey[] allKeys, BlackboardKeyType type, bool isStatic)
        {
            return string.Join(Environment.NewLine + Environment.NewLine,
                GetStringAccessorGeneratedCode(allKeys, type, isStatic, "Getter"),
                GetStringAccessorGeneratedCode(allKeys, type, isStatic, "Setter"));
        }

        private static string GetStringAccessorGeneratedCode(BlackboardKey[] allKeys, BlackboardKeyType type, bool isStatic, string accessType)
        {
            var file = type == BlackboardKeyType.Enum
                ? $"Blackboard/BlackboardEnum{accessType}"
                : $"Blackboard/Blackboard{accessType}";

            var individualToken = $"<BLACKBOARD-INDIVIDUAL-{accessType.ToUpperInvariant()}S>";

            var individualAccessors = string.Join(Environment.NewLine, allKeys
                .Select(k => k.GetStringAccessorGeneratedCode(type, isStatic, accessType)));

            return CodeGenHelpers.ReadTemplate(file,
                ("<BLACKBOARD-STATIC-ACCESSOR>", isStatic ? " new static" : " override"),
                ("<BLACKBOARD-ACTUAL-KEY-TYPE>", BlackboardKey.GetActualTypeName(type)),
                ("<BLACKBOARD-INSTANCE-SYNCED-ACCESSOR>", isStatic ? "InstanceSynced" : ""),
                ("<BLACKBOARD-KEY-DISPLAY-TYPE>", type.ToString()),
                ("<ENUM-ACCESSOR-CONSTRAINTS>", isStatic ? " where T : unmanaged, Enum" : ""),
                (individualToken, individualAccessors));
        }
    }
}
#endif