namespace HiraBots.Editor.Tests
{
    internal partial class CookedDataTestObject : CookedDataSingleton<CookedDataTestObject>
    {
        [UnityEngine.SerializeField] internal int value = 0;
        internal const int k_ValueToCook = 342;
    }
}

#if UNITY_EDITOR

namespace HiraBots.Editor.Tests
{
    using UnityEditor;

    internal partial class CookedDataTestObject
    {
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
            EditorApplication.playModeStateChanged += OnPlayModeStateChange;
        }

        private static void OnPlayModeStateChange(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingEditMode) return;

            CreateCookedData(out var cookedData);
            cookedData.hideFlags = UnityEngine.HideFlags.HideAndDontSave;

            EditorSerializationUtility.ConfirmTempEditorFolder();
            EditorSerializationUtility.CookToTempEditorFolderAndForget(ref cookedData);
        }

        internal static void CreateCookedData(out CookedDataTestObject cookedData)
        {
            cookedData = CreateInstance<CookedDataTestObject>();
            cookedData.value = k_ValueToCook;
        }
    }
}

#endif