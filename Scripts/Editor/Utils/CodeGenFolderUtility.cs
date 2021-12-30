using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HiraBots.Editor
{
    internal abstract partial class EditorSerializationUtility
    {
        #region Folder Confirmation

        /// <summary>
        /// Confirm the existence of the temporary editor-only folder.
        /// </summary>
        internal static void ConfirmCodeGenFolder()
        {
            var directory = Path.Combine(s_ProjectDirectoryA, k_AssetsFolderName, k_CodeGenFolderName);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            directory = Path.Combine(directory, k_CodeGenManualExtensionsFolderName);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        #endregion

        /// <summary>
        /// Create codegen asmdef.
        /// </summary>
        internal static void CreateCodeGenAssemblyDefinition()
        {
            var path = Path.Combine(s_ProjectDirectoryA, k_AssetsFolderName, k_CodeGenFolderName, k_CodeGenAssemblyName + ".asmdef");

            var text = CodeGenHelpers.ReadTemplate("AssemblyDefinition", ("<CODEGEN-ASSEMBLY-NAME>", k_CodeGenAssemblyName));

            File.WriteAllText(path, text);

            path = Path.Combine(s_ProjectDirectoryA, k_AssetsFolderName, k_CodeGenFolderName,
                k_CodeGenManualExtensionsFolderName, "info.txt");

            File.WriteAllText(path,
                "" +
                "// ===================================================\n" +
                "// Feel free to extend partial classes in this folder.\n" +
                "// ===================================================\n");
        }

        /// <summary>
        /// Save generated code to a relative path within the codegen folder.
        /// </summary>
        internal static void GenerateCode(string pathR, string contents, string guid = null)
        {
            var directory = Path.Combine(s_ProjectDirectoryA, k_AssetsFolderName, k_CodeGenFolderName);

            var path = ConfirmDirectoryExistsAndGetAbsolutePath(directory, pathR);

            // write
            File.WriteAllText(path, contents);

            if (guid != null)
            {
                File.WriteAllText(path + ".meta", GetMonoImporterMetaFile(guid));
            }
        }

        /// <summary>
        /// Get a default meta file given a GUID.
        /// </summary>
        private static string GetMonoImporterMetaFile(string guid)
        {
            return "" +
                   "fileFormatVersion: 2\n" +
                   $"guid: {guid}\n" +
                   "MonoImporter:\n" +
                   "  externalObjects: {}\n" +
                   "  serializedVersion: 2\n" +
                   "  defaultReferences: []\n" +
                   "  executionOrder: 0\n" +
                   "  icon: {instanceID: 0}\n" +
                   "  userData: \n" +
                   "  assetBundleName: \n" +
                   "  assetBundleVariant: \n";
        }

        /// <summary>
        /// Clean up orphaned files/folders and write a manifest for all the generated files.
        /// </summary>
        internal static void CleanupAndGenerateManifest(string manifestName, string[] generatedFiles)
        {
            System.Array.Resize(ref generatedFiles, generatedFiles.Length + 2);
            generatedFiles[generatedFiles.Length - 2] = $"{k_CodeGenManualExtensionsFolderName}/info.txt";
            generatedFiles[generatedFiles.Length - 1] = $"{k_CodeGenAssemblyName}.asmdef";

            var directory = Path.Combine(s_ProjectDirectoryA, k_AssetsFolderName, k_CodeGenFolderName);
            var manifestLocation = Path.Combine(directory, $"{manifestName}_manifest.txt");

            var previouslyWrittenFiles = !File.Exists(manifestLocation) ? new string[0] : File.ReadAllLines(manifestLocation);

            // write manifest
            File.WriteAllText(manifestLocation, GenerateNewManifest(directory, previouslyWrittenFiles, generatedFiles));
        }

        /// <summary>
        /// Generates a new manifest and cleans up useless old files.
        /// </summary>
        private static string GenerateNewManifest(string targetDirectory, string[] previouslyWrittenFiles, string[] generatedFiles)
        {
            var potentiallyUselessFolders = new HashSet<string>();

            // remove useless files
            foreach (var uselessFile in previouslyWrittenFiles.Where(s => !generatedFiles.Contains(s)))
            {
                UnityEngine.Debug.LogFormat(UnityEngine.LogType.Log, UnityEngine.LogOption.NoStacktrace, null,
                    $"Remove orphaned file: {uselessFile}");
                var actualFile = Path.Combine(targetDirectory, uselessFile);

                if (File.Exists(actualFile))
                {
                    File.Delete(actualFile);
                }

                if (File.Exists(actualFile + ".meta"))
                {
                    File.Delete(actualFile + ".meta");
                }

                // get the folder hierarchy of the file and mark all those folders as potentially useless
                var subfolders = uselessFile.Split('/');
                var current = "";
                for (var i = 0; i < subfolders.Length - 1; i++) // skip over the last element, which is the file name
                {
                    current += subfolders[i] + "/";
                    potentiallyUselessFolders.Add(current);
                }
            }

            var manifestContent = string.Join("\n", generatedFiles);

            // remove useless folders
            foreach (var uselessFolder in potentiallyUselessFolders.Where(folder => !manifestContent.Contains(folder)))
            {
                UnityEngine.Debug.LogFormat(UnityEngine.LogType.Log, UnityEngine.LogOption.NoStacktrace, null,
                    $"Remove orphaned folder: {uselessFolder}");
                var actualFolder = Path.Combine(targetDirectory, uselessFolder);

                if (Directory.Exists(actualFolder))
                {
                    Directory.Delete(actualFolder);
                }

                var metaFile = actualFolder.TrimEnd('/') + ".meta";
                if (File.Exists(metaFile))
                {
                    File.Delete(metaFile);
                }
            }

            return manifestContent;
        }
    }
}