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
        [MenuItem("Assets/Generate HiraBots Code", false, priority = 800)]
        private static void GenerateHiraBotsCode()
        {
            var templateGenerationSuccessful = CookingHelpers.TryGetBlackboardTemplatesToGenerateCodeFor(out var blackboardTemplates);

            if (!templateGenerationSuccessful)
            {
                Debug.LogError("Failed to generate code.");
                return;
            }

            EditorSerializationUtility.ConfirmCodeGenFolder();
            EditorSerializationUtility.CreateCodeGenAssemblyDefinition();

            var generatedCode = new List<(string path, string contents)>();

            foreach (var (path, template) in blackboardTemplates)
            {
                generatedCode.Add((path, template.allGeneratedCode));
            }

            var generatedFiles = new string[generatedCode.Count];
            for (var i = 0; i < generatedCode.Count; i++)
            {
                var (path, contents) = generatedCode[i];
                generatedFiles[i] = path;

                EditorSerializationUtility.GenerateCode(path, contents);
            }

            EditorSerializationUtility.CleanupAndGenerateManifest(string.Join("\n", generatedFiles));

            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Generate HiraBots Code", true, priority = 800)]
        private static bool AllowCodeGeneration()
        {
            return !EditorApplication.isPlayingOrWillChangePlaymode;
        }
    }
}