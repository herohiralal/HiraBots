using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HiraBots
{
    internal static unsafe class BlackboardUnsafeHelpers
    {
        private const MethodImplOptions inline = MethodImplOptions.AggressiveInlining;

        #region Generic

        [MethodImpl(inline)]
        private static T ReadGenericValue<T>(byte* stream, ushort offset) where T : unmanaged =>
            *(T*) (stream + offset);

        [MethodImpl(inline)]
        private static bool WriteGenericValueAndGetChange<T>(byte* stream, ushort offset, T value) where T : unmanaged
        {
            var address = stream + offset;
            var valuePtr = (byte*) &value;

            var changed = false;
            for (var i = 0; i < sizeof(T); i++) changed |= (address[i] != valuePtr[i]);

            *(T*) address = value;

            return changed;
        }

        [MethodImpl(inline)]
        private static void WriteGenericValue<T>(byte* stream, ushort offset, T value) where T : unmanaged =>
            *(T*) (stream + offset) = value;

        #endregion

        #region Booelan

        [MethodImpl(inline)]
        internal static bool ReadBooleanValue(byte* stream, ushort offset) =>
            ReadGenericValue<byte>(stream, offset).ToBoolean();

        [MethodImpl(inline)]
        internal static bool WriteBooleanValueAndGetChange(byte* stream, ushort offset, bool value) =>
            WriteGenericValueAndGetChange<byte>(stream, offset, value.ToByte());

        [MethodImpl(inline)]
        internal static void WriteBooleanValue(byte* stream, ushort offset, bool value) =>
            WriteGenericValue<byte>(stream, offset, value.ToByte());

        #endregion

        #region Enum

        [MethodImpl(inline)]
        internal static T ReadEnumValue<T>(byte* stream, ushort offset) where T : unmanaged, Enum =>
            ReadGenericValue<T>(stream, offset);

        [MethodImpl(inline)]
        internal static bool WriteEnumValueAndGetChange<T>(byte* stream, ushort offset, T value) where T : unmanaged, Enum =>
            WriteGenericValueAndGetChange<T>(stream, offset, value);

        [MethodImpl(inline)]
        internal static void WriteEnumValue<T>(byte* stream, ushort offset, T value) where T : unmanaged, Enum =>
            WriteGenericValue<T>(stream, offset, value);

        #endregion

        #region Float

        [MethodImpl(inline)]
        internal static float ReadFloatValue(byte* stream, ushort offset) =>
            ReadGenericValue<float>(stream, offset);

        [MethodImpl(inline)]
        internal static bool WriteFloatValueAndGetChange(byte* stream, ushort offset, float value) =>
            WriteGenericValueAndGetChange<float>(stream, offset, value);

        [MethodImpl(inline)]
        internal static void WriteFloatValue(byte* stream, ushort offset, float value) =>
            WriteGenericValue<float>(stream, offset, value);

        #endregion

        #region Integer

        [MethodImpl(inline)]
        internal static int ReadIntegerValue(byte* stream, ushort offset) =>
            ReadGenericValue<int>(stream, offset);

        [MethodImpl(inline)]
        internal static bool WriteIntegerValueAndGetChange(byte* stream, ushort offset, int value) =>
            WriteGenericValueAndGetChange<int>(stream, offset, value);

        [MethodImpl(inline)]
        internal static void WriteIntegerValue(byte* stream, ushort offset, int value) =>
            WriteGenericValue<int>(stream, offset, value);

        #endregion

        #region Object

#if UNITY_2020_3_OR_NEWER // 2020.3 or newer can use Resources.InstanceIDToObject directly.
        [MethodImpl(inline)]
        internal static void ClearObjectCache()
        {
        }

        [MethodImpl(inline)]
        internal static void Pin(Object _)
        {
        }

        [MethodImpl(inline)]
        internal static void Release(Object _)
        {
        }

        [MethodImpl(inline)]
        internal static Object ReadObjectValue(byte* stream, ushort offset)
        {
            var instanceID = ReadIntegerValue(stream, offset);
            return (instanceID) == 0 ? null : Resources.InstanceIDToObject(instanceID);
        }

        [MethodImpl(inline)]
        internal static bool WriteObjectValueAndGetChange(byte* stream, ushort offset, Object value) =>
            WriteIntegerValueAndGetChange(stream, offset, value == null ? 0 : value.GetInstanceID());

        [MethodImpl(inline)]
        internal static bool WriteObjectValueNoProcessAndGetChange(byte* stream, ushort offset, Object value) =>
            WriteIntegerValueAndGetChange(stream, offset, value == null ? 0 : value.GetInstanceID());

        [MethodImpl(inline)]
        internal static void WriteObjectValue(byte* stream, ushort offset, Object value) =>
            WriteIntegerValue(stream, offset, value == null ? 0 : value.GetInstanceID());

        [MethodImpl(inline)]
        internal static void WriteObjectValueNoProcess(byte* stream, ushort offset, Object value) =>
            WriteIntegerValue(stream, offset, value == null ? 0 : value.GetInstanceID());
#else
        private static readonly UnityObjectCache object_cache = new UnityObjectCache(200);

        internal static void ClearObjectCache() => object_cache.Clear();

        [MethodImpl(inline)]
        internal static void Pin(Object target) => object_cache.SetFreeze(target.GetInstanceID(), true);

        [MethodImpl(inline)]
        internal static void Release(Object target) => object_cache.SetFreeze(target.GetInstanceID(), false);

        [MethodImpl(inline)]
        internal static Object ReadObjectValue(byte* stream, ushort offset) =>
            object_cache.Read(ReadIntegerValue(stream, offset));

        [MethodImpl(inline)]
        internal static bool WriteObjectValueAndGetChange(byte* stream, ushort offset, Object value)
        {
            object_cache.Remove(ReadIntegerValue(stream, offset));
            return WriteIntegerValueAndGetChange(stream, offset, object_cache.Process(value));
        }

        [MethodImpl(inline)]
        internal static bool WriteObjectValueNoProcessAndGetChange(byte* stream, ushort offset, Object value) =>
            WriteIntegerValueAndGetChange(stream, offset, value.GetInstanceID());

        [MethodImpl(inline)]
        internal static void WriteObjectValue(byte* stream, ushort offset, Object value)
        {
            object_cache.Remove(ReadIntegerValue(stream, offset));
            WriteIntegerValue(stream, offset, object_cache.Process(value));
        }

        [MethodImpl(inline)]
        internal static void WriteObjectValueNoProcess(byte* stream, ushort offset, Object value) =>
            WriteIntegerValue(stream, offset, value.GetInstanceID());
#endif

        #endregion

        #region Vector

        [MethodImpl(inline)]
        internal static Vector3 ReadVectorValue(byte* stream, ushort offset) =>
            new Vector3
            (
                ReadFloatValue(stream, offset),
                ReadFloatValue(stream, (ushort) (offset + sizeof(float))),
                ReadFloatValue(stream, (ushort) (offset + sizeof(float) + sizeof(float)))
            );

        [MethodImpl(inline)]
        internal static bool WriteVectorValueAndGetChange(byte* stream, ushort offset, Vector3 value)
        {
            var x = WriteFloatValueAndGetChange(stream, offset, value.x);
            var y = WriteFloatValueAndGetChange(stream, (ushort) (offset + sizeof(float)), value.y);
            var z = WriteFloatValueAndGetChange(stream, (ushort) (offset + sizeof(float) + sizeof(float)), value.z);
            return x || y || z;
        }

        [MethodImpl(inline)]
        internal static void WriteVectorValue(byte* stream, ushort offset, Vector3 value)
        {
            WriteFloatValue(stream, offset, value.x);
            WriteFloatValue(stream, (ushort) (offset + sizeof(float)), value.y);
            WriteFloatValue(stream, (ushort) (offset + sizeof(float) + sizeof(float)), value.z);
        }

        #endregion

        #region Quaternion

        [MethodImpl(inline)]
        internal static Quaternion ReadQuaternionValue(byte* stream, ushort offset) =>
            new Quaternion
            (
                ReadFloatValue(stream, offset),
                ReadFloatValue(stream, (ushort) (offset + sizeof(float))),
                ReadFloatValue(stream, (ushort) (offset + sizeof(float) + sizeof(float))),
                ReadFloatValue(stream, (ushort) (offset + sizeof(float) + sizeof(float) + sizeof(float)))
            );

        [MethodImpl(inline)]
        internal static bool WriteQuaternionValueAndGetChange(byte* stream, ushort offset, Quaternion value)
        {
            var x = WriteFloatValueAndGetChange(stream, offset, value.x);
            var y = WriteFloatValueAndGetChange(stream, (ushort) (offset + sizeof(float)), value.y);
            var z = WriteFloatValueAndGetChange(stream, (ushort) (offset + sizeof(float) + sizeof(float)), value.z);
            var w = WriteFloatValueAndGetChange(stream, (ushort) (offset + sizeof(float) + sizeof(float) + sizeof(float)), value.w);
            return x || y || z || w;
        }

        [MethodImpl(inline)]
        internal static void WriteQuaternionValue(byte* stream, ushort offset, Quaternion value)
        {
            WriteFloatValue(stream, offset, value.x);
            WriteFloatValue(stream, (ushort) (offset + sizeof(float)), value.y);
            WriteFloatValue(stream, (ushort) (offset + sizeof(float) + sizeof(float)), value.z);
            WriteFloatValue(stream, (ushort) (offset + sizeof(float) + sizeof(float) + sizeof(float)), value.w);
        }

        #endregion
    }
}