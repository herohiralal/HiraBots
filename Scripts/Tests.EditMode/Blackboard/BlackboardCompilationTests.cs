using NUnit.Framework;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace HiraBots.Editor.Tests
{
    [TestFixture]
    internal class BlackboardCompilationTests
    {
        private BlackboardKey _first, _second, _third, _fourth;
        private BlackboardTemplate _parent, _child;
        private ScriptableObject _throwaway;

        [OneTimeSetUp]
        public void SetUp()
        {
            _throwaway = ScriptableObject.CreateInstance<ScriptableObject>();
            _throwaway.hideFlags = HideFlags.HideAndDontSave;

            _first = IntegerBlackboardKey.Build<IntegerBlackboardKey>("first",
                BlackboardKeyTraits.None, 0,
                HideFlags.HideAndDontSave);

            _second = BooleanBlackboardKey.Build<BooleanBlackboardKey>("second",
                BlackboardKeyTraits.InstanceSynced, true,
                HideFlags.HideAndDontSave);

            _third = VectorBlackboardKey.Build<VectorBlackboardKey>("third",
                BlackboardKeyTraits.BroadcastEventOnUnexpectedChange,
                Vector3.one, HideFlags.HideAndDontSave);

            _fourth = ObjectBlackboardKey.Build<ObjectBlackboardKey>("fourth",
                BlackboardKeyTraits.InstanceSynced | BlackboardKeyTraits.BroadcastEventOnUnexpectedChange,
                _throwaway, HideFlags.HideAndDontSave);

            _parent = BlackboardTemplate.Build<BlackboardTemplate>("parent", null,
                new[] {_first, _second}, HideFlags.HideAndDontSave);
            _child = BlackboardTemplate.Build<BlackboardTemplate>("child", _parent,
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

            Object.DestroyImmediate(_throwaway);
        }

        private static bool AreEqual(in BlackboardKeyCompiledData a, in BlackboardKeyCompiledData b) =>
            a.MemoryOffset == b.MemoryOffset && a.Index == b.Index && a.Traits == b.Traits && a.KeyType == b.KeyType;

        [Test]
        public void TemplateKeyCompiledDataSyncValidation()
        {
            var parentCompiledData = _parent.CompiledData;
            var childCompiledData = _child.CompiledData;

            Assert.IsTrue(AreEqual(parentCompiledData["first"], childCompiledData["first"]));
            Assert.IsTrue(AreEqual(parentCompiledData["second"], childCompiledData["second"]));

            Assert.IsTrue(AreEqual(parentCompiledData["first"], _first.CompiledData));
            Assert.IsTrue(AreEqual(parentCompiledData["second"], _second.CompiledData));
            Assert.IsTrue(AreEqual(childCompiledData["third"], _third.CompiledData));
            Assert.IsTrue(AreEqual(childCompiledData["fourth"], _fourth.CompiledData));
        }

        [Test]
        public void KeyIndexSyncValidation()
        {
            // blackboard keys get sorted according to their sizes
            Assert.AreEqual(0, _second.CompiledData.Index);
            Assert.AreEqual(1, _first.CompiledData.Index);
            Assert.AreEqual(2, _fourth.CompiledData.Index);
            Assert.AreEqual(3, _third.CompiledData.Index);
        }

        [Test]
        public void KeyTypeSyncValidation()
        {
            Assert.AreEqual(BlackboardKeyType.Integer, _first.CompiledData.KeyType);
            Assert.AreEqual(BlackboardKeyType.Boolean, _second.CompiledData.KeyType);
            Assert.AreEqual(BlackboardKeyType.Vector, _third.CompiledData.KeyType);
            Assert.AreEqual(BlackboardKeyType.Object, _fourth.CompiledData.KeyType);
        }

        [Test]
        public void KeyTraitsSyncValidation()
        {
            Assert.AreEqual(BlackboardKeyTraits.None, _first.CompiledData.Traits);
            Assert.AreEqual(BlackboardKeyTraits.InstanceSynced, _second.CompiledData.Traits);
            Assert.AreEqual(BlackboardKeyTraits.BroadcastEventOnUnexpectedChange, _third.CompiledData.Traits);
            Assert.AreEqual(BlackboardKeyTraits.InstanceSynced | BlackboardKeyTraits.BroadcastEventOnUnexpectedChange, _fourth.CompiledData.Traits);
        }

        [Test]
        public void TemplateSizeValidation()
        {
            Assert.AreEqual(5, _parent.CompiledData.TemplateSize, "Child template size mismatch.");
            Assert.AreEqual(21, _child.CompiledData.TemplateSize, "Child template size mismatch.");
        }

        [Test]
        public void MemoryOffsetValidation()
        {
            // blackboard keys get sorted according to their sizes
            Assert.AreEqual(0, _second.CompiledData.MemoryOffset);
            Assert.AreEqual(1, _first.CompiledData.MemoryOffset);
            Assert.AreEqual(5, _fourth.CompiledData.MemoryOffset);
            Assert.AreEqual(9, _third.CompiledData.MemoryOffset);
        }

        [Test]
        public unsafe void TemplateDefaultsValidationParent()
        {
            using (var template = new NativeArray<byte>(_parent.CompiledData.TemplateSize, Allocator.Temp, NativeArrayOptions.UninitializedMemory))
            {
                _parent.CompiledData.CopyTemplateTo(template);

                var firstValue = BlackboardUnsafeHelpers.ReadIntegerValue((byte*) template.GetUnsafeReadOnlyPtr(), _first.CompiledData.MemoryOffset);
                Assert.AreEqual(0, firstValue);

                var secondValue = BlackboardUnsafeHelpers.ReadBooleanValue((byte*) template.GetUnsafeReadOnlyPtr(), _second.CompiledData.MemoryOffset);
                Assert.AreEqual(true, secondValue);
            }
        }

        [Test]
        public unsafe void TemplateDefaultsValidationChild()
        {
            using (var template = new NativeArray<byte>(_child.CompiledData.TemplateSize, Allocator.Temp, NativeArrayOptions.UninitializedMemory))
            {
                _child.CompiledData.CopyTemplateTo(template);

                var firstValue = BlackboardUnsafeHelpers.ReadIntegerValue((byte*) template.GetUnsafeReadOnlyPtr(), _first.CompiledData.MemoryOffset);
                Assert.AreEqual(0, firstValue);

                var secondValue = BlackboardUnsafeHelpers.ReadBooleanValue((byte*) template.GetUnsafeReadOnlyPtr(), _second.CompiledData.MemoryOffset);
                Assert.AreEqual(true, secondValue);

                var thirdValue = BlackboardUnsafeHelpers.ReadVectorValue((byte*) template.GetUnsafeReadOnlyPtr(), _third.CompiledData.MemoryOffset);
                Assert.AreEqual(Vector3.one, thirdValue);

                var fourthValue = BlackboardUnsafeHelpers.ReadObjectValue((byte*) template.GetUnsafeReadOnlyPtr(), _fourth.CompiledData.MemoryOffset);
                Assert.AreEqual(_throwaway, fourthValue);
            }
        }
    }
}