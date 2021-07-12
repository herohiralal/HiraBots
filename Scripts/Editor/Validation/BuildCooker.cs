using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace HiraBots.Editor
{
    internal class BuildCooker : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder => -1;

        public void OnPreprocessBuild(BuildReport report)
        {
            EditorSerializationUtility.ConfirmTempBuildFolder();
            var templateCollection = CookingHelpers.TryGenerateBlackboardTemplateCollection(out var result)
                ? result
                : throw new BuildFailedException("One or more blackboard templates have failed to compile.");

            EditorSerializationUtility.CookToTempBuildFolderAndForget(ref templateCollection);
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            EditorSerializationUtility.DeleteTempBuildFolder();
        }
    }
}