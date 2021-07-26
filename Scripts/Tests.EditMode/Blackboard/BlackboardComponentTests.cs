using NUnit.Framework;
using UnityEngine;
using static HiraBots.BlackboardComponent;
using Object = UnityEngine.Object;

namespace HiraBots.Editor.Tests
{
    [ExposedToHiraBots("5F9A54C1-F247-41B0-8D5E-D64DD26C8317")]
    internal enum GenericStatus : byte
    {
        Empty,
        VeryLow,
        Low,
        Medium,
        High,
        VeryHigh,
        Max
    }

    [TestFixture]
    internal class BlackboardComponentTests
    {
        private BlackboardTemplate _baseCharacterTemplate = null;
        private BlackboardTemplateCompiledData BaseCharacterData => _baseCharacterTemplate.CompiledData;

        private BlackboardTemplate _warriorTemplate = null;
        private BlackboardTemplateCompiledData WarriorData => _warriorTemplate.CompiledData;

        private BlackboardTemplate _mageTemplate = null;
        private BlackboardTemplateCompiledData MageData => _mageTemplate.CompiledData;

        private BlackboardTemplate _elementalistTemplate = null;
        private BlackboardTemplateCompiledData ElementalistData => _elementalistTemplate.CompiledData;

        private ScriptableObject _mockObject1 = null;
        private ScriptableObject _mockObject2 = null;
        private ScriptableObject _mockObject3 = null;

        private ushort LevelKeyInteger => BaseCharacterData["Level"].MemoryOffset;
        private ushort HealthKeyFloat => BaseCharacterData["Health"].MemoryOffset;
        private ushort HealthLowKeyBoolean => BaseCharacterData["HealthLow"].MemoryOffset;
        private ushort CurrentPlayerLocationKeyVector => BaseCharacterData["CurrentPlayerLocation"].MemoryOffset;
        private ushort PlayerReferenceKeyObject => BaseCharacterData["PlayerReference"].MemoryOffset;
        private ushort HealthStatusKeyEnum => BaseCharacterData["HealthStatus"].MemoryOffset;
        private ushort StaminaKeyFloat => WarriorData["Stamina"].MemoryOffset;
        private ushort ManaKeyFloat => MageData["Mana"].MemoryOffset;
        private ushort ElementalPowerKeyInteger => ElementalistData["ElementalPower"].MemoryOffset;
        private ushort ThrowKeyQuaternion => ElementalistData["Throw"].MemoryOffset;

        [OneTimeSetUp]
        public void SetUp()
        {
            _mockObject1 = ScriptableObject.CreateInstance<ScriptableObject>();
            _mockObject1.hideFlags = HideFlags.HideAndDontSave;
            _mockObject2 = ScriptableObject.CreateInstance<ScriptableObject>();
            _mockObject2.hideFlags = HideFlags.HideAndDontSave;
            _mockObject3 = ScriptableObject.CreateInstance<ScriptableObject>();
            _mockObject3.hideFlags = HideFlags.HideAndDontSave;

            _baseCharacterTemplate = Resources.Load<BlackboardTemplate>("TestBaseCharacter");
            _warriorTemplate = Resources.Load<BlackboardTemplate>("TestWarrior");
            _mageTemplate = Resources.Load<BlackboardTemplate>("TestMage");
            _elementalistTemplate = Resources.Load<BlackboardTemplate>("TestElementalist");

            CheckLoaded(_baseCharacterTemplate, _warriorTemplate, _mageTemplate, _elementalistTemplate);
            Validate(_baseCharacterTemplate, _warriorTemplate, _mageTemplate, _elementalistTemplate);
            Compile(_baseCharacterTemplate, _warriorTemplate, _mageTemplate, _elementalistTemplate);
        }

        private static void CheckLoaded(params BlackboardTemplate[] templates)
        {
            foreach (var template in templates) Assert.IsTrue(template != null, "Template could not be loaded.");
        }

        private static void Validate(params BlackboardTemplate[] templates)
        {
            var validator = new BlackboardTemplateValidatorContext();

            foreach (var template in templates)
            {
                template.Validate(validator);
                Assert.IsTrue(validator.Validated, $"{template.name} could not be validated.");
                validator.Reset();
            }
        }

