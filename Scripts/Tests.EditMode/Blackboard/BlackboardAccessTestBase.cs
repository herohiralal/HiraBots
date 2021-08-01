using NUnit.Framework;
using UnityEngine;

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

    public abstract class BlackboardAccessTestBase
    {
        internal BlackboardTemplate BaseCharacterTemplate = null;
        internal BlackboardTemplateCompiledData BaseCharacterData => BaseCharacterTemplate.CompiledData;

        internal BlackboardTemplate WarriorTemplate = null;
        internal BlackboardTemplateCompiledData WarriorData => WarriorTemplate.CompiledData;

        internal BlackboardTemplate MageTemplate = null;
        internal BlackboardTemplateCompiledData MageData => MageTemplate.CompiledData;

        internal BlackboardTemplate ElementalistTemplate = null;
        internal BlackboardTemplateCompiledData ElementalistData => ElementalistTemplate.CompiledData;

        internal ScriptableObject MockObject1 = null;
        internal ScriptableObject MockObject2 = null;
        internal ScriptableObject MockObject3 = null;

        protected ushort LevelKeyInteger => BaseCharacterData["Level"].MemoryOffset;
        protected ushort HealthKeyFloat => BaseCharacterData["Health"].MemoryOffset;
        protected ushort HealthLowKeyBoolean => BaseCharacterData["HealthLow"].MemoryOffset;
        protected ushort CurrentPlayerLocationKeyVector => BaseCharacterData["CurrentPlayerLocation"].MemoryOffset;
        protected ushort PlayerReferenceKeyObject => BaseCharacterData["PlayerReference"].MemoryOffset;
        protected ushort HealthStatusKeyEnum => BaseCharacterData["HealthStatus"].MemoryOffset;
        protected ushort StaminaKeyFloat => WarriorData["Stamina"].MemoryOffset;
        protected ushort ManaKeyFloat => MageData["Mana"].MemoryOffset;
        protected ushort ElementalPowerKeyInteger => ElementalistData["ElementalPower"].MemoryOffset;
        protected ushort ThrowKeyQuaternion => ElementalistData["Throw"].MemoryOffset;
        protected ushort PowerTypeKeyEnum => ElementalistData["PowerType"].MemoryOffset;

        protected void SetUp()
        {
            MockObject1 = ScriptableObject.CreateInstance<ScriptableObject>();
            MockObject1.hideFlags = HideFlags.HideAndDontSave;
            MockObject2 = ScriptableObject.CreateInstance<ScriptableObject>();
            MockObject2.hideFlags = HideFlags.HideAndDontSave;
            MockObject3 = ScriptableObject.CreateInstance<ScriptableObject>();
            MockObject3.hideFlags = HideFlags.HideAndDontSave;

            BaseCharacterTemplate = Resources.Load<BlackboardTemplate>("TestBaseCharacter");
            WarriorTemplate = Resources.Load<BlackboardTemplate>("TestWarrior");
            MageTemplate = Resources.Load<BlackboardTemplate>("TestMage");
            ElementalistTemplate = Resources.Load<BlackboardTemplate>("TestElementalist");

            CheckLoaded(BaseCharacterTemplate, WarriorTemplate, MageTemplate, ElementalistTemplate);
            Validate(BaseCharacterTemplate, WarriorTemplate, MageTemplate, ElementalistTemplate);
            Compile(BaseCharacterTemplate, WarriorTemplate, MageTemplate, ElementalistTemplate);
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

        protected void TearDown()
        {
            Free(ElementalistTemplate, MageTemplate, WarriorTemplate, BaseCharacterTemplate);
            Unload(ElementalistTemplate, MageTemplate, WarriorTemplate, BaseCharacterTemplate);

            BaseCharacterTemplate = WarriorTemplate = MageTemplate = ElementalistTemplate = null;

            Object.DestroyImmediate(MockObject3);
            Object.DestroyImmediate(MockObject2);
            Object.DestroyImmediate(MockObject1);
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
    }
}