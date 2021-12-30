namespace HiraBots.Editor
{
    /// <summary>
    /// Helper class for serialization functionalities within the editor.
    /// It's not meant to be instantiated, it's just a collection of helper functions.
    /// It's not made static to allow inheritance.
    /// </summary>
    internal abstract partial class EditorSerializationUtility : SerializationUtility
    {
        // The overall folder structure is as so:
        // 
        // In the editor, cooked objects are stored inside: <project-path>/Temp/HiraBots/
        // The base class is aware of the editor-only temporary folder.
        // 
        // In the build, cooked objects are stored inside: <project-path>/Assets/HiraBotsBuildArtifacts/Resources/HiraBots/
        // The base class is not aware of the complete folder structure, and is only concerned with .../Resources/HiraBots/

        protected static readonly string s_ProjectDirectoryA = System.IO.Path.GetDirectoryName(UnityEngine.Application.dataPath);

        protected const string k_AssetsFolderName = "Assets";
        protected const string k_PackagesFolderName = "Packages";

        protected const string k_PackageName = "com.rohanjadav.hirabots";
        protected const string k_PackageScriptsFolderName = "Scripts";

        protected const string k_HiraBotsBuildArtifactsFolderName = "HiraBotsBuildArtifacts";
        protected const string k_ResourcesFolderName = "Resources";

        protected const string k_BuildArtifactsFolderNameR = k_AssetsFolderName + "/" + k_HiraBotsBuildArtifactsFolderName;

        protected const string k_BuildArtifactsResourcesFolderNameR = k_BuildArtifactsFolderNameR + "/" + k_ResourcesFolderName + "/" + k_MainSubfolderName;

        protected const string k_CodeGenFolderName = "_HiraBots.CodeGen";
        protected const string k_CodeGenAssemblyName = "HiraBots.CodeGen";
        protected const string k_CodeGenManualExtensionsFolderName = "ManualExtensions";

        protected const string k_InternalCodeGenFolderName = "CodeGen";
        protected const string k_InternalCodeGenAssemblyName = "HiraBots.Generated";

        /// <summary>
        /// Confirm that a relative path within a directory exists.
        /// </summary>
        private static string ConfirmDirectoryExistsAndGetAbsolutePath(string directory, string pathR)
        {
            // create directory as required
            var subfolders = pathR.Split('/');
            var current = directory;
            for (var i = 0; i < subfolders.Length - 1; i++) // skip over the last element, which is the file name
            {
                current = System.IO.Path.Combine(current, subfolders[i]);

                if (!System.IO.Directory.Exists(current))
                    System.IO.Directory.CreateDirectory(current);
            }

            // append filename & extension
            var path = System.IO.Path.Combine(current, subfolders[subfolders.Length - 1]);
            return path;
        }
    }
}