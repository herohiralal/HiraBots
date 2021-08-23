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
                var initializers = string.Join($",{Environment.NewLine}", allKeys.Select(k => k.defaultInitializerGeneratedCode));

                return CodeGenHelpers.ReadTemplate("Blackboard/Blackboard",
                    ("<BLACKBOARD-NAME>", name),
                    ("<BLACKBOARD-KEY-FIELDS>", fields),
                    ("<BLACKBOARD-DEFAULT-INITIALIZERS>", initializers));
            }
        }
    }
}
#endif