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

                var fields = string.Join(Environment.NewLine, selfKeys.Select(k => k.unmanagedFieldGeneratedCode));

                var initializers = string.Join($",{Environment.NewLine}", selfKeys.Select(k => k.defaultInitializerGeneratedCode));

                var accessors = string.Join($"{Environment.NewLine}{Environment.NewLine}", selfKeys.Select(k => k.accessorGeneratedCode))
                    .Replace("<BLACKBOARD-NAME>", name);

                return CodeGenHelpers.ReadTemplate("Blackboard/Blackboard",
                    ("<BLACKBOARD-NAME>", name),
                    ("<BLACKBOARD-PARENT-NAME>", m_Parent == null ? "GeneratedBlackboardTemplate" : m_Parent.name),
                    ("<BLACKBOARD-KEY-FIELDS>", fields),
                    ("<BLACKBOARD-DEFAULT-INITIALIZERS>", initializers),
                    ("<BLACKBOARD-KEY-ACCESSORS>", accessors),
                    ("<BLACKBOARD-PARENT-ACCESSORS>", GetParentAccessorGeneratedCode(hierarchyIndex)));
            }
        }

        private string GetParentAccessorGeneratedCode(int hierarchyIndexToCompare)
        {
            var inherited = m_Parent == null
                ? ""
                : m_Parent.GetParentAccessorGeneratedCode(hierarchyIndexToCompare) + Environment.NewLine + Environment.NewLine;

            var parentAccess = "";
            for (var i = 0; i < hierarchyIndexToCompare - hierarchyIndex; i++)
            {
                parentAccess += ".Parent";
            }

            return inherited +
                   CodeGenHelpers.ReadTemplate("Blackboard/BlackboardParentAccessor",
                       ("<BLACKBOARD-NAME>", name),
                       ("<BLACKBOARD-PARENT-ACCESS>", parentAccess));
        }
    }
}
#endif