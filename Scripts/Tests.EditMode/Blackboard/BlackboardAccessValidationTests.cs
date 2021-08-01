using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using static HiraBots.BlackboardComponent;
using Object = UnityEngine.Object;

namespace HiraBots.Editor.Tests
{
    [TestFixture]
    public class BlackboardAccessValidationTests : BlackboardAccessTestBase
    {
        [ExposedToHiraBots("7E11BA86-1F8E-4010-92C2-FC9482B52507")]
        public enum PowerType : sbyte
        {
            Fire, Air, Earth, Water
        }

        private enum LongEnum : long
        {
        }

        [OneTimeSetUp]
        public new void SetUp() => base.SetUp();

        [OneTimeTearDown]
        public new void TearDown() => base.TearDown();

        private const string invalid_key_name = "E1E5B7FC-5BE4-45BA-A84E-95821BBA9662";

        private const ushort invalid_memory_offset = ushort.MaxValue;

        [Test]
        public void InvalidKeyHandlingOnBlackboardComponent()
        {
            TryCreate(ElementalistTemplate, out var baboon);

            InvalidGetterTest<string, bool, KeyNotFoundException>(baboon.GetBooleanValue, invalid_key_name,
                "Invalid Boolean key name getter test failed.");
            InvalidGetterTest<ushort, bool, KeyNotFoundException>(baboon.GetBooleanValue, invalid_memory_offset,
                "Invalid Boolean memory offset getter test failed.");
            InvalidSetterTest<string, bool, KeyNotFoundException>(baboon.SetBooleanValue, invalid_key_name,
                "Invalid Boolean key name setter test failed.");
            InvalidSetterTest<ushort, bool, KeyNotFoundException>(baboon.SetBooleanValue, invalid_memory_offset,
                "Invalid Boolean memory offset setter test failed.");

            InvalidGetterTest<string, GenericStatus, KeyNotFoundException>(baboon.GetEnumValue<GenericStatus>, invalid_key_name,
                "Invalid Enum key name getter test failed.");
            InvalidGetterTest<ushort, GenericStatus, KeyNotFoundException>(baboon.GetEnumValue<GenericStatus>, invalid_memory_offset,
                "Invalid Enum memory offset getter test failed.");
            InvalidSetterTest<string, GenericStatus, KeyNotFoundException>(baboon.SetEnumValue<GenericStatus>, invalid_key_name,
                "Invalid Enum key name setter test failed.");
            InvalidSetterTest<ushort, GenericStatus, KeyNotFoundException>(baboon.SetEnumValue<GenericStatus>, invalid_memory_offset,
                "Invalid Enum memory offset setter test failed.");

            InvalidGetterTest<string, float, KeyNotFoundException>(baboon.GetFloatValue, invalid_key_name,
                "Invalid Float key name getter test failed.");
            InvalidGetterTest<ushort, float, KeyNotFoundException>(baboon.GetFloatValue, invalid_memory_offset,
                "Invalid Float memory offset getter test failed.");
            InvalidSetterTest<string, float, KeyNotFoundException>(baboon.SetFloatValue, invalid_key_name,
                "Invalid Float key name setter test failed.");
            InvalidSetterTest<ushort, float, KeyNotFoundException>(baboon.SetFloatValue, invalid_memory_offset,
                "Invalid Float memory offset setter test failed.");

            InvalidGetterTest<string, int, KeyNotFoundException>(baboon.GetIntegerValue, invalid_key_name,
                "Invalid Integer key name getter test failed.");
            InvalidGetterTest<ushort, int, KeyNotFoundException>(baboon.GetIntegerValue, invalid_memory_offset,
                "Invalid Integer memory offset getter test failed.");
            InvalidSetterTest<string, int, KeyNotFoundException>(baboon.SetIntegerValue, invalid_key_name,
                "Invalid Integer key name setter test failed.");
            InvalidSetterTest<ushort, int, KeyNotFoundException>(baboon.SetIntegerValue, invalid_memory_offset,
                "Invalid Integer memory offset setter test failed.");

            InvalidGetterTest<string, Object, KeyNotFoundException>(baboon.GetObjectValue, invalid_key_name,
                "Invalid Object key name getter test failed.");
            InvalidGetterTest<ushort, Object, KeyNotFoundException>(baboon.GetObjectValue, invalid_memory_offset,
                "Invalid Object memory offset getter test failed.");
            InvalidSetterTest<string, Object, KeyNotFoundException>(baboon.SetObjectValue, invalid_key_name,
                "Invalid Object key name setter test failed.");
            InvalidSetterTest<ushort, Object, KeyNotFoundException>(baboon.SetObjectValue, invalid_memory_offset,
                "Invalid Object memory offset setter test failed.");

            InvalidGetterTest<string, Vector3, KeyNotFoundException>(baboon.GetVectorValue, invalid_key_name,
                "Invalid Vector key name getter test failed.");
            InvalidGetterTest<ushort, Vector3, KeyNotFoundException>(baboon.GetVectorValue, invalid_memory_offset,
                "Invalid Vector memory offset getter test failed.");
            InvalidSetterTest<string, Vector3, KeyNotFoundException>(baboon.SetVectorValue, invalid_key_name,
                "Invalid Vector key name setter test failed.");
            InvalidSetterTest<ushort, Vector3, KeyNotFoundException>(baboon.SetVectorValue, invalid_memory_offset,
                "Invalid Vector memory offset setter test failed.");

            InvalidGetterTest<string, Quaternion, KeyNotFoundException>(baboon.GetQuaternionValue, invalid_key_name,
                "Invalid Quaternion key name getter test failed.");
            InvalidGetterTest<ushort, Quaternion, KeyNotFoundException>(baboon.GetQuaternionValue, invalid_memory_offset,
                "Invalid Quaternion memory offset getter test failed.");
            InvalidSetterTest<string, Quaternion, KeyNotFoundException>(baboon.SetQuaternionValue, invalid_key_name,
                "Invalid Quaternion key name setter test failed.");
            InvalidSetterTest<ushort, Quaternion, KeyNotFoundException>(baboon.SetQuaternionValue, invalid_memory_offset,
                "Invalid Quaternion memory offset setter test failed.");
        }

