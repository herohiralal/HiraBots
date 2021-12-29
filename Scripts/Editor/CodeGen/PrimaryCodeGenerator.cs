using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    /// <summary>
    /// The main entry point for primary code generation
    /// </summary>
    internal static class PrimaryCodeGenerator
    {
        /// <summary>
        /// Generate code for all HiraBots objects.
        /// </summary>
        [MenuItem("Assets/Generate HiraBots Code", false, priority = 800)]
        private static void GenerateHiraBotsCode()
        {
            // create folder/asmdef
            EditorSerializationUtility.ConfirmCodeGenFolder();
            EditorSerializationUtility.CreateCodeGenAssemblyDefinition();

            var invalidatedHiraBotsObjects = new List<string>();

            CookingHelpers.TryGetBlackboardTemplatesToGenerateCodeFor(out var blackboardTemplates, invalidatedHiraBotsObjects);

            var blackboardData = new Dictionary<BlackboardTemplate, ReadOnlyHashSetAccessor<BlackboardKey>>();

            foreach (var (_, template) in blackboardTemplates)
            {
                var hs = new HashSet<BlackboardKey>();
                template.GetKeySet(hs);
                blackboardData.Add(template, hs.ReadOnly());
            }

            CookingHelpers.TryGetLGOAPDomainsToGenerateCodeFor(blackboardData.ReadOnly(), out var lgoapDomains, invalidatedHiraBotsObjects);

            var generatedCode = new List<(string path, string contents)>();

            if (invalidatedHiraBotsObjects.Count > 0)
            {
                var s = "The following HiraBots objects could not be validated, and so they will not be generated.\n\n";

                foreach (var invalidatedHiraBotsObject in invalidatedHiraBotsObjects)
                {
                    s += $"{invalidatedHiraBotsObject}\n";
                    generatedCode.Add((invalidatedHiraBotsObject, "// the HiraBots object associated with this file could not be validated"));
                }

                Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, s);
            }

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
            EditorSerializationUtility.CleanupAndGenerateManifest("hirabots_objects", generatedFiles);

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