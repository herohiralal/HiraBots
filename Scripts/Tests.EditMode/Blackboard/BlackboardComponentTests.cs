using System.Linq;
using NUnit.Framework;
using UnityEngine;
using static HiraBots.BlackboardComponent;
using Object = UnityEngine.Object;

namespace HiraBots.Editor.Tests
{
    /// <summary>
    /// Tests to check the public API for BlackboardComponent.
    /// </summary>
    [TestFixture]
    internal class BlackboardComponentTests : BlackboardAccessTestBase
    {
        [OneTimeSetUp]
        public new void SetUp()
        {
            base.SetUp();
        }

        [OneTimeTearDown]
        public new void TearDown()
        {
            base.TearDown();
        }

        /// <summary>
        /// Validate blackboard creations from templates.
        /// </summary>
        [Test]
        public void CreationValidation()
        {
            var nonCompilingBlackboard = BlackboardTemplate.Build<BlackboardTemplate>("NonCompilingBlackboardTemplate",
                null, new BlackboardKey[1], HideFlags.HideAndDontSave);

            try
            {
                Assert.IsTrue(!TryCreate(null, out var component) && component == null,
                    "Created a component without template.");

                Assert.IsTrue(!TryCreate(nonCompilingBlackboard, out component) && component == null,
                    "Created a component with a non-compiling template.");

                Assert.IsTrue(TryCreate(m_BaseCharacterTemplate, out component) && component != null,
                    "Failed to create a base character.");

                Assert.IsTrue(TryCreate(m_WarriorTemplate, out component) && component != null,
                    "Failed to create a warrior character.");

                Assert.IsTrue(TryCreate(m_MageTemplate, out component) && component != null,
                    "Failed to create a mage character.");

                Assert.IsTrue(TryCreate(m_ElementalistTemplate, out component) && component != null,
                    "Failed to create a elementalist character.");
            }
            finally
            {
                Object.DestroyImmediate(nonCompilingBlackboard);
            }
        }

