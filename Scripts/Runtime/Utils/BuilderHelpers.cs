#if HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
using UnityEngine;

namespace HiraBots
{
    internal static class BuilderHelpers
    {
        /// <summary>
        /// Build a ScriptableObject.
        /// </summary>
        internal static T BuildScriptableObject<T>(this string name, HideFlags hideFlags = HideFlags.HideAndDontSave)
            where T : ScriptableObject
        {
            var output = ScriptableObject.CreateInstance<T>();

            if (name != null)
            {
                output.name = name;
            }

            output.hideFlags = hideFlags;

            return output;
        }
    }
}
#endif