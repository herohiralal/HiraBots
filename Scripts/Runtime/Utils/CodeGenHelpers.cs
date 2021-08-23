#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace HiraBots
{
    internal static class CodeGenHelpers
    {
        private static readonly Dictionary<string, TextAsset> s_LoadedTextAssets = new Dictionary<string, TextAsset>();

        internal static string ReadTemplate(string pathInResources, params (string key, string value)[] wildcards)
        {
            if (!s_LoadedTextAssets.TryGetValue(pathInResources, out var asset))
            {
                asset = Resources.Load<TextAsset>(pathInResources);
                s_LoadedTextAssets.Add(pathInResources, asset);
            }

            var template = asset.text;

            foreach (var (key, value) in wildcards)
            {
                template = template.Replace(key, value);
            }

            return template;
        }
    }
}
#endif