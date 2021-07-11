using UnityEditor;

namespace HiraBots.Editor
{
    [InitializeOnLoad]
    public static class PlayModeEnterValidator
    {
        static PlayModeEnterValidator()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
            EditorApplication.playModeStateChanged += OnPlayModeStateChange;
        }

        private static void OnPlayModeStateChange(PlayModeStateChange state)
        {
            // acquire all blackboard templates
            // check if the data within them is valid
            // if it isn't,
            // EditorApplication.isPlaying = false;
        }
    }
}