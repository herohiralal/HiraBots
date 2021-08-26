using UnityEditor;

namespace HiraBots.Editor.Tests.ImportedFromPlayMode
{
    internal static class TestBlackboardTemplateCreation
    {
        [MenuItem("Assets/Create/HiraBots/Blackboard (Test)", false)]
        private static void CreateTestBlackboard()
        {
            AssetDatabaseUtility.CreateNewObject<TestBlackboardTemplate>("NewBlackboard");
        }

        [MenuItem("Assets/Create/HiraBots/Blackboard (Test)", true)]
        private static bool CanCreateTestBlackboard()
        {
            return !EditorApplication.isPlayingOrWillChangePlaymode;
        }
    }
}