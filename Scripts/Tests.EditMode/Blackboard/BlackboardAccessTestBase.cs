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
        internal BlackboardTemplate m_BaseCharacterTemplate = null;
        internal BlackboardTemplateCompiledData baseCharacterData => m_BaseCharacterTemplate.compiledData;

        internal BlackboardTemplate m_WarriorTemplate = null;
        internal BlackboardTemplateCompiledData warriorData => m_WarriorTemplate.compiledData;

        internal BlackboardTemplate m_MageTemplate = null;
        internal BlackboardTemplateCompiledData mageData => m_MageTemplate.compiledData;

        internal BlackboardTemplate m_ElementalistTemplate = null;
        internal BlackboardTemplateCompiledData elementalistData => m_ElementalistTemplate.compiledData;

        internal ScriptableObject m_MockObject1 = null;
        internal ScriptableObject m_MockObject2 = null;
        internal ScriptableObject m_MockObject3 = null;

        protected ushort levelKeyInteger => baseCharacterData["Level"].m_MemoryOffset;
        protected ushort healthKeyFloat => baseCharacterData["Health"].m_MemoryOffset;
        protected ushort healthLowKeyBoolean => baseCharacterData["HealthLow"].m_MemoryOffset;
        protected ushort currentPlayerLocationKeyVector => baseCharacterData["CurrentPlayerLocation"].m_MemoryOffset;
        protected ushort playerReferenceKeyObject => baseCharacterData["PlayerReference"].m_MemoryOffset;
        protected ushort healthStatusKeyEnum => baseCharacterData["HealthStatus"].m_MemoryOffset;
        protected ushort staminaKeyFloat => warriorData["Stamina"].m_MemoryOffset;
        protected ushort manaKeyFloat => mageData["Mana"].m_MemoryOffset;
        protected ushort elementalPowerKeyInteger => elementalistData["ElementalPower"].m_MemoryOffset;
        protected ushort throwKeyQuaternion => elementalistData["Throw"].m_MemoryOffset;
        protected ushort powerTypeKeyEnum => elementalistData["PowerType"].m_MemoryOffset;

        protected void SetUp()
        {
            m_MockObject1 = ScriptableObject.CreateInstance<ScriptableObject>();
            m_MockObject1.hideFlags = HideFlags.HideAndDontSave;
            m_MockObject2 = ScriptableObject.CreateInstance<ScriptableObject>();
            m_MockObject2.hideFlags = HideFlags.HideAndDontSave;
            m_MockObject3 = ScriptableObject.CreateInstance<ScriptableObject>();
            m_MockObject3.hideFlags = HideFlags.HideAndDontSave;

            m_BaseCharacterTemplate = Resources.Load<BlackboardTemplate>("TestBaseCharacter");
            m_WarriorTemplate = Resources.Load<BlackboardTemplate>("TestWarrior");
            m_MageTemplate = Resources.Load<BlackboardTemplate>("TestMage");
            m_ElementalistTemplate = Resources.Load<BlackboardTemplate>("TestElementalist");

            CheckLoaded(m_BaseCharacterTemplate, m_WarriorTemplate, m_MageTemplate, m_ElementalistTemplate);
            Validate(m_BaseCharacterTemplate, m_WarriorTemplate, m_MageTemplate, m_ElementalistTemplate);
            Compile(m_BaseCharacterTemplate, m_WarriorTemplate, m_MageTemplate, m_ElementalistTemplate);
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
                Assert.IsTrue(validator.m_Validated, $"{template.name} could not be validated.");
                validator.Reset();
            }
        }

        private static void Compile(params BlackboardTemplate[] templates)
        {
            var compiler = new BlackboardTemplateCompilerContext();

            foreach (var template in templates)
            {
                template.Compile(compiler);
                Assert.IsTrue(template.isCompiled, $"{template.name} could not be compiled.");
                compiler.Update();
            }
        }

        protected void TearDown()
        {
            Free(m_ElementalistTemplate, m_MageTemplate, m_WarriorTemplate, m_BaseCharacterTemplate);
            Unload(m_ElementalistTemplate, m_MageTemplate, m_WarriorTemplate, m_BaseCharacterTemplate);

            m_BaseCharacterTemplate = m_WarriorTemplate = m_MageTemplate = m_ElementalistTemplate = null;

            Object.DestroyImmediate(m_MockObject3);
            Object.DestroyImmediate(m_MockObject2);
            Object.DestroyImmediate(m_MockObject1);
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