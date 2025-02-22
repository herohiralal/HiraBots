using System.IO;

namespace HiraBots.Editor
{
    internal abstract partial class EditorSerializationUtility
    {
        #region Folder Confirmation

        /// <summary>
        /// Confirm the existence of the temporary editor-only folder.
        /// </summary>
        internal static void ConfirmInternalCodeGenFolder()
        {
            var directory = Path.Combine(s_ProjectDirectoryA,
                k_PackagesFolderName,
                k_PackageName,
                k_PackageScriptsFolderName,
                k_InternalCodeGenFolderName);

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
        internal static void CreateInternalCodeGenAssemblyDefinition()
        {
            var path = Path.Combine(s_ProjectDirectoryA,
                k_PackagesFolderName,
                k_PackageName,
                k_PackageScriptsFolderName,
                k_InternalCodeGenFolderName,
                k_InternalCodeGenAssemblyName + ".asmdef");

            File.WriteAllText(path, CodeGenHelpers.ReadTemplate("AssemblyDefinition",
                ("<CODEGEN-ASSEMBLY-NAME>", k_InternalCodeGenAssemblyName)));
        }

        /// <summary>
        /// Save generated code to a relative path within the codegen folder.
        /// </summary>
        internal static void GenerateInternalCode(string pathR, string contents, string guid = null)
        {
            var directory = Path.Combine(s_ProjectDirectoryA,
                k_PackagesFolderName,
                k_PackageName,
                k_PackageScriptsFolderName,
                k_InternalCodeGenFolderName);

            var path = ConfirmDirectoryExistsAndGetAbsolutePath(directory, pathR);

            // write
            File.WriteAllText(path, contents);

            if (guid != null)
            {
                File.WriteAllText(path + ".meta", GetMonoImporterMetaFile(guid));
            }
        }

        /// <summary>
        /// Clean up orphaned files/folders and write a manifest for all the generated files.
        /// </summary>
        internal static void CleanupAndGenerateManifestForInternalCode(string manifestName, string[] generatedFiles)
        {
            System.Array.Resize(ref generatedFiles, generatedFiles.Length + 1);
            generatedFiles[generatedFiles.Length - 1] = $"{k_InternalCodeGenAssemblyName}.asmdef";

            var directory = Path.Combine(s_ProjectDirectoryA,
                k_PackagesFolderName,
                k_PackageName,
                k_PackageScriptsFolderName,
                k_InternalCodeGenFolderName);
            var manifestLocation = Path.Combine(directory, $"{manifestName}_manifest.txt");

            var previouslyWrittenFiles = !File.Exists(manifestLocation) ? new string[0] : File.ReadAllLines(manifestLocation);

            // write manifest
            File.WriteAllText(manifestLocation, GenerateNewManifest(directory, previouslyWrittenFiles, generatedFiles));
        }
    }
}