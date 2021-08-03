namespace HiraBots.Editor.Tests
{
    /// <summary>
    /// An object to test out CookedDataSingleton.
    /// </summary>
    internal partial class CookedDataTestObject : CookedDataSingleton<CookedDataTestObject>
    {
        [UnityEngine.SerializeField] internal int m_Value = 0;
        internal const int k_ValueToCook = 342;
    }
}

#if UNITY_EDITOR

namespace HiraBots.Editor.Tests
{
    using UnityEditor;

    internal partial class CookedDataTestObject
    {
        // bind events to also cook CookedDataTestObject into the play-mode.
        [InitializeOnLoadMethod]
        private static void Initialize()
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

            CreateCookedData(out var cookedData);
            cookedData.hideFlags = UnityEngine.HideFlags.HideAndDontSave;

            EditorSerializationUtility.ConfirmTempEditorFolder();
            EditorSerializationUtility.CookToTempEditorFolderAndForget(ref cookedData);
        }

        /// <summary>
        /// Create an instance of a <see cref="CookedDataTestObject"/>.
        /// </summary>
        /// <param name="cookedData"></param>
        internal static void CreateCookedData(out CookedDataTestObject cookedData)
        {
            cookedData = CreateInstance<CookedDataTestObject>();
            cookedData.m_Value = k_ValueToCook;
        }
    }
}

#endif