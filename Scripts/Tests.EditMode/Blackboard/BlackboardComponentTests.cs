using System.Linq;
using NUnit.Framework;
using UnityEngine;
using static HiraBots.BlackboardComponent;
using Object = UnityEngine.Object;

namespace HiraBots.Editor.Tests
{
    [TestFixture]
    internal class BlackboardComponentTests : BlackboardAccessTestBase
    {
        [OneTimeSetUp]
        public new void SetUp() => base.SetUp();

        [OneTimeTearDown]
        public new void TearDown() => base.TearDown();

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

                Assert.IsTrue(TryCreate(BaseCharacterTemplate, out component) && component != null,
                    "Failed to create a base character.");

                Assert.IsTrue(TryCreate(WarriorTemplate, out component) && component != null,
                    "Failed to create a warrior character.");

                Assert.IsTrue(TryCreate(MageTemplate, out component) && component != null,
                    "Failed to create a mage character.");

                Assert.IsTrue(TryCreate(ElementalistTemplate, out component) && component != null,
                    "Failed to create a elementalist character.");
            }
            finally
            {
                Object.DestroyImmediate(nonCompilingBlackboard);
            }
        }

        [Test]
        public void SimpleReadWriteValidation()
        {
            TryCreate(ElementalistTemplate, out var baboon);
            var elementalistKeyData = ElementalistData.MemoryOffsetToKeyData;

            baboon.SetIntegerValueWithoutValidation(elementalistKeyData[LevelKeyInteger], 3, true);
            Assert.IsTrue(3 == baboon.GetIntegerValueWithoutValidation(LevelKeyInteger), "Integer read-write failed.");

            baboon.SetFloatValueWithoutValidation(elementalistKeyData[HealthKeyFloat], 35f, true);
            Assert.IsTrue(Mathf.Abs(35f - baboon.GetFloatValueWithoutValidation(HealthKeyFloat)) < 0.5f, "Float read-write failed.");

            baboon.SetBooleanValueWithoutValidation(elementalistKeyData[HealthLowKeyBoolean], true, true);
            Assert.IsTrue(baboon.GetBooleanValueWithoutValidation(HealthLowKeyBoolean), "Boolean read-write failed.");

            baboon.SetObjectValueWithoutValidation(elementalistKeyData[PlayerReferenceKeyObject], MockObject2, true);
            Assert.IsTrue(MockObject2 == baboon.GetObjectValueWithoutValidation(PlayerReferenceKeyObject), "Object read-write failed.");

            var q = Quaternion.Euler(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
            baboon.SetQuaternionValueWithoutValidation(elementalistKeyData[ThrowKeyQuaternion], q, true);
            Assert.IsTrue(q == baboon.GetQuaternionValueWithoutValidation(ThrowKeyQuaternion), "Quaternion read-write failed.");

            baboon.SetEnumValueWithoutValidation<GenericStatus>(elementalistKeyData[HealthStatusKeyEnum], GenericStatus.Low, true);
            Assert.IsTrue(GenericStatus.Low == baboon.GetEnumValueWithoutValidation<GenericStatus>(HealthStatusKeyEnum), "Enum read-write failed.");

            Assert.IsFalse(baboon.HasUnexpectedChanges, "Expected changes still lead to dirtying.");

            TryCreate(ElementalistTemplate, out var secondBaboon);

            baboon.SetIntegerValueWithoutValidation(elementalistKeyData[ElementalPowerKeyInteger], 34);
            Assert.IsTrue(34 == secondBaboon.GetIntegerValueWithoutValidation(ElementalPowerKeyInteger), "Instance syncing failed.");

            TryCreate(WarriorTemplate, out var knight);
            var warriorKeyData = WarriorData.MemoryOffsetToKeyData;

            knight.SetFloatValueWithoutValidation(warriorKeyData[StaminaKeyFloat], 1f);
            Assert.IsTrue(knight.HasUnexpectedChanges, "Simple dirtying failed.");
            Assert.IsTrue(knight.UnexpectedChanges
                .Contains(StaminaKeyFloat), "Simple dirtying index validation failed.");

            TryCreate(MageTemplate, out var wizard);
            var mageKeyData = MageData.MemoryOffsetToKeyData;

            wizard.SetFloatValueWithoutValidation(mageKeyData[ManaKeyFloat], 0f);
            Assert.IsTrue(wizard.HasUnexpectedChanges, "Could not test for overwriting with the same value dirtying the blackboard.");
            wizard.ClearUnexpectedChanges();

            wizard.SetFloatValueWithoutValidation(mageKeyData[ManaKeyFloat], 0f);
            Assert.IsFalse(wizard.HasUnexpectedChanges, "Overwriting with same value dirtied the blackboard.");
        }

        [Test]
        public void InstanceSyncValidation()
        {
            TryCreate(BaseCharacterTemplate, out var civilian);
            var baseCharacterKeyData = BaseCharacterData.MemoryOffsetToKeyData;

            TryCreate(WarriorTemplate, out var knight);

            civilian.SetVectorValueWithoutValidation(baseCharacterKeyData[CurrentPlayerLocationKeyVector], Vector3.right);
            Assert.AreEqual(Vector3.right, knight.GetVectorValueWithoutValidation(CurrentPlayerLocationKeyVector),
                "Instance syncing failed between base and warrior.");

            TryCreate(MageTemplate, out var wizard);

            Assert.AreEqual(Vector3.right, wizard.GetVectorValueWithoutValidation(CurrentPlayerLocationKeyVector),
                "Instance sync failed for a newly created key.");
        }

        [Test]
        public void BlackboardDirtyingValidation()
        {
            TryCreate(BaseCharacterTemplate, out var civilian);
            var baseCharacterKeyData = BaseCharacterData.MemoryOffsetToKeyData;

            civilian.SetVectorValueWithoutValidation(baseCharacterKeyData[CurrentPlayerLocationKeyVector], Vector3.right);

            Assert.IsTrue(civilian.HasUnexpectedChanges, "Simple dirtying test failed.");
            Assert.IsTrue(civilian.UnexpectedChanges
                .Contains(CurrentPlayerLocationKeyVector), "Simple dirtying test index validation failed.");

            civilian.ClearUnexpectedChanges();

            Assert.IsFalse(civilian.HasUnexpectedChanges, "Clearing dirtying test failed.");

            TryCreate(ElementalistTemplate, out var baboon);
            civilian.SetVectorValueWithoutValidation(baseCharacterKeyData[CurrentPlayerLocationKeyVector], Vector3.zero, true);

            Assert.IsFalse(civilian.HasUnexpectedChanges, "Expectation parameter test failed.");
            Assert.IsTrue(baboon.HasUnexpectedChanges, "Instance synced dirtying test failed.");
            Assert.IsTrue(baboon.UnexpectedChanges
                .Contains(CurrentPlayerLocationKeyVector), "Simple dirtying test index validation failed.");
        }
    }
}