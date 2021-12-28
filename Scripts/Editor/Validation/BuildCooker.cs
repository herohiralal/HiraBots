using System.Collections.Generic;
using UnityEngine;

namespace HiraBots.Editor
{
    /// <summary>
    /// This class is responsible for cooking objects into the temporary build folder before
    /// everything else, and failing the build, should this process fail.
    /// </summary>
    internal class BuildCooker : UnityEditor.Build.IPreprocessBuildWithReport, UnityEditor.Build.IPostprocessBuildWithReport
    {
        public int callbackOrder => -1;

        public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
        {
            try
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

                    throw new UnityEditor.Build.BuildFailedException("One or more HiraBots objects have failed to compile. " +
                                                                     "You cannot build until they are fixed. Optionally, you " +
                                                                     "can also set their backends to none.");
                }

                // cook the collections into the temporary build folder
                EditorSerializationUtility.CookToTempBuildFolderAndForget(ref templateCollection);
                EditorSerializationUtility.CookToTempBuildFolderAndForget(ref domainCollection);
            }
            catch (System.Exception e)
            {
                throw new UnityEditor.Build.BuildFailedException(e);
            }
        }

        public void OnPostprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
        {
            // delete the temporary build folder
            EditorSerializationUtility.DeleteTempBuildFolder();
        }
    }
}