using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    /// <summary>
    /// This class is responsible for cooking objects into the temporary editor folder before
    /// entering play mode, and disabling access to play mode, should this process fail.
    /// </summary>
    [InitializeOnLoad]
    internal static class PlayModeEnterValidator
    {
        static PlayModeEnterValidator()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
            EditorApplication.playModeStateChanged += OnPlayModeStateChange;
        }

        private static void OnPlayModeStateChange(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingEditMode)
            {
                return;
            }

            EditorSerializationUtility.ConfirmTempEditorFolder();

            // validate the blackboards and generate the template collection
            if (!CookingHelpers.TryGenerateBlackboardTemplateCollection(out var result))
            {
                Debug.LogError($"One or more blackboard templates have failed to compile. " +
                               "You cannot enter play mode until they are fixed.");
                EditorApplication.isPlaying = false;
            }
            else
            {
                result.hideFlags = HideFlags.HideAndDontSave;

                // cook the collection into the temporary build folder
                EditorSerializationUtility.CookToTempEditorFolderAndForget(ref result);
            }
        }
    }
}