using NUnit.Framework;
using UnityEngine.TestTools;

namespace HiraBots.Editor.Tests
{
    [TestFixture]
    internal class CookedDataTests : IPrebuildSetup, IPostBuildCleanup
    {
        [Test]
        public void SerializationAndDeserializationWorksCorrectly()
        {
            Assert.AreEqual(CookedDataTestObject.VALUE_TO_COOK, CookedDataTestObject.Instance.value);
        }

        [Test]
        public void BlackboardTemplateCollectionIsCooked()
        {
            var btc = BlackboardTemplateCollection.Instance;
            Assert.IsTrue(btc != null, "BlackboardTemplateCollection must be cooked into play mode or build.");

            for (var i = 1; i < btc.Count; i++)
                Assert.IsTrue(btc[i].HierarchyIndex >= btc[i - 1].HierarchyIndex, "Blackboard template collection built without proper sorting.");
        }

        public void Setup()
        {
#if UNITY_EDITOR
            CookedDataTestObject.CreateCookedData(out var data);
            
            EditorSerializationUtility.ConfirmTempBuildFolder();
            EditorSerializationUtility.CookToTempBuildFolderAndForget(ref data);
#endif
        }

        public void Cleanup()
        {
#if UNITY_EDITOR
            EditorSerializationUtility.DeleteFromTempBuildFolder<CookedDataTestObject>();
            EditorSerializationUtility.DeleteTempBuildFolder();
#endif
        }
    }
}