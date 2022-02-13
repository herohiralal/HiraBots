using System.Collections.Generic;
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

            try
            {
                EditorUtility.DisplayProgressBar("Validating HiraBots Components", "Validating Blackboard Templates...", 0.1f);

                var success = true;

                EditorSerializationUtility.ConfirmTempEditorFolder();

                // validate the blackboards and generate the template collection
                if (!CookingHelpers.TryGenerateInterpretedBlackboardTemplateCollection(out var templateCollection))
                {
                    success = false;
                }

                EditorUtility.DisplayProgressBar("Validating HiraBots Components", "Cooking Blackboard Templates Collection...", 0.5f);

                var blackboardData = new Dictionary<BlackboardTemplate, ReadOnlyHashSetAccessor<BlackboardKey>>();

                for (var i = 0; i < templateCollection.count; i++)
                {
                    var hs = new HashSet<BlackboardKey>();
                    templateCollection[i].GetKeySet(hs);
                    blackboardData.Add(templateCollection[i], hs.ReadOnly());
                }

                templateCollection.hideFlags = HideFlags.HideAndDontSave;

                EditorUtility.DisplayProgressBar("Validating HiraBots Components", "Validating LGOAP Domains...", 0.6f);

                // cook the collection into the temporary build folder
                EditorSerializationUtility.CookToTempEditorFolderAndForget(ref templateCollection);

                if (!CookingHelpers.TryGenerateInterpretedLGOAPDomainCollection(blackboardData.ReadOnly(), out var domainCollection))
                {
                    success = false;
                }

                EditorUtility.DisplayProgressBar("Validating HiraBots Components", "Cooking LGOAP Domains Collection...", 1.0f);

                domainCollection.hideFlags = HideFlags.HideAndDontSave;

                // cook the collection into the temporary editor folder
                EditorSerializationUtility.CookToTempEditorFolderAndForget(ref domainCollection);

                if (!success)
                {
                    Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, null,
                        "One or more HiraBots objects have failed to compile. " +
                        "You cannot enter play mode until they are fixed. Optionally, you " +
                        "can also set their backends to none.");
                    EditorApplication.isPlaying = false;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                EditorApplication.isPlaying = false;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}