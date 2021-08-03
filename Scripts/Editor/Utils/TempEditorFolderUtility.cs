using System.IO;
using UnityEditorInternal;
using UnityEngine;

namespace HiraBots.Editor
{
    internal abstract partial class EditorSerializationUtility
    {
        #region Folder Confirmation

        protected static readonly string s_ProjectDirectoryA = Path.GetDirectoryName(Application.dataPath);

        /// <summary>
        /// Confirm the existence of the temporary editor-only folder.
        /// </summary>
        internal static void ConfirmTempEditorFolder()
        {
            var directory = Path.Combine(s_ProjectDirectoryA, k_TempFolderName, k_MainSubfolderName);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        #endregion

        /// <summary>
        /// Cook a <see cref="CookedDataSingleton{T}"/> into the temporary editor-only folder.
        /// </summary>
        /// <param name="target">The object to cook. Passed as reference, so the reference can be nullified right after.</param>
        internal static void CookToTempEditorFolderAndForget<T>(ref T target) where T : CookedDataSingleton<T>
        {
            InternalEditorUtility.SaveToSerializedFileAndForget(new Object[] {target},
                FileNameToTempEditorPath(CookedDataSingleton<T>.fileName), true);

            Object.DestroyImmediate(target, false);
            target = null;
        }
    }
}