using System.IO;
using UnityEditorInternal;
using UnityEngine;

namespace HiraBots.Editor
{
    internal partial class EditorSerializationUtility
    {
        #region Folder Confirmation

        protected static readonly string s_ProjectDirectoryA = Path.GetDirectoryName(Application.dataPath);

        internal static void ConfirmTempEditorFolder()
        {
            var directory = Path.Combine(s_ProjectDirectoryA, k_TempFolderName, k_MainSubfolderName);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        #endregion

        internal static void CookToTempEditorFolderAndForget<T>(ref T target) where T : CookedDataSingleton<T>
        {
            InternalEditorUtility.SaveToSerializedFileAndForget(new Object[] {target},
                FileNameToTempEditorPath(CookedDataSingleton<T>.fileName), true);

            Object.DestroyImmediate(target, false);
            target = null;
        }
    }
}