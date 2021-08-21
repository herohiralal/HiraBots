using NUnit.Framework;
using UnityEngine;

namespace HiraBots.Editor.Tests
{
    /// <summary>
    /// A generic unsigned 8-bit enum to be used for testing purposes.
    /// </summary>
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

    /// <summary>
    /// A generic signed 8-bit enum to be used for testing purposes.
    /// </summary>
    [ExposedToHiraBots("7E11BA86-1F8E-4010-92C2-FC9482B52507")]
    internal enum PowerType : sbyte
    {
        Fire, Air, Earth, Water
    }

    /// <summary>
    /// A generic signed 64-bit enum to be used for testing purposes.
    /// </summary>
    internal enum LongEnum : long
    {
    }

    /// <summary>
    /// Base class for blackboard testing, which compiles the test blackboards in OneTimeSetUp
    /// and frees them in OneTimeTearDown
    /// </summary>
    internal abstract class BlackboardAccessTestBase
    {
        internal BlackboardTemplate m_BaseCharacterTemplate = null;
        internal BlackboardTemplateCompiledData baseCharacterData => m_BaseCharacterTemplate.compiledData;

        internal BlackboardTemplate m_WarriorTemplate = null;
        internal BlackboardTemplateCompiledData warriorData => m_WarriorTemplate.compiledData;

        internal BlackboardTemplate m_MageTemplate = null;
        internal BlackboardTemplateCompiledData mageData => m_MageTemplate.compiledData;

        internal BlackboardTemplate m_ElementalistTemplate = null;
        internal BlackboardTemplateCompiledData elementalistData => m_ElementalistTemplate.compiledData;

        protected ScriptableObject mockObject1 { get; private set; } = null;
        protected ScriptableObject mockObject2 { get; private set; } = null;
        protected ScriptableObject mockObject3 { get; private set; } = null;

        protected ushort levelKeyInteger => baseCharacterData["Level"].memoryOffset;
        protected ushort healthKeyFloat => baseCharacterData["Health"].memoryOffset;
        protected ushort healthLowKeyBoolean => baseCharacterData["HealthLow"].memoryOffset;
        protected ushort currentPlayerLocationKeyVector => baseCharacterData["CurrentPlayerLocation"].memoryOffset;
        protected ushort playerReferenceKeyObject => baseCharacterData["PlayerReference"].memoryOffset;
        protected ushort healthStatusKeyEnum => baseCharacterData["HealthStatus"].memoryOffset;
        protected ushort staminaKeyFloat => warriorData["Stamina"].memoryOffset;
        protected ushort manaKeyFloat => mageData["Mana"].memoryOffset;
        protected ushort elementalPowerKeyInteger => elementalistData["ElementalPower"].memoryOffset;
        protected ushort throwKeyQuaternion => elementalistData["Throw"].memoryOffset;
        protected ushort powerTypeKeyEnum => elementalistData["PowerType"].memoryOffset;

        /// <summary>
        /// Validate and compile all blackboards.
        /// </summary>
        protected void SetUp(bool shouldValidateAndCompile)
        {
            mockObject1 = ScriptableObject.CreateInstance<ScriptableObject>();
            mockObject1.hideFlags = HideFlags.HideAndDontSave;
            mockObject2 = ScriptableObject.CreateInstance<ScriptableObject>();
            mockObject2.hideFlags = HideFlags.HideAndDontSave;
            mockObject3 = ScriptableObject.CreateInstance<ScriptableObject>();
            mockObject3.hideFlags = HideFlags.HideAndDontSave;

            m_BaseCharacterTemplate = Resources.Load<BlackboardTemplate>("TestBaseCharacter");
            m_WarriorTemplate = Resources.Load<BlackboardTemplate>("TestWarrior");
            m_MageTemplate = Resources.Load<BlackboardTemplate>("TestMage");
            m_ElementalistTemplate = Resources.Load<BlackboardTemplate>("TestElementalist");

            CheckLoaded(m_BaseCharacterTemplate, m_WarriorTemplate, m_MageTemplate, m_ElementalistTemplate);

            if (shouldValidateAndCompile)
            {
                Validate(m_BaseCharacterTemplate, m_WarriorTemplate, m_MageTemplate, m_ElementalistTemplate);
                Compile(m_BaseCharacterTemplate, m_WarriorTemplate, m_MageTemplate, m_ElementalistTemplate);
            }
        }

        // check whether all the templates are loaded
        private static void CheckLoaded(params BlackboardTemplate[] templates)
        {
            foreach (var template in templates)
            {
                Assert.IsTrue(template != null, "Template could not be loaded.");
            }
        }

        // check whether all the templates are validated
        private static void Validate(params BlackboardTemplate[] templates)
        {
            var validator = new BlackboardTemplateValidator();

            foreach (var template in templates)
            {
                var result = validator.Validate(template, out _);
                Assert.IsTrue(result, $"{template.name} could not be validated.");
            }
        }

        // compile all the templates
        private static void Compile(params BlackboardTemplate[] templates)
        {
            foreach (var template in templates)
            {
                template.Compile();
                Assert.IsTrue(template.isCompiled, $"{template.name} could not be compiled.");
            }
        }

        /// <summary>
        /// Free up all the allocations.
        /// </summary>
        protected void TearDown(bool shouldFreeAndUnload)
        {
            if (shouldFreeAndUnload)
            {
                Free(m_ElementalistTemplate, m_MageTemplate, m_WarriorTemplate, m_BaseCharacterTemplate);
                Unload(m_ElementalistTemplate, m_MageTemplate, m_WarriorTemplate, m_BaseCharacterTemplate);
            }

            m_BaseCharacterTemplate = m_WarriorTemplate = m_MageTemplate = m_ElementalistTemplate = null;

            Object.DestroyImmediate(mockObject3);
            Object.DestroyImmediate(mockObject2);
            Object.DestroyImmediate(mockObject1);
        }

        // free all the compiled templates
        private static void Free(params BlackboardTemplate[] templates)
        {
            foreach (var template in templates)
            {
                if (template != null)
                {
                    template.Free();
                }
            }
        }

        // unload all the loaded templates
        private static void Unload(params BlackboardTemplate[] templates)
        {
            foreach (var template in templates)
            {
                if (template != null)
                {
                    Resources.UnloadAsset(template);
                }
            }
        }
    }
}