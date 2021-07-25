using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    internal static class CookingHelpers
    {
        internal static IEnumerable<T> LoadAndGetAllObjectsOfType<T>() where T : Object =>
            AssetDatabase
                .FindAssets($"t:{typeof(T).FullName}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<T>);

        internal static bool TryGenerateBlackboardTemplateCollection(out BlackboardTemplateCollection output)
        {
            var validator = new BlackboardValidator();

            var templatesToCook = AssetDatabase
                .FindAssets($"t:{typeof(BlackboardTemplate).FullName}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => !path.Contains("HiraBotsTestObjects"))
                .Select(AssetDatabase.LoadAssetAtPath<BlackboardTemplate>)
                .OrderBy(bt => bt.HierarchyIndex)
                .ToArray();

            var result = true;

            foreach (var template in templatesToCook)
            {
                if (!validator.Execute(template, out var errorText))
                {
                    Debug.LogError(errorText, template);
                    result = false;
                }
            }

            output = result ? BlackboardTemplateCollection.Create(templatesToCook) : null;
            return result;
        }
    }
}