        [Test]
        public void InvalidTypeAccessHandlingOnBlackboardComponent()
        {
            TryCreate(ElementalistTemplate, out var baboon);

            InvalidGetterTest<string, bool, InvalidCastException>(baboon.GetBooleanValue, "Mana",
                "Invalid type read-access with key name test failed.");
            InvalidGetterTest<ushort, bool, InvalidCastException>(baboon.GetBooleanValue, ManaKeyFloat,
                "Invalid type read-access with memory offset test failed.");
            InvalidSetterTest<string, bool, InvalidCastException>(baboon.SetBooleanValue, "Mana",
                "Invalid type write-access with key name test failed.");
            InvalidSetterTest<ushort, bool, InvalidCastException>(baboon.SetBooleanValue, ManaKeyFloat,
                "Invalid type write-access with memory offset test failed.");
        }

        [Test]
        public void InvalidEnumTypeAccessHandlingOnBlackboardComponent()
        {
            TryCreate(ElementalistTemplate, out var baboon);
            
            InvalidGetterTest<string, LongEnum, OverflowException>(baboon.GetEnumValue<LongEnum>, "PowerType",
                "Invalid enum type read-access with key name test failed.");
            InvalidGetterTest<ushort, LongEnum, OverflowException>(baboon.GetEnumValue<LongEnum>, PowerTypeKeyEnum,
                "Invalid enum type read-access with memory offset test failed.");
            InvalidSetterTest<string, LongEnum, OverflowException>(baboon.SetEnumValue<LongEnum>, "PowerType",
                "Invalid enum type write-access with key name test failed.");
            InvalidSetterTest<ushort, LongEnum, OverflowException>(baboon.SetEnumValue<LongEnum>, PowerTypeKeyEnum,
                "Invalid enum type write-access with memory offset test failed.");
        }

