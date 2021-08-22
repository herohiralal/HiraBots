using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using static HiraBots.BlackboardComponent;
using Object = UnityEngine.Object;

namespace HiraBots.Editor.Tests
{
    /// <summary>
    /// Tests for validation checks of data input into a blackbaord.
    /// </summary>
    [TestFixture]
    internal class BlackboardAccessValidationTests : BlackboardAccessTestBase
    {
        [OneTimeSetUp]
        public virtual void SetUp()
        {
            base.SetUp(false);
        }

        [OneTimeTearDown]
        public virtual void TearDown()
        {
            base.TearDown(false);
        }

        private const string k_InvalidKeyName = "E1E5B7FC-5BE4-45BA-A84E-95821BBA9662";

        /// <summary>
        /// Check how the blackboard handles when the caller passes in an invalid key name or an invalid memory offset.
        /// </summary>
        [Test]
        public void InvalidKeyHandlingOnBlackboardComponent()
        {
            TryCreate(m_ElementalistTemplate, out var baboon);

            InvalidGetterTest<string, bool, KeyNotFoundException>(baboon.GetBooleanValue, k_InvalidKeyName,
                "Invalid Boolean key name getter test failed.");
            InvalidSetterTest<string, bool, KeyNotFoundException>(baboon.SetBooleanValue, k_InvalidKeyName,
                "Invalid Boolean key name setter test failed.");

            InvalidGetterTest<string, GenericStatus, KeyNotFoundException>(baboon.GetEnumValue<GenericStatus>, k_InvalidKeyName,
                "Invalid Enum key name getter test failed.");
            InvalidSetterTest<string, GenericStatus, KeyNotFoundException>(baboon.SetEnumValue<GenericStatus>, k_InvalidKeyName,
                "Invalid Enum key name setter test failed.");

            InvalidGetterTest<string, float, KeyNotFoundException>(baboon.GetFloatValue, k_InvalidKeyName,
                "Invalid Float key name getter test failed.");
            InvalidSetterTest<string, float, KeyNotFoundException>(baboon.SetFloatValue, k_InvalidKeyName,
                "Invalid Float key name setter test failed.");

            InvalidGetterTest<string, int, KeyNotFoundException>(baboon.GetIntegerValue, k_InvalidKeyName,
                "Invalid Integer key name getter test failed.");
            InvalidSetterTest<string, int, KeyNotFoundException>(baboon.SetIntegerValue, k_InvalidKeyName,
                "Invalid Integer key name setter test failed.");

            InvalidGetterTest<string, Object, KeyNotFoundException>(baboon.GetObjectValue, k_InvalidKeyName,
                "Invalid Object key name getter test failed.");
            InvalidSetterTest<string, Object, KeyNotFoundException>(baboon.SetObjectValue, k_InvalidKeyName,
                "Invalid Object key name setter test failed.");

            InvalidGetterTest<string, float3, KeyNotFoundException>(baboon.GetVectorValue, k_InvalidKeyName,
                "Invalid Vector key name getter test failed.");
            InvalidSetterTest<string, float3, KeyNotFoundException>(baboon.SetVectorValue, k_InvalidKeyName,
                "Invalid Vector key name setter test failed.");

            InvalidGetterTest<string, quaternion, KeyNotFoundException>(baboon.GetQuaternionValue, k_InvalidKeyName,
                "Invalid Quaternion key name getter test failed.");
            InvalidSetterTest<string, quaternion, KeyNotFoundException>(baboon.SetQuaternionValue, k_InvalidKeyName,
                "Invalid Quaternion key name setter test failed.");
        }

        /// <summary>
        /// Check how the blackboard handles when the caller asks for a wrong type of data.
        /// </summary>
        [Test]
        public void InvalidTypeAccessHandlingOnBlackboardComponent()
        {
            TryCreate(m_ElementalistTemplate, out var baboon);

            InvalidGetterTest<string, bool, InvalidCastException>(baboon.GetBooleanValue, "Mana",
                "Invalid type read-access with key name test failed.");
            InvalidSetterTest<string, bool, InvalidCastException>(baboon.SetBooleanValue, "Mana",
                "Invalid type write-access with key name test failed.");
        }

        /// <summary>
        /// Check how the blackboard handles when the caller asks for an invalid enum type.
        /// </summary>
        [Test]
        public void InvalidEnumTypeAccessHandlingOnBlackboardComponent()
        {
            TryCreate(m_ElementalistTemplate, out var baboon);
            
            InvalidGetterTest<string, LongEnum, OverflowException>(baboon.GetEnumValue<LongEnum>, "PowerType",
                "Invalid enum type read-access with key name test failed.");
            InvalidSetterTest<string, LongEnum, OverflowException>(baboon.SetEnumValue<LongEnum>, "PowerType",
                "Invalid enum type write-access with key name test failed.");
        }

