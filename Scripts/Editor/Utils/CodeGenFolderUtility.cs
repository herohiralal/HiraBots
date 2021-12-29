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
        internal static void GenerateCode(string pathR, string contents)
        {
            var directory = Path.Combine(s_ProjectDirectoryA, k_AssetsFolderName, k_CodeGenFolderName);

            // create directory as required
            var subfolders = pathR.Split('/');
            var current = directory;
            for (var i = 0; i < subfolders.Length - 1; i++) // skip over the last element, which is the file name
            {
                current = Path.Combine(current, subfolders[i]);

                if (!Directory.Exists(current))
                    Directory.CreateDirectory(current);
            }

            // append filename & extension
            var path = Path.Combine(current, subfolders[subfolders.Length - 1]);

            // write
            File.WriteAllText(path, contents);
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

            var potentiallyUselessFolders = new HashSet<string>();

            // remove useless files
            foreach (var uselessFile in previouslyWrittenFiles.Where(s => !generatedFiles.Contains(s)))
            {
                UnityEngine.Debug.LogFormat(UnityEngine.LogType.Log, UnityEngine.LogOption.NoStacktrace, null,
                    $"Remove orphaned file: {uselessFile}");
                var actualFile = Path.Combine(directory, uselessFile);

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

            // remove useless folders
            foreach (var uselessFolder in potentiallyUselessFolders.Where(folder => generatedFiles.All(filePath => !filePath.Contains(folder))))
            {
                UnityEngine.Debug.LogFormat(UnityEngine.LogType.Log, UnityEngine.LogOption.NoStacktrace, null,
                    $"Remove orphaned folder: {uselessFolder}");
                var actualFolder = Path.Combine(directory, uselessFolder);

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

            // write manifest
            File.WriteAllText(manifestLocation, string.Join("\n", generatedFiles));
        }
    }
}