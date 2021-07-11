using UnityEditor;

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
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                // todo: cook blackboard templates into a scriptable object
            }
            // acquire all blackboard templates
            // check if the data within them is valid
            // if it isn't,
            // EditorApplication.isPlaying = false;
        }
    }
}