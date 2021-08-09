using NUnit.Framework;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace HiraBots.Editor.Tests
{
    /// <summary>
    /// Tests for cross-checking compiled blackboard template data against original inputs.
    /// </summary>
    [TestFixture]
    internal class BlackboardCompilationTests
    {
        private const string k_FirstKeyName = "first";
        private const string k_SecondKeyName = "second";
        private const string k_ThirdKeyName = "third";
        private const string k_FourthKeyName = "fourth";
        private const string k_ParentTemplateName = "parent";
        private const string k_ChildTemplateName = "child";
        private const int k_FirstKeyIndex = 1;
        private const int k_SecondKeyIndex = 0;
        private const int k_ThirdKeyIndex = 3;
        private const int k_FourthKeyIndex = 2;
        private const int k_SecondKeyMemoryOffset = 0;
        private const int k_FirstKeyMemoryOffset = 1;
        private const int k_FourthKeyMemoryOffset = k_ParentTemplateSize + 0;
        private const int k_ThirdKeyMemoryOffset = k_ParentTemplateSize + 4;
        private const int k_ParentTemplateSize = (5 + 3) & ~3;
        private static readonly unsafe int s_ChildTemplateSize = k_ParentTemplateSize + ((sizeof(float3) + sizeof(int) + 3) & ~3);
        private const int k_ParentKeyCount = 2;
        private const int k_ChildKeyCount = 4;
        private const int k_FirstKeyDefaultValue = 0;
        private const bool k_SecondKeyDefaultValue = true;
        private static readonly float3 s_ThirdKeyDefaultValue = new float3(1, 1, 1);
        private ScriptableObject m_FourthKeyDefaultValue;
        private const BlackboardKeyTraits k_FirstKeyTraits = BlackboardKeyTraits.None;
        private const BlackboardKeyTraits k_SecondKeyTraits = BlackboardKeyTraits.InstanceSynced;
        private const BlackboardKeyTraits k_ThirdKeyTraits = BlackboardKeyTraits.BroadcastEventOnUnexpectedChange;
        private const BlackboardKeyTraits k_FourthKeyTraits = BlackboardKeyTraits.InstanceSynced | BlackboardKeyTraits.BroadcastEventOnUnexpectedChange;
        private const BlackboardKeyType k_FirstKeyType = BlackboardKeyType.Integer;
        private const BlackboardKeyType k_SecondKeyType = BlackboardKeyType.Boolean;
        private const BlackboardKeyType k_ThirdKeyType = BlackboardKeyType.Vector;
        private const BlackboardKeyType k_FourthKeyType = BlackboardKeyType.Object;

        private BlackboardKey m_First, m_Second, m_Third, m_Fourth;
        private BlackboardTemplate m_Parent, m_Child;

        /// <summary>
        /// Build the blackboard templates, validate them, and compile them.
        /// </summary>
        [OneTimeSetUp]
        public void SetUp()
        {
            m_FourthKeyDefaultValue = ScriptableObject.CreateInstance<ScriptableObject>();
            m_FourthKeyDefaultValue.hideFlags = HideFlags.HideAndDontSave;

            var first = k_FirstKeyName.BuildScriptableObject<IntegerBlackboardKey>();
            first.BuildBlackboardKey(k_FirstKeyTraits);
            first.BuildIntegerBlackboardKey(k_FirstKeyDefaultValue);
            m_First = first;

            var second = k_SecondKeyName.BuildScriptableObject<BooleanBlackboardKey>();
            second.BuildBlackboardKey(k_SecondKeyTraits);
            second.BuildBooleanBlackboardKey(k_SecondKeyDefaultValue);
            m_Second = second;

            var third = k_ThirdKeyName.BuildScriptableObject<VectorBlackboardKey>();
            third.BuildBlackboardKey(k_ThirdKeyTraits);
            third.BuildVectorBlackboardKey(s_ThirdKeyDefaultValue);
            m_Third = third;

            var fourth = k_FourthKeyName.BuildScriptableObject<ObjectBlackboardKey>();
            fourth.BuildBlackboardKey(k_FourthKeyTraits);
            fourth.BuildObjectBlackboardKey(m_FourthKeyDefaultValue);
            m_Fourth = fourth;

            m_Parent = k_ParentTemplateName.BuildScriptableObject<BlackboardTemplate>();
            m_Parent.BuildBlackboardTemplate(null, new[] {m_First, m_Second});

            m_Child = k_ChildTemplateName.BuildScriptableObject<BlackboardTemplate>();
            m_Child.BuildBlackboardTemplate(m_Parent, new[] {m_Third, m_Fourth});


            // validate the blackboard templates first
            {
                var validator = new BlackboardTemplateValidator();

                var result = validator.Validate(m_Parent, out _);
                Assert.IsTrue(result, "Parent blackboard template couldn't be validated. Test incomplete.");

                result = validator.Validate(m_Child, out _);
                Assert.IsTrue(result, "Child blackboard template couldn't be validated. Test incomplete.");
            }

            // compile
            {
                var compiler = new BlackboardTemplateCompiler();
                compiler.Compile(m_Parent);
                compiler.Compile(m_Child);
            }
        }

        /// <summary>
        /// Free up compiled templates, and destroy the created objects.
        /// </summary>
        [OneTimeTearDown]
        public void TearDown()
        {
            m_Child.Free();
            m_Parent.Free();

            Object.DestroyImmediate(m_Child);
            Object.DestroyImmediate(m_Parent);

            Object.DestroyImmediate(m_Fourth);
            Object.DestroyImmediate(m_Third);
            Object.DestroyImmediate(m_Second);
            Object.DestroyImmediate(m_First);

            Object.DestroyImmediate(m_FourthKeyDefaultValue);
        }

        // helper function to check if two BlackboardKeyCompiledData objects are equal.
        private static bool AreEqual(in BlackboardKeyCompiledData a, in BlackboardKeyCompiledData b)
        {
            return
                a.memoryOffset == b.memoryOffset
                && a.index == b.index
                && a.traits == b.traits
                && a.keyType == b.keyType;
        }

        /// <summary>
        /// Cross-check between compiled data stored on a key and the data stored on the blackboard template's compiled data.
        /// </summary>
        [Test]
        public void TemplateKeyCompiledDataSyncValidation()
        {
            var parentCompiledData = m_Parent.compiledData;
            var childCompiledData = m_Child.compiledData;

            Assert.IsTrue(AreEqual(parentCompiledData[k_FirstKeyName], childCompiledData[k_FirstKeyName]));
            Assert.IsTrue(AreEqual(parentCompiledData[k_SecondKeyName], childCompiledData[k_SecondKeyName]));

            Assert.IsTrue(AreEqual(parentCompiledData[k_FirstKeyName], m_First.compiledData));
            Assert.IsTrue(AreEqual(parentCompiledData[k_SecondKeyName], m_Second.compiledData));
            Assert.IsTrue(AreEqual(childCompiledData[k_ThirdKeyName], m_Third.compiledData));
            Assert.IsTrue(AreEqual(childCompiledData[k_FourthKeyName], m_Fourth.compiledData));
        }

        /// <summary>
        /// Check whether the keys get compiled in the order of their sizes.
        /// </summary>
        [Test]
        public void KeyIndexSyncValidation()
        {
            // blackboard keys get sorted according to their sizes
            Assert.AreEqual(k_FirstKeyIndex, m_First.compiledData.index);
            Assert.AreEqual(k_SecondKeyIndex, m_Second.compiledData.index);
            Assert.AreEqual(k_ThirdKeyIndex, m_Third.compiledData.index);
            Assert.AreEqual(k_FourthKeyIndex, m_Fourth.compiledData.index);
        }

        /// <summary>
        /// Cross-check key type between provided data and compiled data.
        /// </summary>
        [Test]
        public void KeyTypeSyncValidation()
        {
            Assert.AreEqual(k_FirstKeyType, m_First.compiledData.keyType);
            Assert.AreEqual(k_SecondKeyType, m_Second.compiledData.keyType);
            Assert.AreEqual(k_ThirdKeyType, m_Third.compiledData.keyType);
            Assert.AreEqual(k_FourthKeyType, m_Fourth.compiledData.keyType);
        }

        /// <summary>
        /// Cross-check key traits between provided data and compiled data.
        /// </summary>
        [Test]
        public void KeyTraitsSyncValidation()
        {
            Assert.AreEqual(k_FirstKeyTraits, m_First.compiledData.traits);
            Assert.AreEqual(k_SecondKeyTraits, m_Second.compiledData.traits);
            Assert.AreEqual(k_ThirdKeyTraits, m_Third.compiledData.traits);
            Assert.AreEqual(k_FourthKeyTraits, m_Fourth.compiledData.traits);
        }

        /// <summary>
        /// Validate key counts.
        /// </summary>
        [Test]
        public void KeyCountValidation()
        {
            Assert.AreEqual(k_ParentKeyCount, m_Parent.compiledData.keyCount);
            Assert.AreEqual(k_ChildKeyCount, m_Child.compiledData.keyCount);
        }

        /// <summary>
        /// Validate template sizes.
        /// </summary>
        [Test]
        public void TemplateSizeValidation()
        {
            Assert.AreEqual(k_ParentTemplateSize, m_Parent.compiledData.templateSize, "Child template size mismatch.");
            Assert.AreEqual(s_ChildTemplateSize, m_Child.compiledData.templateSize, "Child template size mismatch.");
        }

        /// <summary>
        /// Validate memory offsets.
        /// </summary>
        [Test]
        public void MemoryOffsetValidation()
        {
            // blackboard keys get sorted according to their sizes
            Assert.AreEqual(k_SecondKeyMemoryOffset, m_Second.compiledData.memoryOffset);
            Assert.AreEqual(k_FirstKeyMemoryOffset, m_First.compiledData.memoryOffset);
            Assert.AreEqual(k_FourthKeyMemoryOffset, m_Fourth.compiledData.memoryOffset);
            Assert.AreEqual(k_ThirdKeyMemoryOffset, m_Third.compiledData.memoryOffset);
        }

        /// <summary>
        /// Validate default values getting stored into compiled data on the parent template.
        /// </summary>
        [Test]
        public unsafe void TemplateDefaultsValidationParent()
        {
            using (var template = new NativeArray<byte>(m_Parent.compiledData.templateSize, Allocator.Temp, NativeArrayOptions.UninitializedMemory))
            {
                m_Parent.compiledData.CopyTemplateTo(template);
                var templatePtr = (byte*) template.GetUnsafeReadOnlyPtr();

                var firstValue = BlackboardUnsafeHelpers.ReadIntegerValue(templatePtr, m_First.compiledData.memoryOffset);
                Assert.AreEqual(k_FirstKeyDefaultValue, firstValue);

                var secondValue = BlackboardUnsafeHelpers.ReadBooleanValue(templatePtr, m_Second.compiledData.memoryOffset);
                Assert.AreEqual(k_SecondKeyDefaultValue, secondValue);
            }
        }

        /// <summary>
        /// Validate default values getting stored into compiled data on the child template.
        /// </summary>
        [Test]
        public unsafe void TemplateDefaultsValidationChild()
        {
            using (var template = new NativeArray<byte>(m_Child.compiledData.templateSize, Allocator.Temp, NativeArrayOptions.UninitializedMemory))
            {
                m_Child.compiledData.CopyTemplateTo(template);
                var templatePtr = (byte*) template.GetUnsafeReadOnlyPtr();

                var firstValue = BlackboardUnsafeHelpers.ReadIntegerValue(templatePtr, m_First.compiledData.memoryOffset);
                Assert.AreEqual(k_FirstKeyDefaultValue, firstValue);

                var secondValue = BlackboardUnsafeHelpers.ReadBooleanValue(templatePtr, m_Second.compiledData.memoryOffset);
                Assert.AreEqual(k_SecondKeyDefaultValue, secondValue);

                var thirdValue = BlackboardUnsafeHelpers.ReadVectorValue(templatePtr, m_Third.compiledData.memoryOffset);
                Assert.AreEqual(s_ThirdKeyDefaultValue, thirdValue);

                var fourthValue = BlackboardUnsafeHelpers.ReadObjectValue(templatePtr, m_Fourth.compiledData.memoryOffset);
                Assert.AreEqual(m_FourthKeyDefaultValue, fourthValue);
            }
        }

        /// <summary>
        /// Confirm that instance synced keys are correctly synced between child and parent templates.
        /// </summary>
        [Test]
        public unsafe void InstanceSyncedKeysSynchronizedBetweenChildAndParentTemplatesCheck()
        {
            using (var template = new NativeArray<byte>(m_Child.compiledData.templateSize, Allocator.Temp, NativeArrayOptions.UninitializedMemory))
            {
                var templatePtr = (byte*) template.GetUnsafeReadOnlyPtr();

                var secondKeyCompiledData = m_Second.compiledData;
                m_Parent.compiledData.SetInstanceSyncedBooleanValueWithoutValidation(
                    in secondKeyCompiledData,
                    !k_SecondKeyDefaultValue);

                m_Parent.compiledData.CopyTemplateTo(template);
                Assert.AreEqual(!k_SecondKeyDefaultValue, BlackboardUnsafeHelpers.ReadBooleanValue(templatePtr, secondKeyCompiledData.memoryOffset),
                    "Update not applied to the parent.");

                m_Child.compiledData.CopyTemplateTo(template);
                Assert.AreEqual(!k_SecondKeyDefaultValue, BlackboardUnsafeHelpers.ReadBooleanValue(templatePtr, secondKeyCompiledData.memoryOffset),
                    "Mismatch between child and parent values.");

                m_Child.compiledData.SetInstanceSyncedBooleanValueWithoutValidation(
                    in secondKeyCompiledData,
                    k_SecondKeyDefaultValue);

                m_Parent.compiledData.CopyTemplateTo(template);
                Assert.AreEqual(k_SecondKeyDefaultValue, BlackboardUnsafeHelpers.ReadBooleanValue(templatePtr, secondKeyCompiledData.memoryOffset),
                    "Update not applied to the parent.");

                m_Child.compiledData.CopyTemplateTo(template);
                Assert.AreEqual(k_SecondKeyDefaultValue, BlackboardUnsafeHelpers.ReadBooleanValue(templatePtr, secondKeyCompiledData.memoryOffset),
                    "Mismatch between child and parent values.");
            }
        }

        /// <summary>
        /// Validate the GetOwningTemplate() function, which gets the owning template of a key.
        /// </summary>
        [Test]
        public void OwningTemplateValidation()
        {
            var parentData = m_Parent.compiledData;
            var childData = m_Child.compiledData;

            Assert.IsTrue(parentData.GetOwningTemplate(k_FirstKeyMemoryOffset) == parentData, "Culprit: Parent. Key: first.");
            Assert.IsTrue(childData.GetOwningTemplate(k_FirstKeyMemoryOffset) == parentData, "Culprit: Child. Key: first.");

            Assert.IsTrue(parentData.GetOwningTemplate(k_SecondKeyMemoryOffset) == parentData, "Culprit: Parent. Key: second.");
            Assert.IsTrue(childData.GetOwningTemplate(k_SecondKeyMemoryOffset) == parentData, "Culprit: Child. Key: second.");

            Assert.IsTrue(parentData.GetOwningTemplate(k_ThirdKeyMemoryOffset) == null, "Culprit: Parent. Key: third.");
            Assert.IsTrue(childData.GetOwningTemplate(k_ThirdKeyMemoryOffset) == childData, "Culprit: Child. Key: third.");

            Assert.IsTrue(parentData.GetOwningTemplate(k_FourthKeyMemoryOffset) == null, "Culprit: Parent. Key: fourth.");
            Assert.IsTrue(childData.GetOwningTemplate(k_FourthKeyMemoryOffset) == childData, "Culprit: Child. Key: fourth.");

            Assert.IsTrue(parentData.GetOwningTemplate(parentData.templateSize) == null, "Culprit: Parent. Key: template size.");
            Assert.IsTrue(childData.GetOwningTemplate(childData.templateSize) == null, "Culprit: Child. Key: template size.");

            Assert.IsTrue(parentData.GetOwningTemplate((ushort) (parentData.templateSize + 5)) == null, "Culprit: Parent. Key: template size + 5.");
            Assert.IsTrue(childData.GetOwningTemplate((ushort) (childData.templateSize + 5)) == null, "Culprit: Child. Key: template size + 5.");
        }
    }
}