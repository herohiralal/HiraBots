namespace HiraBots
{
    /// <summary>
    /// Helper class for serialization functionalities.
    /// It's not meant to be instantiated, it's just a collection of helper functions.
    /// It's not made static to allow inheritance.
    /// </summary>
    internal abstract class SerializationUtility
    {
        // The overall folder structure is as so:
        // In the editor, cooked objects are stored inside: <project-path>/Temp/HiraBots/
        // In the build, cooked objects are stored inside: .../Resources/HiraBots/
        // Since Unity abstracts away the folder-path before Resources, it is of no concern to this class.

        protected const string k_MainSubfolderName = "HiraBots";

#if UNITY_EDITOR
        protected const string k_TempFolderName = "Temp";
#endif

        /// <summary>
        /// Load a cooked <see cref="CookedDataSingleton{T}"/>, wherever it might be.
        /// </summary>
        /// <typeparam name="T">The type of object to load.</typeparam>
        /// <returns>The cooked object.</returns>
        internal static T LoadCookedData<T>() where T : CookedDataSingleton<T>
        {
            var fileName = CookedDataSingleton<T>.fileName;

#if UNITY_EDITOR // load from temp editor-only folder

            var tempPath = FileNameToTempEditorPath(fileName);
            var serializedAssets = UnityEditorInternal.InternalEditorUtility
                .LoadSerializedFileAndForget(tempPath);

            return serializedAssets.Length > 0 ? (T) serializedAssets[0] : null;

#else // use Resources.Load in the build

            return UnityEngine.Resources.Load<T>(FileNameToResourcesRelativePath(fileName));

#endif
        }

        /// <summary>
        /// Unload a cooked <see cref="CookedDataSingleton{T}"/>.
        /// </summary>
        /// <param name="value">The cooked object.</param>
        /// <typeparam name="T">The type of object to unload.</typeparam>
        internal static void UnloadCookedData<T>(T value) where T : CookedDataSingleton<T>
        {
#if UNITY_EDITOR
            UnityEngine.Object.Destroy(value);
#else
            UnityEngine.Resources.UnloadAsset(value);
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// Convert file name to an address in the temporary editor folder.
        /// </summary>
        protected static string FileNameToTempEditorPath(string name)
        {
            return k_TempFolderName + "/" + k_MainSubfolderName + "/" + name + ".asset";
        }
#endif

        /// <summary>
        /// Convert a file name to an address in the Resources folder.
        /// </summary>
        protected static string FileNameToResourcesRelativePath(string name)
        {
            return k_MainSubfolderName + "/" + name;
        }
    }
}