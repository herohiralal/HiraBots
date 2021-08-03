using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace HiraBots.Editor
{
    /// <summary>
    /// This class is responsible for cooking objects into the temporary build folder before
    /// everything else, and failing the build, should this process fail.
    /// </summary>
    internal class BuildCooker : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder => -1;

        public void OnPreprocessBuild(BuildReport report)
        {
            EditorSerializationUtility.ConfirmTempBuildFolder();

            // validate the blackboards and generate the template collection
            var templateCollection = CookingHelpers.TryGenerateBlackboardTemplateCollection(out var result)
                ? result
                : throw new BuildFailedException("One or more blackboard templates have failed to compile.");

            // cook the collection into the temporary build folder
            EditorSerializationUtility.CookToTempBuildFolderAndForget(ref templateCollection);
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            // delete the temporary build folder
            EditorSerializationUtility.DeleteTempBuildFolder();
        }
    }
}