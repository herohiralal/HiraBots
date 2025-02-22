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
        /// Check for validation failure when the parent is missing a backend.
        /// </summary>
        [Test]
        public void IncompatibleBackendValidation()
        {
            var parent = "Parent".BuildScriptableObject<BlackboardTemplate>();
            parent.BuildBlackboardTemplate(null, BackendType.CodeGenerator, new BlackboardKey[0]);

            var child = "Child".BuildScriptableObject<BlackboardTemplate>();
            child.BuildBlackboardTemplate(parent, BackendType.RuntimeInterpreter, new BlackboardKey[0]);

            var validator = new BlackboardTemplateValidator();

            try
            {
                Assert.IsTrue(validator.Validate(parent, out _), "Parent failed to validate, could not complete test.");

                var result = validator.Validate(child, out var errorText);

                Assert.IsFalse(result, "Validation succeeded when it shouldn't have.");

                var requiredError = BlackboardTemplateValidator
                    .FormatErrorStringForUnsupportedBackends(BackendType.RuntimeInterpreter);
                Assert.IsTrue(errorText.Contains(requiredError), "Correct missing backend not included in the report.");
            }
            finally
            {
                Object.DestroyImmediate(parent);
                Object.DestroyImmediate(child);
            }
        }

        /// <summary>
        /// Check for validation failure when a key is null.
        /// </summary>
        [Test]
        public void NullKeyValidation()
        {
            var floatThrowaway = "Dumb Key".BuildScriptableObject<FloatBlackboardKey>();
            floatThrowaway.BuildBlackboardKey(BlackboardKeyTraits.None);
            floatThrowaway.BuildFloatBlackboardKey(0f);

            var booleanThrowaway = "Second Dumb Key".BuildScriptableObject<BooleanBlackboardKey>();
            booleanThrowaway.BuildBlackboardKey(BlackboardKeyTraits.None);
            booleanThrowaway.BuildBooleanBlackboardKey(false);

            var template = "NullKeyFailTestObject".BuildScriptableObject<BlackboardTemplate>();
            template.BuildBlackboardTemplate(null, BackendType.RuntimeInterpreter, new BlackboardKey[]
                {
                    floatThrowaway,
                    null,
                    booleanThrowaway
                });

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
            var first = "NullKeyFailTestObject".BuildScriptableObject<BlackboardTemplate>();
            var second = "NullKeyFailTestObject".BuildScriptableObject<BlackboardTemplate>();
            second.BuildBlackboardTemplate(first, BackendType.RuntimeInterpreter, new BlackboardKey[0]);
            first.BuildBlackboardTemplate(second, BackendType.RuntimeInterpreter, new BlackboardKey[0]);

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
            var floatThrowaway = "Dumb Key".BuildScriptableObject<FloatBlackboardKey>();
            floatThrowaway.BuildBlackboardKey(BlackboardKeyTraits.None);
            floatThrowaway.BuildFloatBlackboardKey(0f);

            var booleanThrowaway = "Dumb Key".BuildScriptableObject<BooleanBlackboardKey>();
            booleanThrowaway.BuildBlackboardKey(BlackboardKeyTraits.None);
            booleanThrowaway.BuildBooleanBlackboardKey(false);

            var template = "NullKeyFailTestObject".BuildScriptableObject<BlackboardTemplate>();
            template.BuildBlackboardTemplate(null, BackendType.RuntimeInterpreter, new BlackboardKey[]
                {
                    floatThrowaway,
                    booleanThrowaway
                });

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