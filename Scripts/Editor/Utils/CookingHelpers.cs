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

        internal static IEnumerable<BlackboardTemplate> LoadBlackboardTemplatesToCook() =>
            AssetDatabase
                .FindAssets($"t:{typeof(BlackboardTemplate).FullName}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => !path.Contains("HiraBotsTestObjects"))
                .Select(AssetDatabase.LoadAssetAtPath<BlackboardTemplate>);
    }
}