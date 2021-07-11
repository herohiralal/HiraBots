using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

namespace HiraBots.Editor.Tests
{
    [TestFixture]
    internal class EditorCookedDataTest
    {
        [UnityTest]
        public IEnumerator SerializationAndDeserializationWorksCorrectly()
        {
            var cookedData = ScriptableObject.CreateInstance<CookedDataTestObject>();
            cookedData.hideFlags = HideFlags.HideAndDontSave;

            cookedData.value = 342;

            EditorSerializationUtility.ConfirmTempEditorFolder();
            EditorSerializationUtility.CookToTempEditorFolderAndForget(ref cookedData);

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                throw new AssertionException("Need to close the scene in order to run the test.");

            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            yield return new EnterPlayMode();

            Assert.AreEqual(342, CookedDataTestObject.Instance.value);
        }
    }
}