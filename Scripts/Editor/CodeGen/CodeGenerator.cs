using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    /// <summary>
    /// The main entry point for code generation
    /// </summary>
    internal static class CodeGeneratorEntryPoint
    {
        /// <summary>
        /// Generate code for all HiraBots objects.
        /// </summary>
        [MenuItem("Assets/Generate HiraBots Code", false, priority = 800)]
        private static void GenerateHiraBotsCode()
        {
            var success = true;

            // create folder/asmdef
            EditorSerializationUtility.ConfirmCodeGenFolder();
            EditorSerializationUtility.CreateCodeGenAssemblyDefinition();

            if (!CookingHelpers.TryGetBlackboardTemplatesToGenerateCodeFor(out var blackboardTemplates))
            {
                success = false;
            }

            var blackboardData = new Dictionary<BlackboardTemplate, ReadOnlyHashSetAccessor<BlackboardKey>>();

            foreach (var (_, template) in blackboardTemplates)
            {
                var hs = new HashSet<BlackboardKey>();
                template.GetKeySet(hs);
                blackboardData.Add(template, hs.ReadOnly());
            }

            if (!CookingHelpers.TryGetLGOAPDomainsToGenerateCodeFor(blackboardData.ReadOnly(), out var lgoapDomains))
            {
                success = false;
            }

            if (!success)
            {
                blackboardTemplates = new (string path, BlackboardTemplate template)[0];
                lgoapDomains = new (string path, LGOAPDomain domain)[0];
            }

            var generatedCode = new List<(string path, string contents)>();

            // generate all code as strings
            foreach (var (path, template) in blackboardTemplates)
            {
                generatedCode.Add((path, template.allGeneratedCode));
            }

            foreach (var (path, domain) in lgoapDomains)
            {
                generatedCode.Add((path, domain.allGeneratedCode));
            }

            // write all c# code
            var generatedFiles = new string[generatedCode.Count];
            for (var i = 0; i < generatedCode.Count; i++)
            {
                var (path, contents) = generatedCode[i];
                generatedFiles[i] = path;

                EditorSerializationUtility.GenerateCode(path, contents);
            }

            // generate manifest
            EditorSerializationUtility.CleanupAndGenerateManifest(string.Join("\n", generatedFiles));

            // import new files
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Only allow code generation if in Edit mode.
        /// </summary>
        [MenuItem("Assets/Generate HiraBots Code", true, priority = 800)]
        private static bool AllowCodeGeneration()
        {
            return !EditorApplication.isPlayingOrWillChangePlaymode;
        }
    }
}