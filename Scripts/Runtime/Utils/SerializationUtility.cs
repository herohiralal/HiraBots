namespace HiraBots
{
    internal class SerializationUtility
    {
        protected const string k_MainSubfolderName = "HiraBots";

#if UNITY_EDITOR
        protected const string k_TempFolderName = "Temp";
#endif

        internal static T LoadCookedData<T>(string fileName) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            var tempPath = FileNameToTempEditorPath(fileName);
            var serializedAssets = UnityEditorInternal.InternalEditorUtility
                .LoadSerializedFileAndForget(tempPath);

            return serializedAssets.Length > 0 ? (T) serializedAssets[0] : null;
#else
            return UnityEngine.Resources.Load<T>(FileNameToResourcesRelativePath(fileName));
#endif
        }

#if UNITY_EDITOR
        protected static string FileNameToTempEditorPath(string name) =>
            k_TempFolderName + "/" + k_MainSubfolderName + "/" + name + ".asset";
#endif

        protected static string FileNameToResourcesRelativePath(string name) =>
            k_MainSubfolderName + "/" + name;
    }
}