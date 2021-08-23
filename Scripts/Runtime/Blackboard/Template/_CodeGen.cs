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
                    ("<BLACKBOARD-INITIALIZERS>", initializers));
            }
        }
    }
}
#endif