using System.Linq;
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

            var validator = new BlackboardValidator();

            var templatesToCook = CookingHelpers.LoadBlackboardTemplatesToCook().ToArray();

            var result = true;

            foreach (var template in templatesToCook)
            {
                if (!validator.Execute(template, out var errorText))
                {
                    Debug.LogError(errorText, template);
                    result = false;
                }
            }

            if (!result)
            {
                Debug.LogError($"One or more blackboard templates have failed to compile. " +
                               $"You cannot enter play mode until they are fixed.");
                EditorApplication.isPlaying = false;
                return;
            }

            var templatesCollection = BlackboardTemplateCollection.Create(templatesToCook);
                
            EditorSerializationUtility.ConfirmTempEditorFolder();
            EditorSerializationUtility.CookToTempEditorFolderAndForget(ref templatesCollection);
        }
    }
}