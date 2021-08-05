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
        public void SetUp()
        {
            base.SetUp(false);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            base.TearDown(false);
        }

        private const string k_InvalidKeyName = "E1E5B7FC-5BE4-45BA-A84E-95821BBA9662";

        private const ushort k_InvalidMemoryOffset = ushort.MaxValue;

        /// <summary>
        /// Check how the blackboard handles when the caller passes in an invalid key name or an invalid memory offset.
        /// </summary>
        [Test]
        public void InvalidKeyHandlingOnBlackboardComponent()
        {
            TryCreate(m_ElementalistTemplate, out var baboon);

            InvalidGetterTest<string, bool, KeyNotFoundException>(baboon.GetBooleanValue, k_InvalidKeyName,
                "Invalid Boolean key name getter test failed.");
            InvalidGetterTest<ushort, bool, KeyNotFoundException>(baboon.GetBooleanValue, k_InvalidMemoryOffset,
                "Invalid Boolean memory offset getter test failed.");
            InvalidSetterTest<string, bool, KeyNotFoundException>(baboon.SetBooleanValue, k_InvalidKeyName,
                "Invalid Boolean key name setter test failed.");
            InvalidSetterTest<ushort, bool, KeyNotFoundException>(baboon.SetBooleanValue, k_InvalidMemoryOffset,
                "Invalid Boolean memory offset setter test failed.");

            InvalidGetterTest<string, GenericStatus, KeyNotFoundException>(baboon.GetEnumValue<GenericStatus>, k_InvalidKeyName,
                "Invalid Enum key name getter test failed.");
            InvalidGetterTest<ushort, GenericStatus, KeyNotFoundException>(baboon.GetEnumValue<GenericStatus>, k_InvalidMemoryOffset,
                "Invalid Enum memory offset getter test failed.");
            InvalidSetterTest<string, GenericStatus, KeyNotFoundException>(baboon.SetEnumValue<GenericStatus>, k_InvalidKeyName,
                "Invalid Enum key name setter test failed.");
            InvalidSetterTest<ushort, GenericStatus, KeyNotFoundException>(baboon.SetEnumValue<GenericStatus>, k_InvalidMemoryOffset,
                "Invalid Enum memory offset setter test failed.");

            InvalidGetterTest<string, float, KeyNotFoundException>(baboon.GetFloatValue, k_InvalidKeyName,
                "Invalid Float key name getter test failed.");
            InvalidGetterTest<ushort, float, KeyNotFoundException>(baboon.GetFloatValue, k_InvalidMemoryOffset,
                "Invalid Float memory offset getter test failed.");
            InvalidSetterTest<string, float, KeyNotFoundException>(baboon.SetFloatValue, k_InvalidKeyName,
                "Invalid Float key name setter test failed.");
            InvalidSetterTest<ushort, float, KeyNotFoundException>(baboon.SetFloatValue, k_InvalidMemoryOffset,
                "Invalid Float memory offset setter test failed.");

            InvalidGetterTest<string, int, KeyNotFoundException>(baboon.GetIntegerValue, k_InvalidKeyName,
                "Invalid Integer key name getter test failed.");
            InvalidGetterTest<ushort, int, KeyNotFoundException>(baboon.GetIntegerValue, k_InvalidMemoryOffset,
                "Invalid Integer memory offset getter test failed.");
            InvalidSetterTest<string, int, KeyNotFoundException>(baboon.SetIntegerValue, k_InvalidKeyName,
                "Invalid Integer key name setter test failed.");
            InvalidSetterTest<ushort, int, KeyNotFoundException>(baboon.SetIntegerValue, k_InvalidMemoryOffset,
                "Invalid Integer memory offset setter test failed.");

            InvalidGetterTest<string, Object, KeyNotFoundException>(baboon.GetObjectValue, k_InvalidKeyName,
                "Invalid Object key name getter test failed.");
            InvalidGetterTest<ushort, Object, KeyNotFoundException>(baboon.GetObjectValue, k_InvalidMemoryOffset,
                "Invalid Object memory offset getter test failed.");
            InvalidSetterTest<string, Object, KeyNotFoundException>(baboon.SetObjectValue, k_InvalidKeyName,
                "Invalid Object key name setter test failed.");
            InvalidSetterTest<ushort, Object, KeyNotFoundException>(baboon.SetObjectValue, k_InvalidMemoryOffset,
                "Invalid Object memory offset setter test failed.");

            InvalidGetterTest<string, float3, KeyNotFoundException>(baboon.GetVectorValue, k_InvalidKeyName,
                "Invalid Vector key name getter test failed.");
            InvalidGetterTest<ushort, float3, KeyNotFoundException>(baboon.GetVectorValue, k_InvalidMemoryOffset,
                "Invalid Vector memory offset getter test failed.");
            InvalidSetterTest<string, float3, KeyNotFoundException>(baboon.SetVectorValue, k_InvalidKeyName,
                "Invalid Vector key name setter test failed.");
            InvalidSetterTest<ushort, float3, KeyNotFoundException>(baboon.SetVectorValue, k_InvalidMemoryOffset,
                "Invalid Vector memory offset setter test failed.");

            InvalidGetterTest<string, quaternion, KeyNotFoundException>(baboon.GetQuaternionValue, k_InvalidKeyName,
                "Invalid Quaternion key name getter test failed.");
            InvalidGetterTest<ushort, quaternion, KeyNotFoundException>(baboon.GetQuaternionValue, k_InvalidMemoryOffset,
                "Invalid Quaternion memory offset getter test failed.");
            InvalidSetterTest<string, quaternion, KeyNotFoundException>(baboon.SetQuaternionValue, k_InvalidKeyName,
                "Invalid Quaternion key name setter test failed.");
            InvalidSetterTest<ushort, quaternion, KeyNotFoundException>(baboon.SetQuaternionValue, k_InvalidMemoryOffset,
                "Invalid Quaternion memory offset setter test failed.");
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
            InvalidGetterTest<ushort, bool, InvalidCastException>(baboon.GetBooleanValue, manaKeyFloat,
                "Invalid type read-access with memory offset test failed.");
            InvalidSetterTest<string, bool, InvalidCastException>(baboon.SetBooleanValue, "Mana",
                "Invalid type write-access with key name test failed.");
            InvalidSetterTest<ushort, bool, InvalidCastException>(baboon.SetBooleanValue, manaKeyFloat,
                "Invalid type write-access with memory offset test failed.");
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
            InvalidGetterTest<ushort, LongEnum, OverflowException>(baboon.GetEnumValue<LongEnum>, powerTypeKeyEnum,
                "Invalid enum type read-access with memory offset test failed.");
            InvalidSetterTest<string, LongEnum, OverflowException>(baboon.SetEnumValue<LongEnum>, "PowerType",
                "Invalid enum type write-access with key name test failed.");
            InvalidSetterTest<ushort, LongEnum, OverflowException>(baboon.SetEnumValue<LongEnum>, powerTypeKeyEnum,
                "Invalid enum type write-access with memory offset test failed.");
        }

        /// <summary>
        /// Check how the blackboard template handles when the caller passes in an invalid key name or an invalid memory offset.
        /// </summary>
        [Test]
        public void InvalidKeyHandlingOnBlackboardCompiledData()
        {
            InvalidGetterTest<string, bool, KeyNotFoundException>(elementalistData.GetInstanceSyncedBooleanValue, k_InvalidKeyName,
                "Invalid instance synced Boolean key name getter test failed.");
            InvalidGetterTest<ushort, bool, KeyNotFoundException>(elementalistData.GetInstanceSyncedBooleanValue, k_InvalidMemoryOffset,
                "Invalid instance synced Boolean memory offset getter test failed.");
            InvalidSetterTest<string, bool, KeyNotFoundException>(elementalistData.SetInstanceSyncedBooleanValue, k_InvalidKeyName,
                "Invalid instance synced Boolean key name setter test failed.");
            InvalidSetterTest<ushort, bool, KeyNotFoundException>(elementalistData.SetInstanceSyncedBooleanValue, k_InvalidMemoryOffset,
                "Invalid instance synced Boolean memory offset setter test failed.");

            InvalidGetterTest<string, GenericStatus, KeyNotFoundException>(elementalistData.GetInstanceSyncedEnumValue<GenericStatus>, k_InvalidKeyName,
                "Invalid instance synced Enum key name getter test failed.");
            InvalidGetterTest<ushort, GenericStatus, KeyNotFoundException>(elementalistData.GetInstanceSyncedEnumValue<GenericStatus>, k_InvalidMemoryOffset,
                "Invalid instance synced Enum memory offset getter test failed.");
            InvalidSetterTest<string, GenericStatus, KeyNotFoundException>(elementalistData.SetInstanceSyncedEnumValue<GenericStatus>, k_InvalidKeyName,
                "Invalid instance synced Enum key name setter test failed.");
            InvalidSetterTest<ushort, GenericStatus, KeyNotFoundException>(elementalistData.SetInstanceSyncedEnumValue<GenericStatus>, k_InvalidMemoryOffset,
                "Invalid instance synced Enum memory offset setter test failed.");

            InvalidGetterTest<string, float, KeyNotFoundException>(elementalistData.GetInstanceSyncedFloatValue, k_InvalidKeyName,
                "Invalid instance synced Float key name getter test failed.");
            InvalidGetterTest<ushort, float, KeyNotFoundException>(elementalistData.GetInstanceSyncedFloatValue, k_InvalidMemoryOffset,
                "Invalid instance synced Float memory offset getter test failed.");
            InvalidSetterTest<string, float, KeyNotFoundException>(elementalistData.SetInstanceSyncedFloatValue, k_InvalidKeyName,
                "Invalid instance synced Float key name setter test failed.");
            InvalidSetterTest<ushort, float, KeyNotFoundException>(elementalistData.SetInstanceSyncedFloatValue, k_InvalidMemoryOffset,
                "Invalid instance synced Float memory offset setter test failed.");

            InvalidGetterTest<string, int, KeyNotFoundException>(elementalistData.GetInstanceSyncedIntegerValue, k_InvalidKeyName,
                "Invalid instance synced Integer key name getter test failed.");
            InvalidGetterTest<ushort, int, KeyNotFoundException>(elementalistData.GetInstanceSyncedIntegerValue, k_InvalidMemoryOffset,
                "Invalid instance synced Integer memory offset getter test failed.");
            InvalidSetterTest<string, int, KeyNotFoundException>(elementalistData.SetInstanceSyncedIntegerValue, k_InvalidKeyName,
                "Invalid instance synced Integer key name setter test failed.");
            InvalidSetterTest<ushort, int, KeyNotFoundException>(elementalistData.SetInstanceSyncedIntegerValue, k_InvalidMemoryOffset,
                "Invalid instance synced Integer memory offset setter test failed.");

            InvalidGetterTest<string, Object, KeyNotFoundException>(elementalistData.GetInstanceSyncedObjectValue, k_InvalidKeyName,
                "Invalid instance synced Object key name getter test failed.");
            InvalidGetterTest<ushort, Object, KeyNotFoundException>(elementalistData.GetInstanceSyncedObjectValue, k_InvalidMemoryOffset,
                "Invalid instance synced Object memory offset getter test failed.");
            InvalidSetterTest<string, Object, KeyNotFoundException>(elementalistData.SetInstanceSyncedObjectValue, k_InvalidKeyName,
                "Invalid instance synced Object key name setter test failed.");
            InvalidSetterTest<ushort, Object, KeyNotFoundException>(elementalistData.SetInstanceSyncedObjectValue, k_InvalidMemoryOffset,
                "Invalid instance synced Object memory offset setter test failed.");

            InvalidGetterTest<string, float3, KeyNotFoundException>(elementalistData.GetInstanceSyncedVectorValue, k_InvalidKeyName,
                "Invalid instance synced Vector key name getter test failed.");
            InvalidGetterTest<ushort, float3, KeyNotFoundException>(elementalistData.GetInstanceSyncedVectorValue, k_InvalidMemoryOffset,
                "Invalid instance synced Vector memory offset getter test failed.");
            InvalidSetterTest<string, float3, KeyNotFoundException>(elementalistData.SetInstanceSyncedVectorValue, k_InvalidKeyName,
                "Invalid instance synced Vector key name setter test failed.");
            InvalidSetterTest<ushort, float3, KeyNotFoundException>(elementalistData.SetInstanceSyncedVectorValue, k_InvalidMemoryOffset,
                "Invalid instance synced Vector memory offset setter test failed.");

            InvalidGetterTest<string, quaternion, KeyNotFoundException>(elementalistData.GetInstanceSyncedQuaternionValue, k_InvalidKeyName,
                "Invalid instance synced Quaternion key name getter test failed.");
            InvalidGetterTest<ushort, quaternion, KeyNotFoundException>(elementalistData.GetInstanceSyncedQuaternionValue, k_InvalidMemoryOffset,
                "Invalid instance synced Quaternion memory offset getter test failed.");
            InvalidSetterTest<string, quaternion, KeyNotFoundException>(elementalistData.SetInstanceSyncedQuaternionValue, k_InvalidKeyName,
                "Invalid instance synced Quaternion key name setter test failed.");
            InvalidSetterTest<ushort, quaternion, KeyNotFoundException>(elementalistData.SetInstanceSyncedQuaternionValue, k_InvalidMemoryOffset,
                "Invalid instance synced Quaternion memory offset setter test failed.");
        }

        /// <summary>
        /// Check how the blackboard template handles when the caller passes in a key that's not instance-synchronized.
        /// </summary>
        [Test]
        public void NonInstanceSyncedKeyHandlingOnBlackboardCompiledData()
        {
            InvalidGetterTest<string, float, InvalidOperationException>(elementalistData.GetInstanceSyncedFloatValue, "Mana",
                "Non instance synced key read-access with key name test failed.");
            InvalidGetterTest<ushort, float, InvalidOperationException>(elementalistData.GetInstanceSyncedFloatValue, manaKeyFloat,
                "Non instance synced key read-access with memory offset test failed.");
            InvalidSetterTest<string, float, InvalidOperationException>(elementalistData.SetInstanceSyncedFloatValue, "Mana",
                "Non instance synced key write-access with key name test failed.");
            InvalidSetterTest<ushort, float, InvalidOperationException>(elementalistData.SetInstanceSyncedFloatValue, manaKeyFloat,
                "Non instance synced key write-access with memory offset test failed.");
        }

        /// <summary>
        /// Check how the blackboard template handles when the caller asks for a wrong type of data.
        /// </summary>
        [Test]
        public void InvalidTypeAccessHandlingOnBlackboardCompiledData()
        {
            InvalidGetterTest<string, bool, InvalidCastException>(elementalistData.GetInstanceSyncedBooleanValue, "CurrentPlayerLocation",
                "Invalid type read-access with key name test failed.");
            InvalidGetterTest<ushort, bool, InvalidCastException>(elementalistData.GetInstanceSyncedBooleanValue, currentPlayerLocationKeyVector,
                "Invalid type read-access with memory offset test failed.");
            InvalidSetterTest<string, bool, InvalidCastException>(elementalistData.SetInstanceSyncedBooleanValue, "CurrentPlayerLocation",
                "Invalid type write-access with key name test failed.");
            InvalidSetterTest<ushort, bool, InvalidCastException>(elementalistData.SetInstanceSyncedBooleanValue, currentPlayerLocationKeyVector,
                "Invalid type write-access with memory offset test failed.");
        }

        /// <summary>
        /// Check how the blackboard template handles when the caller asks for an invalid enum type.
        /// </summary>
        [Test]
        public void InvalidEnumTypeAccessHandlingOnBlackboardCompiledData()
        {
            InvalidGetterTest<string, LongEnum, OverflowException>(elementalistData.GetInstanceSyncedEnumValue<LongEnum>, "PowerType",
                "Invalid enum type read-access with key name test failed.");
            InvalidGetterTest<ushort, LongEnum, OverflowException>(elementalistData.GetInstanceSyncedEnumValue<LongEnum>, powerTypeKeyEnum,
                "Invalid enum type read-access with memory offset test failed.");
            InvalidSetterTest<string, LongEnum, OverflowException>(elementalistData.SetInstanceSyncedEnumValue<LongEnum>, "PowerType",
                "Invalid enum type write-access with key name test failed.");
            InvalidSetterTest<ushort, LongEnum, OverflowException>(elementalistData.SetInstanceSyncedEnumValue<LongEnum>, powerTypeKeyEnum,
                "Invalid enum type write-access with memory offset test failed.");
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