        private static void Compile(params BlackboardTemplate[] templates)
        {
            var compiler = new BlackboardTemplateCompilerContext();

            foreach (var template in templates)
            {
                template.Compile(compiler);
                Assert.IsTrue(template.IsCompiled, $"{template.name} could not be compiled.");
                compiler.Update();
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Free(_elementalistTemplate, _mageTemplate, _warriorTemplate, _baseCharacterTemplate);
            Unload(_elementalistTemplate, _mageTemplate, _warriorTemplate, _baseCharacterTemplate);

            _baseCharacterTemplate = _warriorTemplate = _mageTemplate = _elementalistTemplate = null;

            Object.DestroyImmediate(_mockObject3);
            Object.DestroyImmediate(_mockObject2);
            Object.DestroyImmediate(_mockObject1);
        }

        private static void Free(params BlackboardTemplate[] templates)
        {
            foreach (var template in templates)
                if (template != null)
                    template.Free();
        }

        private static void Unload(params BlackboardTemplate[] templates)
        {
            foreach (var template in templates)
                if (template != null)
                    Resources.UnloadAsset(template);
        }

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

                Assert.IsTrue(TryCreate(_baseCharacterTemplate, out component) && component != null,
                    "Failed to create a base character.");

                Assert.IsTrue(TryCreate(_warriorTemplate, out component) && component != null,
                    "Failed to create a warrior character.");

                Assert.IsTrue(TryCreate(_mageTemplate, out component) && component != null,
                    "Failed to create a mage character.");

                Assert.IsTrue(TryCreate(_elementalistTemplate, out component) && component != null,
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
            TryCreate(_elementalistTemplate, out var baboon);

            baboon.SetIntegerValueWithoutValidation(LevelKeyInteger, 3, true);
            Assert.IsTrue(3 == baboon.GetIntegerValueWithoutValidation(LevelKeyInteger), "Integer read-write failed.");

            baboon.SetFloatValueWithoutValidation(HealthKeyFloat, 35f, true);
            Assert.IsTrue(Mathf.Abs(35f - baboon.GetFloatValueWithoutValidation(HealthKeyFloat)) < 0.5f, "Float read-write failed.");

            baboon.SetBooleanValueWithoutValidation(HealthLowKeyBoolean, true, true);
            Assert.IsTrue(baboon.GetBooleanValueWithoutValidation(HealthLowKeyBoolean), "Boolean read-write failed.");

            baboon.SetObjectValueWithoutValidation(PlayerReferenceKeyObject, _mockObject2, true);
            Assert.IsTrue(_mockObject2 == baboon.GetObjectValueWithoutValidation(PlayerReferenceKeyObject), "Object read-write failed.");

            var q = Quaternion.Euler(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
            baboon.SetQuaternionValueWithoutValidation(ThrowKeyQuaternion, q, true);
            Assert.IsTrue(q == baboon.GetQuaternionValueWithoutValidation(ThrowKeyQuaternion), "Quaternion read-write failed.");

            baboon.SetEnumValueWithoutValidation<GenericStatus>(HealthStatusKeyEnum, GenericStatus.Low, true);
            Assert.IsTrue(GenericStatus.Low == baboon.GetEnumValueWithoutValidation<GenericStatus>(HealthStatusKeyEnum), "Enum read-write failed.");

            Assert.IsFalse(baboon.HasUnexpectedChanges, "Expected changes still lead to dirtying.");

            TryCreate(_elementalistTemplate, out var secondBaboon);

            baboon.SetIntegerValueWithoutValidation(ElementalPowerKeyInteger, 34);
            Assert.IsTrue(34 == secondBaboon.GetIntegerValueWithoutValidation(ElementalPowerKeyInteger), "Instance syncing failed.");

            TryCreate(_warriorTemplate, out var knight);
            knight.SetFloatValueWithoutValidation(StaminaKeyFloat, 1f);
            Assert.IsTrue(knight.HasUnexpectedChanges, "Simple dirtying failed.");

            TryCreate(_mageTemplate, out var wizard);
            wizard.SetFloatValueWithoutValidation(ManaKeyFloat, 0f);
            Assert.IsTrue(wizard.HasUnexpectedChanges, "Could not test for overwriting with the same value dirtying the blackboard.");
            wizard.ClearUnexpectedChanges();

            wizard.SetFloatValueWithoutValidation(ManaKeyFloat, 0f);
            Assert.IsFalse(wizard.HasUnexpectedChanges, "Overwriting with same value dirtied the blackboard.");
        }

        [Test]
        public void InstanceSyncValidation()
        {
            TryCreate(_baseCharacterTemplate, out var civilian);
            TryCreate(_warriorTemplate, out var knight);

            civilian.SetVectorValueWithoutValidation(CurrentPlayerLocationKeyVector, Vector3.right);
            Assert.AreEqual(Vector3.right, knight.GetVectorValueWithoutValidation(CurrentPlayerLocationKeyVector),
                "Instance syncing failed between base and warrior.");

            TryCreate(_mageTemplate, out var wizard);

            Assert.AreEqual(Vector3.right, wizard.GetVectorValueWithoutValidation(CurrentPlayerLocationKeyVector),
                "Instance sync failed for a newly created key.");
        }

        [Test]
        public void BlackboardDirtyingValidation()
        {
            TryCreate(_baseCharacterTemplate, out var civilian);
            civilian.SetVectorValueWithoutValidation(CurrentPlayerLocationKeyVector, Vector3.right);

            Assert.IsTrue(civilian.HasUnexpectedChanges, "Simple dirtying test failed.");

            civilian.ClearUnexpectedChanges();

            Assert.IsFalse(civilian.HasUnexpectedChanges, "Clearing dirtying test failed.");

            TryCreate(_elementalistTemplate, out var baboon);
            civilian.SetVectorValueWithoutValidation(CurrentPlayerLocationKeyVector, Vector3.zero, true);

            Assert.IsFalse(civilian.HasUnexpectedChanges, "Expectation parameter test failed.");
            Assert.IsTrue(baboon.HasUnexpectedChanges, "Instance synced dirtying test failed.");
        }
    }
}