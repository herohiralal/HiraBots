using System.Linq;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using static HiraBots.BlackboardComponent;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace HiraBots.Editor.Tests
{
    /// <summary>
    /// Tests to check the public API for BlackboardComponent.
    /// </summary>
    [TestFixture]
    internal class BlackboardComponentTests : BlackboardAccessTestBase
    {
        [OneTimeSetUp]
        public virtual void SetUp()
        {
            base.SetUp(false);
        }

        [OneTimeTearDown]
        public virtual void TearDown()
        {
            base.TearDown(false);
        }

        /// <summary>
        /// Validate blackboard creations from templates.
        /// </summary>
        [Test]
        public void CreationValidation()
        {
            var nonCompilingBlackboard = "NonCompilingBlackboardTemplate".BuildScriptableObject<BlackboardTemplate>();
            nonCompilingBlackboard.BuildBlackboardTemplate(null, BackendType.RuntimeInterpreter, new BlackboardKey[1]);

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

            var levelKeyIntegerData = elementalistData[k_LevelKeyInteger];
            baboon.SetIntegerValueWithoutValidation(levelKeyIntegerData, 3, true);
            Assert.IsTrue(3 == baboon.GetIntegerValueWithoutValidation(levelKeyIntegerData.memoryOffset), "Integer read-write failed.");

            var healthKeyFloatData = elementalistData[k_HealthKeyFloat];
            baboon.SetFloatValueWithoutValidation(healthKeyFloatData, 35f, true);
            Assert.IsTrue(Mathf.Abs(35f - baboon.GetFloatValueWithoutValidation(healthKeyFloatData.memoryOffset)) < 0.5f, "Float read-write failed.");

            var healthLowKeyBooleanData = elementalistData[k_HealthLowKeyBoolean];
            baboon.SetBooleanValueWithoutValidation(healthLowKeyBooleanData, true, true);
            Assert.IsTrue(baboon.GetBooleanValueWithoutValidation(healthLowKeyBooleanData.memoryOffset), "Boolean read-write failed.");

            var playerReferenceKeyObjectData = elementalistData[k_PlayerReferenceKeyObject];
            baboon.SetObjectValueWithoutValidation(playerReferenceKeyObjectData, mockObject2, true);
            Assert.IsTrue(mockObject2 == baboon.GetObjectValueWithoutValidation(playerReferenceKeyObjectData.memoryOffset), "Object read-write failed.");

            var q = Quaternion.Euler(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
            var throwKeyQuaternionData = elementalistData[k_ThrowKeyQuaternion];
            baboon.SetQuaternionValueWithoutValidation(throwKeyQuaternionData, q, true);
            Assert.IsTrue(q == baboon.GetQuaternionValueWithoutValidation(throwKeyQuaternionData.memoryOffset), "Quaternion read-write failed.");

            var healthStatusKeyEnumData = elementalistData[k_HealthStatusKeyEnum];
            baboon.SetEnumValueWithoutValidation<GenericStatus>(healthStatusKeyEnumData, GenericStatus.Low, true);
            Assert.IsTrue(GenericStatus.Low == baboon.GetEnumValueWithoutValidation<GenericStatus>(healthStatusKeyEnumData.memoryOffset), "Enum read-write failed.");

            Assert.IsFalse(baboon.hasUnexpectedChanges, "Expected changes still lead to dirtying.");

            TryCreate(m_ElementalistTemplate, out var secondBaboon);

            var elementalPowerKeyIntegerData = elementalistData[k_ElementalPowerKeyInteger];
            baboon.SetIntegerValueWithoutValidation(elementalPowerKeyIntegerData, 34);
            Assert.IsTrue(34 == secondBaboon.GetIntegerValueWithoutValidation(elementalPowerKeyIntegerData.memoryOffset), "Instance syncing failed.");

            TryCreate(m_WarriorTemplate, out var knight);

            var staminaKeyFloatData = warriorData[k_StaminaKeyFloat];
            knight.SetFloatValueWithoutValidation(staminaKeyFloatData, 1f);
            Assert.IsTrue(knight.hasUnexpectedChanges, "Simple dirtying failed.");
            Assert.IsTrue(knight.unexpectedChanges
                .Contains(staminaKeyFloatData.keyName), "Simple dirtying index validation failed.");

            TryCreate(m_MageTemplate, out var wizard);

            var manaKeyFloatData = mageData[k_ManaKeyFloat];
            wizard.SetFloatValueWithoutValidation(manaKeyFloatData, 0f);
            Assert.IsTrue(wizard.hasUnexpectedChanges, "Could not test for overwriting with the same value dirtying the blackboard.");
            wizard.ClearUnexpectedChanges();

            wizard.SetFloatValueWithoutValidation(manaKeyFloatData, 0f);
            Assert.IsFalse(wizard.hasUnexpectedChanges, "Overwriting with same value dirtied the blackboard.");
        }

        /// <summary>
        /// Validate instance synchronization.
        /// </summary>
        [Test]
        public void InstanceSyncValidation()
        {
            TryCreate(m_BaseCharacterTemplate, out var civilian);
            var currentPlayerLocationKeyVectorData = baseCharacterData[k_CurrentPlayerLocationKeyVector];
            var currentPlayerLocationKeyVectorMemoryOffset = currentPlayerLocationKeyVectorData.memoryOffset;

            TryCreate(m_WarriorTemplate, out var knight);

            civilian.SetVectorValueWithoutValidation(currentPlayerLocationKeyVectorData, new float3(1, 0, 0));
            Assert.AreEqual(new float3(1, 0, 0), knight.GetVectorValueWithoutValidation(currentPlayerLocationKeyVectorMemoryOffset),
                "Instance syncing failed between base and warrior.");

            TryCreate(m_MageTemplate, out var wizard);

            Assert.AreEqual(new float3(1, 0, 0), wizard.GetVectorValueWithoutValidation(currentPlayerLocationKeyVectorMemoryOffset),
                "Instance sync failed for a newly created key.");
        }

        /// <summary>
        /// Validate dirtying of the blackboard.
        /// </summary>
        [Test]
        public void BlackboardDirtyingValidation()
        {
            TryCreate(m_BaseCharacterTemplate, out var civilian);
            var currentPlayerLocationKeyVectorData = baseCharacterData[k_CurrentPlayerLocationKeyVector];
            var currentPlayerLocationKeyVectorKeyName = currentPlayerLocationKeyVectorData.keyName;

            civilian.SetVectorValueWithoutValidation(currentPlayerLocationKeyVectorData, new float3(1, 0, 0));

            Assert.IsTrue(civilian.hasUnexpectedChanges, "Simple dirtying test failed.");
            Assert.IsTrue(civilian.unexpectedChanges
                .Contains(currentPlayerLocationKeyVectorKeyName), "Simple dirtying test index validation failed.");

            civilian.ClearUnexpectedChanges();

            Assert.IsFalse(civilian.hasUnexpectedChanges, "Clearing dirtying test failed.");

            TryCreate(m_ElementalistTemplate, out var baboon);
            civilian.SetVectorValueWithoutValidation(currentPlayerLocationKeyVectorData, float3.zero, true);

            Assert.IsFalse(civilian.hasUnexpectedChanges, "Expectation parameter test failed.");
            Assert.IsTrue(baboon.hasUnexpectedChanges, "Instance synced dirtying test failed.");
            Assert.IsTrue(baboon.unexpectedChanges
                .Contains(currentPlayerLocationKeyVectorKeyName), "Simple dirtying test index validation failed.");
        }
    }
}