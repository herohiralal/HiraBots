﻿using NUnit.Framework;
using UnityEngine.TestTools;

namespace HiraBots.Editor.Tests
{
    [TestFixture]
    internal class CookedDataTest : IPrebuildSetup, IPostBuildCleanup
    {
        [Test]
        public void SerializationAndDeserializationWorksCorrectly()
        {
            Assert.AreEqual(CookedDataTestObject.VALUE_TO_COOK, CookedDataTestObject.Instance.value);
        }

        [Test]
        public void BlackboardTemplateCollectionIsCooked()
        {
            Assert.IsTrue(BlackboardTemplateCollection.Instance != null, "BlackboardTemplateCollection must be cooked into play mode or build.");
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