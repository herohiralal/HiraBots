using System.IO;
using UnityEditorInternal;
using UnityEngine;

namespace HiraBots.Editor
{
    internal partial class EditorSerializationUtility
    {
        #region Folder Confirmation

        protected static readonly string PROJECT_DIRECTORY_A = Path.GetDirectoryName(Application.dataPath);

        internal static void ConfirmTempEditorFolder()
        {
            var directory = Path.Combine(PROJECT_DIRECTORY_A, TEMP_FOLDER_NAME, MAIN_SUBFOLDER_NAME);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        #endregion

        internal static void CookToTempEditorFolderAndForget<T>(ref T target) where T : CookedDataSingleton<T>
        {
            InternalEditorUtility.SaveToSerializedFileAndForget(new Object[] {target},
                FileNameToTempEditorPath(CookedDataSingleton<T>.FileName), true);

            target = null;
        }
    }
}