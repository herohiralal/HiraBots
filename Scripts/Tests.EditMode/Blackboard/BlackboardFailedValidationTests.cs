using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace HiraBots.Editor.Tests
{
    /// <summary>
    /// Determine that the blackboard templates fail validation when invalid input is provided.
    /// </summary>
    [TestFixture]
    internal class BlackboardFailedValidationTests
    {
        /// <summary>
        /// Check for validation failure when a key is null.
        /// </summary>
        [Test]
        public void NullKeyValidation()
        {
            var floatThrowaway = FloatBlackboardKey.Build<FloatBlackboardKey>("Dumb Key",
                BlackboardKeyTraits.None, 0f, HideFlags.HideAndDontSave);

            var booleanThrowaway = BooleanBlackboardKey.Build<BooleanBlackboardKey>("Second Dumb Key",
                BlackboardKeyTraits.None, false, HideFlags.HideAndDontSave);

            var template = BlackboardTemplate
                .Build<BlackboardTemplate>("NullKeyFailTestObject", null, new BlackboardKey[]
                {
                    floatThrowaway,
                    null,
                    booleanThrowaway
                }, HideFlags.HideAndDontSave);

            var validatorContext = new BlackboardTemplateValidatorContext();

            try
            {
                template.Validate(validatorContext);

                Assert.IsFalse(validatorContext.validated, "Validation succeeded when it shouldn't have.");
                Assert.IsTrue(validatorContext.emptyIndices.Contains(1), "Correct validation index not included in the report.");
            }
            finally
            {
                validatorContext.Reset();
                Object.DestroyImmediate(template);
                Object.DestroyImmediate(floatThrowaway);
                Object.DestroyImmediate(booleanThrowaway);
            }
        }

        /// <summary>
        /// Check for validation failure for cyclical hierarchies.
        /// </summary>
        [Test]
        public void CyclicalHierarchyValidation()
        {
            var parentField = typeof(BlackboardTemplate).GetField("m_Parent", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsFalse(parentField == null, "Could not find parent field in blackboard template. Test incomplete.");

            var first = BlackboardTemplate
                .Build<BlackboardTemplate>("NullKeyFailTestObject", null, new BlackboardKey[0], HideFlags.HideAndDontSave);
            var second = BlackboardTemplate
                .Build<BlackboardTemplate>("NullKeyFailTestObject", first, new BlackboardKey[0], HideFlags.HideAndDontSave);

            parentField.SetValue(first, second);

            var validatorContext = new BlackboardTemplateValidatorContext();

            try
            {
                first.Validate(validatorContext);

                Assert.IsFalse(validatorContext.validated, "Validation succeeded when it shouldn't have.");
                Assert.IsTrue(validatorContext.recursionPoint == second, "Correct recursion point not included in the report.");

                validatorContext.Reset();

                second.Validate(validatorContext);

                Assert.IsFalse(validatorContext.validated, "Validation succeeded when it shouldn't have.");
                Assert.IsTrue(validatorContext.recursionPoint == first, "Correct recursion point not included in the report.");
            }
            finally
            {
                validatorContext.Reset();
                Object.DestroyImmediate(first);
                Object.DestroyImmediate(second);
            }
        }

        /// <summary>
        /// Check for validation failure on name duplication.
        /// </summary>
        [Test]
        public void DuplicateNameValidation()
        {
            var floatThrowaway = FloatBlackboardKey.Build<FloatBlackboardKey>("Dumb Key",
                BlackboardKeyTraits.None, 0f, HideFlags.HideAndDontSave);

            var booleanThrowaway = BooleanBlackboardKey.Build<BooleanBlackboardKey>("Dumb Key",
                BlackboardKeyTraits.None, false, HideFlags.HideAndDontSave);

            var template = BlackboardTemplate
                .Build<BlackboardTemplate>("NullKeyFailTestObject", null, new BlackboardKey[]
                {
                    floatThrowaway,
                    booleanThrowaway
                }, HideFlags.HideAndDontSave);

            var validatorContext = new BlackboardTemplateValidatorContext();

            try
            {
                template.Validate(validatorContext);

                Assert.IsFalse(validatorContext.validated, "Validation succeeded when it shouldn't have.");
                Assert.IsTrue(validatorContext.duplicateKeys.Contains(("Dumb Key", template)), "Correct key name not included in the report.");
            }
            finally
            {
                validatorContext.Reset();
                Object.DestroyImmediate(template);
                Object.DestroyImmediate(floatThrowaway);
                Object.DestroyImmediate(booleanThrowaway);
            }
        }
    }
}