        /// <summary>
        /// Check how the blackboard template handles when the caller passes in an invalid key name or an invalid memory offset.
        /// </summary>
        [Test]
        public void InvalidKeyHandlingOnBlackboardCompiledData()
        {
            InvalidGetterTest<string, bool, KeyNotFoundException>(elementalistData.GetInstanceSyncedBooleanValue, k_InvalidKeyName,
                "Invalid instance synced Boolean key name getter test failed.");
            InvalidSetterTest<string, bool, KeyNotFoundException>(elementalistData.SetInstanceSyncedBooleanValue, k_InvalidKeyName,
                "Invalid instance synced Boolean key name setter test failed.");

            InvalidGetterTest<string, GenericStatus, KeyNotFoundException>(elementalistData.GetInstanceSyncedEnumValue<GenericStatus>, k_InvalidKeyName,
                "Invalid instance synced Enum key name getter test failed.");
            InvalidSetterTest<string, GenericStatus, KeyNotFoundException>(elementalistData.SetInstanceSyncedEnumValue<GenericStatus>, k_InvalidKeyName,
                "Invalid instance synced Enum key name setter test failed.");

            InvalidGetterTest<string, float, KeyNotFoundException>(elementalistData.GetInstanceSyncedFloatValue, k_InvalidKeyName,
                "Invalid instance synced Float key name getter test failed.");
            InvalidSetterTest<string, float, KeyNotFoundException>(elementalistData.SetInstanceSyncedFloatValue, k_InvalidKeyName,
                "Invalid instance synced Float key name setter test failed.");

            InvalidGetterTest<string, int, KeyNotFoundException>(elementalistData.GetInstanceSyncedIntegerValue, k_InvalidKeyName,
                "Invalid instance synced Integer key name getter test failed.");
            InvalidSetterTest<string, int, KeyNotFoundException>(elementalistData.SetInstanceSyncedIntegerValue, k_InvalidKeyName,
                "Invalid instance synced Integer key name setter test failed.");

            InvalidGetterTest<string, Object, KeyNotFoundException>(elementalistData.GetInstanceSyncedObjectValue, k_InvalidKeyName,
                "Invalid instance synced Object key name getter test failed.");
            InvalidSetterTest<string, Object, KeyNotFoundException>(elementalistData.SetInstanceSyncedObjectValue, k_InvalidKeyName,
                "Invalid instance synced Object key name setter test failed.");

            InvalidGetterTest<string, float3, KeyNotFoundException>(elementalistData.GetInstanceSyncedVectorValue, k_InvalidKeyName,
                "Invalid instance synced Vector key name getter test failed.");
            InvalidSetterTest<string, float3, KeyNotFoundException>(elementalistData.SetInstanceSyncedVectorValue, k_InvalidKeyName,
                "Invalid instance synced Vector key name setter test failed.");

            InvalidGetterTest<string, quaternion, KeyNotFoundException>(elementalistData.GetInstanceSyncedQuaternionValue, k_InvalidKeyName,
                "Invalid instance synced Quaternion key name getter test failed.");
            InvalidSetterTest<string, quaternion, KeyNotFoundException>(elementalistData.SetInstanceSyncedQuaternionValue, k_InvalidKeyName,
                "Invalid instance synced Quaternion key name setter test failed.");
        }

        /// <summary>
        /// Check how the blackboard template handles when the caller passes in a key that's not instance-synchronized.
        /// </summary>
        [Test]
        public void NonInstanceSyncedKeyHandlingOnBlackboardCompiledData()
        {
            InvalidGetterTest<string, float, InvalidOperationException>(elementalistData.GetInstanceSyncedFloatValue, "Mana",
                "Non instance synced key read-access with key name test failed.");
            InvalidSetterTest<string, float, InvalidOperationException>(elementalistData.SetInstanceSyncedFloatValue, "Mana",
                "Non instance synced key write-access with key name test failed.");
        }

        /// <summary>
        /// Check how the blackboard template handles when the caller asks for a wrong type of data.
        /// </summary>
        [Test]
        public void InvalidTypeAccessHandlingOnBlackboardCompiledData()
        {
            InvalidGetterTest<string, bool, InvalidCastException>(elementalistData.GetInstanceSyncedBooleanValue, "CurrentPlayerLocation",
                "Invalid type read-access with key name test failed.");
            InvalidSetterTest<string, bool, InvalidCastException>(elementalistData.SetInstanceSyncedBooleanValue, "CurrentPlayerLocation",
                "Invalid type write-access with key name test failed.");
        }

        /// <summary>
        /// Check how the blackboard template handles when the caller asks for an invalid enum type.
        /// </summary>
        [Test]
        public void InvalidEnumTypeAccessHandlingOnBlackboardCompiledData()
        {
            InvalidGetterTest<string, LongEnum, OverflowException>(elementalistData.GetInstanceSyncedEnumValue<LongEnum>, "PowerType",
                "Invalid enum type read-access with key name test failed.");
            InvalidSetterTest<string, LongEnum, OverflowException>(elementalistData.SetInstanceSyncedEnumValue<LongEnum>, "PowerType",
                "Invalid enum type write-access with key name test failed.");
        }

        private static void InvalidGetterTest<TKey, TValue, TException>(Func<TKey, TValue> getter, TKey key, string text = "")
            where TException : Exception
        {
            Assert.Catch<TException>(() => getter.Invoke(key), text);
        }

        private static void InvalidSetterTest<TKey, TValue, TException>(Action<TKey, TValue> setter, TKey key, string text = "")
            where TException : Exception
        {
            Assert.Catch<TException>(() => setter.Invoke(key, default), text);
        }

        private static void InvalidSetterTest<TKey, TValue, TException>(Action<TKey, TValue, bool> setter, TKey key, string text = "")
            where TException : Exception
        {
            Assert.Catch<TException>(() => setter.Invoke(key, default, default), text);
        }
    }
}