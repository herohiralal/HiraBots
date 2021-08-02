using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    internal partial class EditorSerializationUtility : SerializationUtility
    {
        #region Folder Confirmation

        protected const string k_AssetsFolderName = "Assets";
        protected const string k_HiraBotsBuildArtifactsFolderName = "HiraBotsBuildArtifacts";
        protected const string k_ResourcesFolderName = "Resources";

        protected const string k_BuildArtifactsFolderNameR = k_AssetsFolderName + "/" + k_HiraBotsBuildArtifactsFolderName;

        protected const string k_BuildArtifactsResourcesFolderNameR = k_BuildArtifactsFolderNameR + "/" + k_ResourcesFolderName + "/" + k_MainSubfolderName;

        internal static void ConfirmTempBuildFolder()
        {
            if (!AssetDatabase.IsValidFolder(k_BuildArtifactsFolderNameR))
            {
                AssetDatabase.CreateFolder(k_AssetsFolderName, k_HiraBotsBuildArtifactsFolderName);
            }

            if (!AssetDatabase.IsValidFolder(k_BuildArtifactsFolderNameR + "/" + k_ResourcesFolderName))
            {
                AssetDatabase.CreateFolder(k_BuildArtifactsFolderNameR, k_ResourcesFolderName);
            }

            if (!AssetDatabase.IsValidFolder(k_BuildArtifactsResourcesFolderNameR))
            {
                AssetDatabase.CreateFolder(k_BuildArtifactsFolderNameR + "/" + k_ResourcesFolderName,
                    k_MainSubfolderName);
            }
        }

        #endregion

        protected static string FileNameToTempBuildPath(string fileName) =>
            k_BuildArtifactsResourcesFolderNameR + "/" + fileName + ".asset";

        internal static void CookToTempBuildFolderAndForget<T>(ref T target) where T : CookedDataSingleton<T>
        {
            AssetDatabase.CreateAsset(target, FileNameToTempBuildPath(CookedDataSingleton<T>.fileName));

            Resources.UnloadAsset(target);
            target = null;
        }

        internal static void DeleteFromTempBuildFolder<T>() where T : CookedDataSingleton<T> =>
            AssetDatabase.DeleteAsset(FileNameToTempBuildPath(CookedDataSingleton<T>.fileName));

        internal static void DeleteTempBuildFolder()
        {
            if (AssetDatabase.IsValidFolder(k_BuildArtifactsResourcesFolderNameR))
                AssetDatabase.DeleteAsset(k_BuildArtifactsResourcesFolderNameR);

            if (AssetDatabase.IsValidFolder(k_BuildArtifactsFolderNameR + "/" + k_ResourcesFolderName))
                AssetDatabase.DeleteAsset(k_BuildArtifactsFolderNameR + "/" + k_ResourcesFolderName);

            if (AssetDatabase.IsValidFolder(k_BuildArtifactsFolderNameR))
                AssetDatabase.DeleteAsset(k_BuildArtifactsFolderNameR);
        }
    }
}