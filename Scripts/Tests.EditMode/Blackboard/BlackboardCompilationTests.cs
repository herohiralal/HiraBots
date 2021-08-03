using NUnit.Framework;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
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
        private const int k_FourthKeyMemoryOffset = 5;
        private const int k_ThirdKeyMemoryOffset = 9;
        private const int k_ParentTemplateSize = 5;
        private const int k_ChildTemplateSize = 21;
        private const int k_ParentKeyCount = 2;
        private const int k_ChildKeyCount = 4;
        private const int k_FirstKeyDefaultValue = 0;
        private const bool k_SecondKeyDefaultValue = true;
        private static readonly Vector3 s_ThirdKeyDefaultValue = Vector3.one;
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

            m_First = IntegerBlackboardKey.Build<IntegerBlackboardKey>(k_FirstKeyName, k_FirstKeyTraits, k_FirstKeyDefaultValue, HideFlags.HideAndDontSave);

            m_Second = BooleanBlackboardKey.Build<BooleanBlackboardKey>(k_SecondKeyName, k_SecondKeyTraits, k_SecondKeyDefaultValue, HideFlags.HideAndDontSave);

            m_Third = VectorBlackboardKey.Build<VectorBlackboardKey>(k_ThirdKeyName, k_ThirdKeyTraits, s_ThirdKeyDefaultValue, HideFlags.HideAndDontSave);

            m_Fourth = ObjectBlackboardKey.Build<ObjectBlackboardKey>(k_FourthKeyName, k_FourthKeyTraits, m_FourthKeyDefaultValue, HideFlags.HideAndDontSave);

            m_Parent = BlackboardTemplate.Build<BlackboardTemplate>(k_ParentTemplateName, null,
                new[] {m_First, m_Second}, HideFlags.HideAndDontSave);
            m_Child = BlackboardTemplate.Build<BlackboardTemplate>(k_ChildTemplateName, m_Parent,
                new[] {m_Third, m_Fourth}, HideFlags.HideAndDontSave);


            // validate the blackboard templates first
            {
                var validatorContext = new BlackboardTemplateValidatorContext();

                m_Parent.Validate(validatorContext);
                Assert.IsTrue(validatorContext.m_Validated, "Parent blackboard template couldn't be validated. Test incomplete.");

                validatorContext.Reset();

                m_Child.Validate(validatorContext);
                Assert.IsTrue(validatorContext.m_Validated, "Child blackboard template couldn't be validated. Test incomplete.");

                validatorContext.Reset();
            }

            // compile
            {
                var compilerContext = new BlackboardTemplateCompilerContext();
                m_Parent.Compile(compilerContext);
                compilerContext.Update();
                m_Child.Compile(compilerContext);
                compilerContext.Update();
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
                a.m_MemoryOffset == b.m_MemoryOffset
                && a.m_Index == b.m_Index
                && a.m_Traits == b.m_Traits
                && a.m_KeyType == b.m_KeyType;
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
            Assert.AreEqual(k_FirstKeyIndex, m_First.compiledData.m_Index);
            Assert.AreEqual(k_SecondKeyIndex, m_Second.compiledData.m_Index);
            Assert.AreEqual(k_ThirdKeyIndex, m_Third.compiledData.m_Index);
            Assert.AreEqual(k_FourthKeyIndex, m_Fourth.compiledData.m_Index);
        }

        /// <summary>
        /// Cross-check key type between provided data and compiled data.
        /// </summary>
        [Test]
        public void KeyTypeSyncValidation()
        {
            Assert.AreEqual(k_FirstKeyType, m_First.compiledData.m_KeyType);
            Assert.AreEqual(k_SecondKeyType, m_Second.compiledData.m_KeyType);
            Assert.AreEqual(k_ThirdKeyType, m_Third.compiledData.m_KeyType);
            Assert.AreEqual(k_FourthKeyType, m_Fourth.compiledData.m_KeyType);
        }

        /// <summary>
        /// Cross-check key traits between provided data and compiled data.
        /// </summary>
        [Test]
        public void KeyTraitsSyncValidation()
        {
            Assert.AreEqual(k_FirstKeyTraits, m_First.compiledData.m_Traits);
            Assert.AreEqual(k_SecondKeyTraits, m_Second.compiledData.m_Traits);
            Assert.AreEqual(k_ThirdKeyTraits, m_Third.compiledData.m_Traits);
            Assert.AreEqual(k_FourthKeyTraits, m_Fourth.compiledData.m_Traits);
        }

        /// <summary>
        /// Validate key counts.
        /// </summary>
        [Test]
        public void KeyCountValidation()
        {
            Assert.AreEqual(k_ParentKeyCount, m_Parent.compiledData.m_KeyCount);
            Assert.AreEqual(k_ChildKeyCount, m_Child.compiledData.m_KeyCount);
        }

        /// <summary>
        /// Validate template sizes.
        /// </summary>
        [Test]
        public void TemplateSizeValidation()
        {
            Assert.AreEqual(k_ParentTemplateSize, m_Parent.compiledData.templateSize, "Child template size mismatch.");
            Assert.AreEqual(k_ChildTemplateSize, m_Child.compiledData.templateSize, "Child template size mismatch.");
        }

        /// <summary>
        /// Validate memory offsets.
        /// </summary>
        [Test]
        public void MemoryOffsetValidation()
        {
            // blackboard keys get sorted according to their sizes
            Assert.AreEqual(k_SecondKeyMemoryOffset, m_Second.compiledData.m_MemoryOffset);
            Assert.AreEqual(k_FirstKeyMemoryOffset, m_First.compiledData.m_MemoryOffset);
            Assert.AreEqual(k_FourthKeyMemoryOffset, m_Fourth.compiledData.m_MemoryOffset);
            Assert.AreEqual(k_ThirdKeyMemoryOffset, m_Third.compiledData.m_MemoryOffset);
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

                var firstValue = BlackboardUnsafeHelpers.ReadIntegerValue(templatePtr, m_First.compiledData.m_MemoryOffset);
                Assert.AreEqual(k_FirstKeyDefaultValue, firstValue);

                var secondValue = BlackboardUnsafeHelpers.ReadBooleanValue(templatePtr, m_Second.compiledData.m_MemoryOffset);
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

                var firstValue = BlackboardUnsafeHelpers.ReadIntegerValue(templatePtr, m_First.compiledData.m_MemoryOffset);
                Assert.AreEqual(k_FirstKeyDefaultValue, firstValue);

                var secondValue = BlackboardUnsafeHelpers.ReadBooleanValue(templatePtr, m_Second.compiledData.m_MemoryOffset);
                Assert.AreEqual(k_SecondKeyDefaultValue, secondValue);

                var thirdValue = BlackboardUnsafeHelpers.ReadVectorValue(templatePtr, m_Third.compiledData.m_MemoryOffset);
                Assert.AreEqual(s_ThirdKeyDefaultValue, thirdValue);

                var fourthValue = BlackboardUnsafeHelpers.ReadObjectValue(templatePtr, m_Fourth.compiledData.m_MemoryOffset);
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
                Assert.AreEqual(!k_SecondKeyDefaultValue, BlackboardUnsafeHelpers.ReadBooleanValue(templatePtr, secondKeyCompiledData.m_MemoryOffset),
                    "Update not applied to the parent.");

                m_Child.compiledData.CopyTemplateTo(template);
                Assert.AreEqual(!k_SecondKeyDefaultValue, BlackboardUnsafeHelpers.ReadBooleanValue(templatePtr, secondKeyCompiledData.m_MemoryOffset),
                    "Mismatch between child and parent values.");

                m_Child.compiledData.SetInstanceSyncedBooleanValueWithoutValidation(
                    in secondKeyCompiledData,
                    k_SecondKeyDefaultValue);

                m_Parent.compiledData.CopyTemplateTo(template);
                Assert.AreEqual(k_SecondKeyDefaultValue, BlackboardUnsafeHelpers.ReadBooleanValue(templatePtr, secondKeyCompiledData.m_MemoryOffset),
                    "Update not applied to the parent.");

                m_Child.compiledData.CopyTemplateTo(template);
                Assert.AreEqual(k_SecondKeyDefaultValue, BlackboardUnsafeHelpers.ReadBooleanValue(templatePtr, secondKeyCompiledData.m_MemoryOffset),
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