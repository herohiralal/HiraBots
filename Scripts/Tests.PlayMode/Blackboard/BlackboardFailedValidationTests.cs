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

            var validator = new BlackboardTemplateValidator();

            try
            {
                var result = validator.Validate(template, out var errorText);

                Assert.IsFalse(result, "Validation succeeded when it shouldn't have.");
                Assert.IsTrue(errorText.Contains(BlackboardTemplateValidator.FormatErrorStringForEmptyIndex(1)),
                    "Correct validation index not included in the report.");
            }
            finally
            {
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

            var validator = new BlackboardTemplateValidator();

            try
            {
                var result = validator.Validate(first, out var errorText);

                Assert.IsFalse(result, "Validation succeeded when it shouldn't have.");
                Assert.IsTrue(errorText.Contains(BlackboardTemplateValidator.FormatErrorStringForRecursionPoint(second)),
                    "Correct recursion point not included in the report.");

                result = validator.Validate(second, out errorText);

                Assert.IsFalse(result, "Validation succeeded when it shouldn't have.");
                Assert.IsTrue(errorText.Contains(BlackboardTemplateValidator.FormatErrorStringForRecursionPoint(first)),
                    "Correct recursion point not included in the report.");
            }
            finally
            {
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

            var validator = new BlackboardTemplateValidator();

            try
            {
                var result = validator.Validate(template, out var errorText);

                Assert.IsFalse(result, "Validation succeeded when it shouldn't have.");
                Assert.IsTrue(errorText.Contains(BlackboardTemplateValidator.FormatErrorStringForDuplicateKey(template, "Dumb Key", template)),
                    "Correct key name not included in the report.");
            }
            finally
            {
                Object.DestroyImmediate(template);
                Object.DestroyImmediate(floatThrowaway);
                Object.DestroyImmediate(booleanThrowaway);
            }
        }
    }
}