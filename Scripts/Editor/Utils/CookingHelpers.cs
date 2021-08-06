using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    /// <summary>
    /// Helpers to cook objects into the build or before play mode.
    /// </summary>
    internal static class CookingHelpers
    {
        /// <summary>
        /// Validate all the blackboard templates in the project, and pack them into a collection.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        internal static bool TryGenerateBlackboardTemplateCollection(out BlackboardTemplateCollection output)
        {
            var validator = new BlackboardTemplateValidator();

            // objects used for testing are subclasses that get compiled away, leaving the asset itself as
            // not being determined a Blackboard Template

            // the templates get ordered by their hierarchy index because the runtime compiler assumes they're
            // ordered in a way that they can be compiled in the sequence

            var templatesToCook = AssetDatabase
                .FindAssets($"t:{typeof(BlackboardTemplate).FullName}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<BlackboardTemplate>)
                .OrderBy(bt => bt.hierarchyIndex)
                .ToArray();

            var result = true;

            foreach (var template in templatesToCook)
            {
                if (validator.Validate(template, out var errorText))
                {
                    continue;
                }

                Debug.LogError(errorText, template);
                result = false;
            }

            output = result ? BlackboardTemplateCollection.Create(templatesToCook) : null;
            return result;
        }
    }
}