        /// <summary>
        /// Validate read/write on blackboard components.
        /// </summary>
        [Test]
        public void SimpleReadWriteValidation()
        {
            TryCreate(m_ElementalistTemplate, out var baboon);
            var elementalistKeyData = elementalistData.memoryOffsetToKeyData;

            baboon.SetIntegerValueWithoutValidation(elementalistKeyData[levelKeyInteger], 3, true);
            Assert.IsTrue(3 == baboon.GetIntegerValueWithoutValidation(levelKeyInteger), "Integer read-write failed.");

            baboon.SetFloatValueWithoutValidation(elementalistKeyData[healthKeyFloat], 35f, true);
            Assert.IsTrue(Mathf.Abs(35f - baboon.GetFloatValueWithoutValidation(healthKeyFloat)) < 0.5f, "Float read-write failed.");

            baboon.SetBooleanValueWithoutValidation(elementalistKeyData[healthLowKeyBoolean], true, true);
            Assert.IsTrue(baboon.GetBooleanValueWithoutValidation(healthLowKeyBoolean), "Boolean read-write failed.");

            baboon.SetObjectValueWithoutValidation(elementalistKeyData[playerReferenceKeyObject], mockObject2, true);
            Assert.IsTrue(mockObject2 == baboon.GetObjectValueWithoutValidation(playerReferenceKeyObject), "Object read-write failed.");

            var q = Quaternion.Euler(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
            baboon.SetQuaternionValueWithoutValidation(elementalistKeyData[throwKeyQuaternion], q, true);
            Assert.IsTrue(q == baboon.GetQuaternionValueWithoutValidation(throwKeyQuaternion), "Quaternion read-write failed.");

            baboon.SetEnumValueWithoutValidation<GenericStatus>(elementalistKeyData[healthStatusKeyEnum], GenericStatus.Low, true);
            Assert.IsTrue(GenericStatus.Low == baboon.GetEnumValueWithoutValidation<GenericStatus>(healthStatusKeyEnum), "Enum read-write failed.");

            Assert.IsFalse(baboon.hasUnexpectedChanges, "Expected changes still lead to dirtying.");

            TryCreate(m_ElementalistTemplate, out var secondBaboon);

            baboon.SetIntegerValueWithoutValidation(elementalistKeyData[elementalPowerKeyInteger], 34);
            Assert.IsTrue(34 == secondBaboon.GetIntegerValueWithoutValidation(elementalPowerKeyInteger), "Instance syncing failed.");

            TryCreate(m_WarriorTemplate, out var knight);
            var warriorKeyData = warriorData.memoryOffsetToKeyData;

            knight.SetFloatValueWithoutValidation(warriorKeyData[staminaKeyFloat], 1f);
            Assert.IsTrue(knight.hasUnexpectedChanges, "Simple dirtying failed.");
            Assert.IsTrue(knight.unexpectedChanges
                .Contains(staminaKeyFloat), "Simple dirtying index validation failed.");

            TryCreate(m_MageTemplate, out var wizard);
            var mageKeyData = mageData.memoryOffsetToKeyData;

            wizard.SetFloatValueWithoutValidation(mageKeyData[manaKeyFloat], 0f);
            Assert.IsTrue(wizard.hasUnexpectedChanges, "Could not test for overwriting with the same value dirtying the blackboard.");
            wizard.ClearUnexpectedChanges();

            wizard.SetFloatValueWithoutValidation(mageKeyData[manaKeyFloat], 0f);
            Assert.IsFalse(wizard.hasUnexpectedChanges, "Overwriting with same value dirtied the blackboard.");
        }

        /// <summary>
        /// Validate instance synchronization.
        /// </summary>
        [Test]
        public void InstanceSyncValidation()
        {
            TryCreate(m_BaseCharacterTemplate, out var civilian);
            var baseCharacterKeyData = baseCharacterData.memoryOffsetToKeyData;

            TryCreate(m_WarriorTemplate, out var knight);

            civilian.SetVectorValueWithoutValidation(baseCharacterKeyData[currentPlayerLocationKeyVector], Vector3.right);
            Assert.AreEqual(Vector3.right, knight.GetVectorValueWithoutValidation(currentPlayerLocationKeyVector),
                "Instance syncing failed between base and warrior.");

            TryCreate(m_MageTemplate, out var wizard);

            Assert.AreEqual(Vector3.right, wizard.GetVectorValueWithoutValidation(currentPlayerLocationKeyVector),
                "Instance sync failed for a newly created key.");
        }

        /// <summary>
        /// Validate dirtying of the blackboard.
        /// </summary>
        [Test]
        public void BlackboardDirtyingValidation()
        {
            TryCreate(m_BaseCharacterTemplate, out var civilian);
            var baseCharacterKeyData = baseCharacterData.memoryOffsetToKeyData;

            civilian.SetVectorValueWithoutValidation(baseCharacterKeyData[currentPlayerLocationKeyVector], Vector3.right);

            Assert.IsTrue(civilian.hasUnexpectedChanges, "Simple dirtying test failed.");
            Assert.IsTrue(civilian.unexpectedChanges
                .Contains(currentPlayerLocationKeyVector), "Simple dirtying test index validation failed.");

            civilian.ClearUnexpectedChanges();

            Assert.IsFalse(civilian.hasUnexpectedChanges, "Clearing dirtying test failed.");

            TryCreate(m_ElementalistTemplate, out var baboon);
            civilian.SetVectorValueWithoutValidation(baseCharacterKeyData[currentPlayerLocationKeyVector], Vector3.zero, true);

            Assert.IsFalse(civilian.hasUnexpectedChanges, "Expectation parameter test failed.");
            Assert.IsTrue(baboon.hasUnexpectedChanges, "Instance synced dirtying test failed.");
            Assert.IsTrue(baboon.unexpectedChanges
                .Contains(currentPlayerLocationKeyVector), "Simple dirtying test index validation failed.");
        }
    }
}