using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
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
            if (state != PlayModeStateChange.ExitingEditMode) return;

            if (!CookingHelpers.TryGenerateBlackboardTemplateCollection(out var result))
            {
                Debug.LogError($"One or more blackboard templates have failed to compile. " +
                               "You cannot enter play mode until they are fixed.");
                EditorApplication.isPlaying = false;
            }
            else
            {
                result.hideFlags = HideFlags.HideAndDontSave;
                EditorSerializationUtility.ConfirmTempEditorFolder();
                EditorSerializationUtility.CookToTempEditorFolderAndForget(ref result);
            }
        }
    }
}