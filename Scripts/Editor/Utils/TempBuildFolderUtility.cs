using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    /// <summary>
    /// Editor-only serialization utility with regards to the temporary build folder.
    /// The folder gets created before the build and gets deleted after the build succeeds/fails.
    /// </summary>
    internal abstract partial class EditorSerializationUtility
    {
        #region Folder Confirmation

        /// <summary>
        /// Confirm the existence of the temporary build-only folder.
        /// </summary>
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

        /// <summary>
        /// Convert a file name to an address in the temporary build-only folder.
        /// </summary>
        protected static string FileNameToTempBuildPath(string fileName)
        {
            return k_BuildArtifactsResourcesFolderNameR + "/" + fileName + ".asset";
        }

        /// <summary>
        /// Cook a <see cref="CookedDataSingleton{T}"/> into the temporary build-only folder.
        /// </summary>
        /// <param name="target">The object to cook. Passed as reference, so the reference can be nullified right after.</param>
        internal static void CookToTempBuildFolderAndForget<T>(ref T target) where T : CookedDataSingleton<T>
        {
            AssetDatabase.CreateAsset(target, FileNameToTempBuildPath(CookedDataSingleton<T>.fileName));

            Resources.UnloadAsset(target);
            target = null;
        }

        /// <summary>
        /// Delete a <see cref="CookedDataSingleton{T}"/> from the temporary build-only folder.
        /// </summary>
        internal static void DeleteFromTempBuildFolder<T>() where T : CookedDataSingleton<T>
        {
            AssetDatabase.DeleteAsset(FileNameToTempBuildPath(CookedDataSingleton<T>.fileName));
        }

        /// <summary>
        /// Delete the temporary build-only folder.
        /// </summary>
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