        [Test]
        public void InvalidKeyHandlingOnBlackboardCompiledData()
        {
            InvalidGetterTest<string, bool, KeyNotFoundException>(ElementalistData.GetInstanceSyncedBooleanValue, invalid_key_name,
                "Invalid instance synced Boolean key name getter test failed.");
            InvalidGetterTest<ushort, bool, KeyNotFoundException>(ElementalistData.GetInstanceSyncedBooleanValue, invalid_memory_offset,
                "Invalid instance synced Boolean memory offset getter test failed.");
            InvalidSetterTest<string, bool, KeyNotFoundException>(ElementalistData.SetInstanceSyncedBooleanValue, invalid_key_name,
                "Invalid instance synced Boolean key name setter test failed.");
            InvalidSetterTest<ushort, bool, KeyNotFoundException>(ElementalistData.SetInstanceSyncedBooleanValue, invalid_memory_offset,
                "Invalid instance synced Boolean memory offset setter test failed.");

            InvalidGetterTest<string, GenericStatus, KeyNotFoundException>(ElementalistData.GetInstanceSyncedEnumValue<GenericStatus>, invalid_key_name,
                "Invalid instance synced Enum key name getter test failed.");
            InvalidGetterTest<ushort, GenericStatus, KeyNotFoundException>(ElementalistData.GetInstanceSyncedEnumValue<GenericStatus>, invalid_memory_offset,
                "Invalid instance synced Enum memory offset getter test failed.");
            InvalidSetterTest<string, GenericStatus, KeyNotFoundException>(ElementalistData.SetInstanceSyncedEnumValue<GenericStatus>, invalid_key_name,
                "Invalid instance synced Enum key name setter test failed.");
            InvalidSetterTest<ushort, GenericStatus, KeyNotFoundException>(ElementalistData.SetInstanceSyncedEnumValue<GenericStatus>, invalid_memory_offset,
                "Invalid instance synced Enum memory offset setter test failed.");

            InvalidGetterTest<string, float, KeyNotFoundException>(ElementalistData.GetInstanceSyncedFloatValue, invalid_key_name,
                "Invalid instance synced Float key name getter test failed.");
            InvalidGetterTest<ushort, float, KeyNotFoundException>(ElementalistData.GetInstanceSyncedFloatValue, invalid_memory_offset,
                "Invalid instance synced Float memory offset getter test failed.");
            InvalidSetterTest<string, float, KeyNotFoundException>(ElementalistData.SetInstanceSyncedFloatValue, invalid_key_name,
                "Invalid instance synced Float key name setter test failed.");
            InvalidSetterTest<ushort, float, KeyNotFoundException>(ElementalistData.SetInstanceSyncedFloatValue, invalid_memory_offset,
                "Invalid instance synced Float memory offset setter test failed.");

            InvalidGetterTest<string, int, KeyNotFoundException>(ElementalistData.GetInstanceSyncedIntegerValue, invalid_key_name,
                "Invalid instance synced Integer key name getter test failed.");
            InvalidGetterTest<ushort, int, KeyNotFoundException>(ElementalistData.GetInstanceSyncedIntegerValue, invalid_memory_offset,
                "Invalid instance synced Integer memory offset getter test failed.");
            InvalidSetterTest<string, int, KeyNotFoundException>(ElementalistData.SetInstanceSyncedIntegerValue, invalid_key_name,
                "Invalid instance synced Integer key name setter test failed.");
            InvalidSetterTest<ushort, int, KeyNotFoundException>(ElementalistData.SetInstanceSyncedIntegerValue, invalid_memory_offset,
                "Invalid instance synced Integer memory offset setter test failed.");

            InvalidGetterTest<string, Object, KeyNotFoundException>(ElementalistData.GetInstanceSyncedObjectValue, invalid_key_name,
                "Invalid instance synced Object key name getter test failed.");
            InvalidGetterTest<ushort, Object, KeyNotFoundException>(ElementalistData.GetInstanceSyncedObjectValue, invalid_memory_offset,
                "Invalid instance synced Object memory offset getter test failed.");
            InvalidSetterTest<string, Object, KeyNotFoundException>(ElementalistData.SetInstanceSyncedObjectValue, invalid_key_name,
                "Invalid instance synced Object key name setter test failed.");
            InvalidSetterTest<ushort, Object, KeyNotFoundException>(ElementalistData.SetInstanceSyncedObjectValue, invalid_memory_offset,
                "Invalid instance synced Object memory offset setter test failed.");

            InvalidGetterTest<string, Vector3, KeyNotFoundException>(ElementalistData.GetInstanceSyncedVectorValue, invalid_key_name,
                "Invalid instance synced Vector key name getter test failed.");
            InvalidGetterTest<ushort, Vector3, KeyNotFoundException>(ElementalistData.GetInstanceSyncedVectorValue, invalid_memory_offset,
                "Invalid instance synced Vector memory offset getter test failed.");
            InvalidSetterTest<string, Vector3, KeyNotFoundException>(ElementalistData.SetInstanceSyncedVectorValue, invalid_key_name,
                "Invalid instance synced Vector key name setter test failed.");
            InvalidSetterTest<ushort, Vector3, KeyNotFoundException>(ElementalistData.SetInstanceSyncedVectorValue, invalid_memory_offset,
                "Invalid instance synced Vector memory offset setter test failed.");

            InvalidGetterTest<string, Quaternion, KeyNotFoundException>(ElementalistData.GetInstanceSyncedQuaternionValue, invalid_key_name,
                "Invalid instance synced Quaternion key name getter test failed.");
            InvalidGetterTest<ushort, Quaternion, KeyNotFoundException>(ElementalistData.GetInstanceSyncedQuaternionValue, invalid_memory_offset,
                "Invalid instance synced Quaternion memory offset getter test failed.");
            InvalidSetterTest<string, Quaternion, KeyNotFoundException>(ElementalistData.SetInstanceSyncedQuaternionValue, invalid_key_name,
                "Invalid instance synced Quaternion key name setter test failed.");
            InvalidSetterTest<ushort, Quaternion, KeyNotFoundException>(ElementalistData.SetInstanceSyncedQuaternionValue, invalid_memory_offset,
                "Invalid instance synced Quaternion memory offset setter test failed.");
        }

