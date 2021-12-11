using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

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
            var success = true;

            EditorSerializationUtility.ConfirmTempBuildFolder();

            // validate the blackboards and generate the template collection
            if (!CookingHelpers.TryGenerateInterpretedBlackboardTemplateCollection(out var templateCollection))
            {
                success = false;
            }

            var blackboardData = new Dictionary<BlackboardTemplate, ReadOnlyHashSetAccessor<BlackboardKey>>();

            for (var i = 0; i < templateCollection.count; i++)
            {
                var hs = new HashSet<BlackboardKey>();
                templateCollection[i].GetKeySet(hs);
                blackboardData.Add(templateCollection[i], hs.ReadOnly());
            }

            if (!CookingHelpers.TryGenerateInterpretedLGOAPDomainCollection(blackboardData.ReadOnly(), out var domainCollection))
            {
                success = false;
            }

            if (!success)
            {
                Object.DestroyImmediate(templateCollection);
                Object.DestroyImmediate(domainCollection);

                throw new BuildFailedException("One or more HiraBots objects have failed to compile. " +
                                               "You cannot build until they are fixed. Optionally, you " +
                                               "can also set their backends to none.");
            }

            // cook the collections into the temporary build folder
            EditorSerializationUtility.CookToTempBuildFolderAndForget(ref templateCollection);
            EditorSerializationUtility.CookToTempBuildFolderAndForget(ref domainCollection);
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            // delete the temporary build folder
            EditorSerializationUtility.DeleteTempBuildFolder();
        }
    }
}