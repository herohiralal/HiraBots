using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    internal partial class EditorSerializationUtility : SerializationUtility
    {
        #region Folder Confirmation

        protected const string ASSETS_FOLDER_NAME = "Assets";
        protected const string HIRA_BOTS_BUILD_ARTIFACTS_FOLDER_NAME = "HiraBotsBuildArtifacts";
        protected const string RESOURCES_FOLDER_NAME = "Resources";

        protected const string BUILD_ARTIFACTS_FOLDER_NAME_R = ASSETS_FOLDER_NAME + "/" + HIRA_BOTS_BUILD_ARTIFACTS_FOLDER_NAME;

        protected const string BUILD_ARTIFACTS_RESOURCES_FOLDER_NAME_R = BUILD_ARTIFACTS_FOLDER_NAME_R + "/" + RESOURCES_FOLDER_NAME + "/" + MAIN_SUBFOLDER_NAME;

        internal static void ConfirmTempBuildFolder()
        {
            if (!AssetDatabase.IsValidFolder(BUILD_ARTIFACTS_FOLDER_NAME_R))
            {
                AssetDatabase.CreateFolder(ASSETS_FOLDER_NAME, HIRA_BOTS_BUILD_ARTIFACTS_FOLDER_NAME);
            }

            if (!AssetDatabase.IsValidFolder(BUILD_ARTIFACTS_FOLDER_NAME_R + "/" + RESOURCES_FOLDER_NAME))
            {
                AssetDatabase.CreateFolder(BUILD_ARTIFACTS_FOLDER_NAME_R, RESOURCES_FOLDER_NAME);
            }

            if (!AssetDatabase.IsValidFolder(BUILD_ARTIFACTS_RESOURCES_FOLDER_NAME_R))
            {
                AssetDatabase.CreateFolder(BUILD_ARTIFACTS_FOLDER_NAME_R + "/" + RESOURCES_FOLDER_NAME,
                    MAIN_SUBFOLDER_NAME);
            }
        }

        #endregion

        protected static string FileNameToTempBuildPath(string fileName) =>
            BUILD_ARTIFACTS_RESOURCES_FOLDER_NAME_R + "/" + fileName + ".asset";

        internal static void CookToTempBuildFolderAndForget<T>(ref T target) where T : CookedDataSingleton<T>
        {
            AssetDatabase.CreateAsset(target, FileNameToTempBuildPath(CookedDataSingleton<T>.FileName));

            Resources.UnloadAsset(target);
            target = null;
        }
    }
}