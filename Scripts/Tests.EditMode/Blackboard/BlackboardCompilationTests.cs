using NUnit.Framework;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace HiraBots.Editor.Tests
{
    [TestFixture]
    internal class BlackboardCompilationTests
    {
        private const string first_key_name = "first";
        private const string second_key_name = "second";
        private const string third_key_name = "third";
        private const string fourth_key_name = "fourth";
        private const string parent_template_name = "parent";
        private const string child_template_name = "child";
        private const int first_key_index = 1;
        private const int second_key_index = 0;
        private const int third_key_index = 3;
        private const int fourth_key_index = 2;
        private const int second_key_memory_offset = 0;
        private const int first_key_memory_offset = 1;
        private const int fourth_key_memory_offset = 5;
        private const int third_key_memory_offset = 9;
        private const int parent_template_size = 5;
        private const int child_template_size = 21;
        private const int parent_key_count = 2;
        private const int child_key_count = 4;
        private const int first_key_default_value = 0;
        private const bool second_key_default_value = true;
        private static readonly Vector3 third_key_default_value = Vector3.one;
        // ReSharper disable once InconsistentNaming
        private ScriptableObject fourth_key_default_value;
        private const BlackboardKeyTraits first_key_traits = BlackboardKeyTraits.None;
        private const BlackboardKeyTraits second_key_traits = BlackboardKeyTraits.InstanceSynced;
        private const BlackboardKeyTraits third_key_traits = BlackboardKeyTraits.BroadcastEventOnUnexpectedChange;
        private const BlackboardKeyTraits fourth_key_traits = BlackboardKeyTraits.InstanceSynced | BlackboardKeyTraits.BroadcastEventOnUnexpectedChange;
        private const BlackboardKeyType first_key_type = BlackboardKeyType.Integer;
        private const BlackboardKeyType second_key_type = BlackboardKeyType.Boolean;
        private const BlackboardKeyType third_key_type = BlackboardKeyType.Vector;
        private const BlackboardKeyType fourth_key_type = BlackboardKeyType.Object;

        private BlackboardKey _first, _second, _third, _fourth;
        private BlackboardTemplate _parent, _child;

        [OneTimeSetUp]
        public void SetUp()
        {
            fourth_key_default_value = ScriptableObject.CreateInstance<ScriptableObject>();
            fourth_key_default_value.hideFlags = HideFlags.HideAndDontSave;

            _first = IntegerBlackboardKey.Build<IntegerBlackboardKey>(first_key_name, first_key_traits, first_key_default_value, HideFlags.HideAndDontSave);

            _second = BooleanBlackboardKey.Build<BooleanBlackboardKey>(second_key_name, second_key_traits, second_key_default_value, HideFlags.HideAndDontSave);

            _third = VectorBlackboardKey.Build<VectorBlackboardKey>(third_key_name, third_key_traits, third_key_default_value, HideFlags.HideAndDontSave);

            _fourth = ObjectBlackboardKey.Build<ObjectBlackboardKey>(fourth_key_name, fourth_key_traits, fourth_key_default_value, HideFlags.HideAndDontSave);

            _parent = BlackboardTemplate.Build<BlackboardTemplate>(parent_template_name, null,
                new[] {_first, _second}, HideFlags.HideAndDontSave);
            _child = BlackboardTemplate.Build<BlackboardTemplate>(child_template_name, _parent,
                new[] {_third, _fourth}, HideFlags.HideAndDontSave);


            // validate the blackboard templates first
            {
                var validatorContext = new BlackboardTemplateValidatorContext();

                _parent.Validate(validatorContext);
                Assert.IsTrue(validatorContext.Validated, "Parent blackboard template couldn't be validated. Test incomplete.");

                validatorContext.Reset();

                _child.Validate(validatorContext);
                Assert.IsTrue(validatorContext.Validated, "Child blackboard template couldn't be validated. Test incomplete.");

                validatorContext.Reset();
            }

            // compile
            {
                var compilerContext = new BlackboardTemplateCompilerContext();
                _parent.Compile(compilerContext);
                compilerContext.Update();
                _child.Compile(compilerContext);
                compilerContext.Update();
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _child.Free();
            _parent.Free();

            Object.DestroyImmediate(_child);
            Object.DestroyImmediate(_parent);

            Object.DestroyImmediate(_fourth);
            Object.DestroyImmediate(_third);
            Object.DestroyImmediate(_second);
            Object.DestroyImmediate(_first);

            Object.DestroyImmediate(fourth_key_default_value);
        }

        private static bool AreEqual(in BlackboardKeyCompiledData a, in BlackboardKeyCompiledData b) =>
            a.MemoryOffset == b.MemoryOffset && a.Index == b.Index && a.Traits == b.Traits && a.KeyType == b.KeyType;

        [Test]
        public void TemplateKeyCompiledDataSyncValidation()
        {
            var parentCompiledData = _parent.CompiledData;
            var childCompiledData = _child.CompiledData;

            Assert.IsTrue(AreEqual(parentCompiledData[first_key_name], childCompiledData[first_key_name]));
            Assert.IsTrue(AreEqual(parentCompiledData[second_key_name], childCompiledData[second_key_name]));

            Assert.IsTrue(AreEqual(parentCompiledData[first_key_name], _first.CompiledData));
            Assert.IsTrue(AreEqual(parentCompiledData[second_key_name], _second.CompiledData));
            Assert.IsTrue(AreEqual(childCompiledData[third_key_name], _third.CompiledData));
            Assert.IsTrue(AreEqual(childCompiledData[fourth_key_name], _fourth.CompiledData));
        }

        [Test]
        public void KeyIndexSyncValidation()
        {
            // blackboard keys get sorted according to their sizes
            Assert.AreEqual(first_key_index, _first.CompiledData.Index);
            Assert.AreEqual(second_key_index, _second.CompiledData.Index);
            Assert.AreEqual(third_key_index, _third.CompiledData.Index);
            Assert.AreEqual(fourth_key_index, _fourth.CompiledData.Index);
        }

        [Test]
        public void KeyTypeSyncValidation()
        {
            Assert.AreEqual(first_key_type, _first.CompiledData.KeyType);
            Assert.AreEqual(second_key_type, _second.CompiledData.KeyType);
            Assert.AreEqual(third_key_type, _third.CompiledData.KeyType);
            Assert.AreEqual(fourth_key_type, _fourth.CompiledData.KeyType);
        }

        [Test]
        public void KeyTraitsSyncValidation()
        {
            Assert.AreEqual(first_key_traits, _first.CompiledData.Traits);
            Assert.AreEqual(second_key_traits, _second.CompiledData.Traits);
            Assert.AreEqual(third_key_traits, _third.CompiledData.Traits);
            Assert.AreEqual(fourth_key_traits, _fourth.CompiledData.Traits);
        }

        [Test]
        public void KeyCountValidation()
        {
            Assert.AreEqual(parent_key_count, _parent.CompiledData.KeyCount);
            Assert.AreEqual(child_key_count, _child.CompiledData.KeyCount);
        }

        [Test]
        public void TemplateSizeValidation()
        {
            Assert.AreEqual(parent_template_size, _parent.CompiledData.TemplateSize, "Child template size mismatch.");
            Assert.AreEqual(child_template_size, _child.CompiledData.TemplateSize, "Child template size mismatch.");
        }

        [Test]
        public void MemoryOffsetValidation()
        {
            // blackboard keys get sorted according to their sizes
            Assert.AreEqual(second_key_memory_offset, _second.CompiledData.MemoryOffset);
            Assert.AreEqual(first_key_memory_offset, _first.CompiledData.MemoryOffset);
            Assert.AreEqual(fourth_key_memory_offset, _fourth.CompiledData.MemoryOffset);
            Assert.AreEqual(third_key_memory_offset, _third.CompiledData.MemoryOffset);
        }

        [Test]
        public unsafe void TemplateDefaultsValidationParent()
        {
            using (var template = new NativeArray<byte>(_parent.CompiledData.TemplateSize, Allocator.Temp, NativeArrayOptions.UninitializedMemory))
            {
                _parent.CompiledData.CopyTemplateTo(template);
                var templatePtr = (byte*) template.GetUnsafeReadOnlyPtr();

                var firstValue = BlackboardUnsafeHelpers.ReadIntegerValue(templatePtr, _first.CompiledData.MemoryOffset);
                Assert.AreEqual(first_key_default_value, firstValue);

                var secondValue = BlackboardUnsafeHelpers.ReadBooleanValue(templatePtr, _second.CompiledData.MemoryOffset);
                Assert.AreEqual(second_key_default_value, secondValue);
            }
        }

        [Test]
        public unsafe void TemplateDefaultsValidationChild()
        {
            using (var template = new NativeArray<byte>(_child.CompiledData.TemplateSize, Allocator.Temp, NativeArrayOptions.UninitializedMemory))
            {
                _child.CompiledData.CopyTemplateTo(template);
                var templatePtr = (byte*) template.GetUnsafeReadOnlyPtr();

                var firstValue = BlackboardUnsafeHelpers.ReadIntegerValue(templatePtr, _first.CompiledData.MemoryOffset);
                Assert.AreEqual(first_key_default_value, firstValue);

                var secondValue = BlackboardUnsafeHelpers.ReadBooleanValue(templatePtr, _second.CompiledData.MemoryOffset);
                Assert.AreEqual(second_key_default_value, secondValue);

                var thirdValue = BlackboardUnsafeHelpers.ReadVectorValue(templatePtr, _third.CompiledData.MemoryOffset);
                Assert.AreEqual(third_key_default_value, thirdValue);

                var fourthValue = BlackboardUnsafeHelpers.ReadObjectValue(templatePtr, _fourth.CompiledData.MemoryOffset);
                Assert.AreEqual(fourth_key_default_value, fourthValue);
            }
        }

        [Test]
        public unsafe void InstanceSyncedKeysSynchronizedBetweenChildAndParentTemplatesCheck()
        {
            using (var template = new NativeArray<byte>(_child.CompiledData.TemplateSize, Allocator.Temp, NativeArrayOptions.UninitializedMemory))
            {
                var templatePtr = (byte*) template.GetUnsafeReadOnlyPtr();

                var secondKeyCompiledData = _second.CompiledData;
                _parent.CompiledData.SetInstanceSyncedBooleanValueWithoutValidation(
                    in secondKeyCompiledData,
                    !second_key_default_value);

                _parent.CompiledData.CopyTemplateTo(template);
                Assert.AreEqual(!second_key_default_value, BlackboardUnsafeHelpers.ReadBooleanValue(templatePtr, secondKeyCompiledData.MemoryOffset),
                    "Update not applied to the parent.");

                _child.CompiledData.CopyTemplateTo(template);
                Assert.AreEqual(!second_key_default_value, BlackboardUnsafeHelpers.ReadBooleanValue(templatePtr, secondKeyCompiledData.MemoryOffset),
                    "Mismatch between child and parent values.");

                _child.CompiledData.SetInstanceSyncedBooleanValueWithoutValidation(
                    in secondKeyCompiledData,
                    second_key_default_value);

                _parent.CompiledData.CopyTemplateTo(template);
                Assert.AreEqual(second_key_default_value, BlackboardUnsafeHelpers.ReadBooleanValue(templatePtr, secondKeyCompiledData.MemoryOffset),
                    "Update not applied to the parent.");

                _child.CompiledData.CopyTemplateTo(template);
                Assert.AreEqual(second_key_default_value, BlackboardUnsafeHelpers.ReadBooleanValue(templatePtr, secondKeyCompiledData.MemoryOffset),
                    "Mismatch between child and parent values.");
            }
        }

        [Test]
        public void OwningTemplateValidation()
        {
            var parentData = _parent.CompiledData;
            var childData = _child.CompiledData;

            Assert.IsTrue(parentData.GetOwningTemplate(first_key_memory_offset) == parentData, "Culprit: Parent. Key: first.");
            Assert.IsTrue(childData.GetOwningTemplate(first_key_memory_offset) == parentData, "Culprit: Child. Key: first.");

            Assert.IsTrue(parentData.GetOwningTemplate(second_key_memory_offset) == parentData, "Culprit: Parent. Key: second.");
            Assert.IsTrue(childData.GetOwningTemplate(second_key_memory_offset) == parentData, "Culprit: Child. Key: second.");

            Assert.IsTrue(parentData.GetOwningTemplate(third_key_memory_offset) == null, "Culprit: Parent. Key: third.");
            Assert.IsTrue(childData.GetOwningTemplate(third_key_memory_offset) == childData, "Culprit: Child. Key: third.");

            Assert.IsTrue(parentData.GetOwningTemplate(fourth_key_memory_offset) == null, "Culprit: Parent. Key: fourth.");
            Assert.IsTrue(childData.GetOwningTemplate(fourth_key_memory_offset) == childData, "Culprit: Child. Key: fourth.");

            Assert.IsTrue(parentData.GetOwningTemplate(parentData.TemplateSize) == null, "Culprit: Parent. Key: template size.");
            Assert.IsTrue(childData.GetOwningTemplate(childData.TemplateSize) == null, "Culprit: Child. Key: template size.");

            Assert.IsTrue(parentData.GetOwningTemplate((ushort) (parentData.TemplateSize + 5)) == null, "Culprit: Parent. Key: template size + 5.");
            Assert.IsTrue(childData.GetOwningTemplate((ushort) (childData.TemplateSize + 5)) == null, "Culprit: Child. Key: template size + 5.");
        }
    }
}