using NUnit.Framework;
using UnityEngine.TestTools;

namespace HiraBots.Editor.Tests
{
    /// <summary>
    /// Tests to validate the data-cooking functionality.
    /// </summary>
    [TestFixture]
    internal class CookedDataTests : IPrebuildSetup, IPostBuildCleanup
    {
        /// <summary>
        /// Confirm correct serialization in CookedDataTestObject.
        /// </summary>
        [Test]
        public void SerializationAndDeserializationWorksCorrectly()
        {
            Assert.AreEqual(CookedDataTestObject.k_ValueToCook, CookedDataTestObject.instance.m_Value);
        }

        /// <summary>
        /// Confirm the existence of template collection instance, be it in play mode or in the build.
        /// Also validate the sorting order in the template collection.
        /// </summary>
        [Test]
        public void BlackboardTemplateCollectionIsCooked()
        {
            var btc = BlackboardTemplateCollection.instance;
            Assert.IsTrue(btc != null, "BlackboardTemplateCollection must be cooked into play mode or build.");

#if UNITY_EDITOR // hierarchy index is not accessible in a build
            for (var i = 1; i < btc.count; i++)
            {
                Assert.IsTrue(btc[i].hierarchyIndex >= btc[i - 1].hierarchyIndex, "Blackboard template collection built without proper sorting.");
            }
#endif
        }

        public void Setup()
        {
#if UNITY_EDITOR // cook the CookedDataTestObject into the build.
            CookedDataTestObject.CreateCookedData(out var data);

            EditorSerializationUtility.ConfirmTempBuildFolder();
            EditorSerializationUtility.CookToTempBuildFolderAndForget(ref data);
#endif
        }

        public void Cleanup()
        {
#if UNITY_EDITOR // remove the CookedDataTestObject.
            EditorSerializationUtility.DeleteFromTempBuildFolder<CookedDataTestObject>();
#endif
        }
    }
}