        [Test]
        public void NonInstanceSyncedKeyHandlingOnBlackboardCompiledData()
        {
            InvalidGetterTest<string, float, InvalidOperationException>(ElementalistData.GetInstanceSyncedFloatValue, "Mana",
                "Non instance synced key read-access with key name test failed.");
            InvalidGetterTest<ushort, float, InvalidOperationException>(ElementalistData.GetInstanceSyncedFloatValue, ManaKeyFloat,
                "Non instance synced key read-access with memory offset test failed.");
            InvalidSetterTest<string, float, InvalidOperationException>(ElementalistData.SetInstanceSyncedFloatValue, "Mana",
                "Non instance synced key write-access with key name test failed.");
            InvalidSetterTest<ushort, float, InvalidOperationException>(ElementalistData.SetInstanceSyncedFloatValue, ManaKeyFloat,
                "Non instance synced key write-access with memory offset test failed.");
        }

        [Test]
        public void InvalidTypeAccessHandlingOnBlackboardCompiledData()
        {
            InvalidGetterTest<string, bool, InvalidCastException>(ElementalistData.GetInstanceSyncedBooleanValue, "CurrentPlayerLocation",
                "Invalid type read-access with key name test failed.");
            InvalidGetterTest<ushort, bool, InvalidCastException>(ElementalistData.GetInstanceSyncedBooleanValue, CurrentPlayerLocationKeyVector,
                "Invalid type read-access with memory offset test failed.");
            InvalidSetterTest<string, bool, InvalidCastException>(ElementalistData.SetInstanceSyncedBooleanValue, "CurrentPlayerLocation",
                "Invalid type write-access with key name test failed.");
            InvalidSetterTest<ushort, bool, InvalidCastException>(ElementalistData.SetInstanceSyncedBooleanValue, CurrentPlayerLocationKeyVector,
                "Invalid type write-access with memory offset test failed.");
        }

        [Test]
        public void InvalidEnumTypeAccessHandlingOnBlackboardCompiledData()
        {
            InvalidGetterTest<string, LongEnum, OverflowException>(ElementalistData.GetInstanceSyncedEnumValue<LongEnum>, "PowerType",
                "Invalid enum type read-access with key name test failed.");
            InvalidGetterTest<ushort, LongEnum, OverflowException>(ElementalistData.GetInstanceSyncedEnumValue<LongEnum>, PowerTypeKeyEnum,
                "Invalid enum type read-access with memory offset test failed.");
            InvalidSetterTest<string, LongEnum, OverflowException>(ElementalistData.SetInstanceSyncedEnumValue<LongEnum>, "PowerType",
                "Invalid enum type write-access with key name test failed.");
            InvalidSetterTest<ushort, LongEnum, OverflowException>(ElementalistData.SetInstanceSyncedEnumValue<LongEnum>, PowerTypeKeyEnum,
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