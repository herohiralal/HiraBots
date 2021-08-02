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
            Assert.AreEqual(CookedDataTestObject.k_ValueToCook, CookedDataTestObject.instance.m_Value);
        }

        [Test]
        public void BlackboardTemplateCollectionIsCooked()
        {
            var btc = BlackboardTemplateCollection.instance;
            Assert.IsTrue(btc != null, "BlackboardTemplateCollection must be cooked into play mode or build.");

            for (var i = 1; i < btc.count; i++)
                Assert.IsTrue(btc[i].hierarchyIndex >= btc[i - 1].hierarchyIndex, "Blackboard template collection built without proper sorting.");
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