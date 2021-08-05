using UnityEngine;

namespace HiraBots
{
    internal partial class BlackboardTemplate
    {
        /// <summary>
        /// Build a BlackboardTemplate.
        /// </summary>
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