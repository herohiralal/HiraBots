using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Object = UnityEngine.Object;

namespace HiraBots
{
    /// <summary>
    /// Helper functions to access blackboards at a low-level.
    /// </summary>
    internal static unsafe class BlackboardUnsafeHelpers
    {
        private const MethodImplOptions k_Inline = MethodImplOptions.AggressiveInlining;

        #region Generic

        // read a value using generics
        [MethodImpl(k_Inline)]
        private static T ReadGenericValue<T>(byte* stream, ushort offset) where T : unmanaged
        {
            return *(T*) (stream + offset);
        }

        // write a value using generics and determine whether it has changed from before
        [MethodImpl(k_Inline)]
        private static bool WriteGenericValueAndGetChange<T>(byte* stream, ushort offset, T value) where T : unmanaged
        {
            var address = stream + offset;
            var valuePtr = (byte*) &value;

            var changed = false;
            for (var i = 0; i < sizeof(T); i++)
            {
                changed |= (address[i] != valuePtr[i]);
            }

            *(T*) address = value;

            return changed;
        }

        // write a value using generics
        [MethodImpl(k_Inline)]
        private static void WriteGenericValue<T>(byte* stream, ushort offset, T value) where T : unmanaged
        {
            *(T*) (stream + offset) = value;
        }

        #endregion

        #region Booelan

        /// <summary>
        /// Read Boolean value from a memory stream.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static bool ReadBooleanValue(byte* stream, ushort offset)
        {
            return ReadGenericValue<bool>(stream, offset);
        }

        /// <summary>
        /// Write Boolean value to a memory stream and determine whether it has changed from before.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static bool WriteBooleanValueAndGetChange(byte* stream, ushort offset, bool value)
        {
            return WriteGenericValueAndGetChange<bool>(stream, offset, value);
        }

        /// <summary>
        /// Write Boolean value to a memory stream.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static void WriteBooleanValue(byte* stream, ushort offset, bool value)
        {
            WriteGenericValue<bool>(stream, offset, value);
        }

        #endregion

        #region Enum

        /// <summary>
        /// Read Enum value from a memory stream.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static T ReadEnumValue<T>(byte* stream, ushort offset) where T : unmanaged, Enum
        {
            return ReadGenericValue<T>(stream, offset);
        }

        /// <summary>
        /// Write Enum value to a memory stream and determine whether it has changed from before.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static bool WriteEnumValueAndGetChange<T>(byte* stream, ushort offset, T value) where T : unmanaged, Enum
        {
            return WriteGenericValueAndGetChange<T>(stream, offset, value);
        }

        /// <summary>
        /// Write Enum value to a memory stream.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static void WriteEnumValue<T>(byte* stream, ushort offset, T value) where T : unmanaged, Enum
        {
            WriteGenericValue<T>(stream, offset, value);
        }

        #endregion

        #region Float

        /// <summary>
        /// Read Float value from a memory stream.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static float ReadFloatValue(byte* stream, ushort offset)
        {
            return ReadGenericValue<float>(stream, offset);
        }

        /// <summary>
        /// Write Float value to a memory stream and determine whether it has changed from before.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static bool WriteFloatValueAndGetChange(byte* stream, ushort offset, float value)
        {
            return WriteGenericValueAndGetChange<float>(stream, offset, value);
        }

        /// <summary>
        /// Write Float value to a memory stream.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static void WriteFloatValue(byte* stream, ushort offset, float value)
        {
            WriteGenericValue<float>(stream, offset, value);
        }

        #endregion

        #region Integer

        /// <summary>
        /// Read Integer value from a memory stream.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static int ReadIntegerValue(byte* stream, ushort offset)
        {
            return ReadGenericValue<int>(stream, offset);
        }

        /// <summary>
        /// Write Integer value to a memory stream and determine whether it has changed from before.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static bool WriteIntegerValueAndGetChange(byte* stream, ushort offset, int value)
        {
            return WriteGenericValueAndGetChange<int>(stream, offset, value);
        }

        /// <summary>
        /// Write Integer value to a memory stream.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static void WriteIntegerValue(byte* stream, ushort offset, int value)
        {
            WriteGenericValue<int>(stream, offset, value);
        }

        #endregion

        #region Object

#if UNITY_2020_3_OR_NEWER // 2020.3 or newer can use Resources.InstanceIDToObject directly.

        /// <summary>
        /// Get Object from InstanceID.
        /// </summary>
        internal static Object InstanceIDToObject(int input)
        {
            return UnityEngine.Resources.InstanceIDToObject(input);
        }

        /// <summary>
        /// Get InstanceID from Object.
        /// </summary>
        internal static int ObjectToInstanceID(Object input)
        {
            return input.GetInstanceID();
        }

        /// <summary>
        /// Remove existing handle from object cache.
        /// </summary>
        internal static void RemoveInstanceFromObjectCache(int _)
        {
        }

        /// <summary>
        /// No-op replacement for a 2019.4 function.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static void ClearObjectCache()
        {
        }

        /// <summary>
        /// No-op replacement for a 2019.4 function.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static void Pin(Object _)
        {
        }

        /// <summary>
        /// No-op replacement for a 2019.4 function.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static void Release(Object _)
        {
        }

        /// <summary>
        /// Read Object value from a memory stream.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static Object ReadObjectValue(byte* stream, ushort offset)
        {
            var instanceID = ReadIntegerValue(stream, offset);
            return (instanceID) == 0 ? null : UnityEngine.Resources.InstanceIDToObject(instanceID);
        }

        /// <summary>
        /// Write Object value to a memory stream and determine whether it has changed from before.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static bool WriteObjectValueAndGetChange(byte* stream, ushort offset, Object value)
        {
            return WriteIntegerValueAndGetChange(stream, offset, value == null ? 0 : value.GetInstanceID());
        }

        /// <summary>
        /// Write Object value to a memory stream without caching it and determine whether it has changed from before.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static bool WriteObjectValueNoProcessAndGetChange(byte* stream, ushort offset, Object value)
        {
            return WriteIntegerValueAndGetChange(stream, offset, value == null ? 0 : value.GetInstanceID());
        }

        /// <summary>
        /// Write Object value to a memory stream.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static void WriteObjectValue(byte* stream, ushort offset, Object value)
        {
            WriteIntegerValue(stream, offset, value == null ? 0 : value.GetInstanceID());
        }

        /// <summary>
        /// Write Object value to a memory stream without caching it.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static void WriteObjectValueNoProcess(byte* stream, ushort offset, Object value)
        {
            WriteIntegerValue(stream, offset, value == null ? 0 : value.GetInstanceID());
        }
#else
        // the object cache utility
        private static readonly UnityObjectCache s_ObjectCache = new UnityObjectCache(200);

        /// <summary>
        /// Get Object from InstanceID.
        /// </summary>
        internal static Object InstanceIDToObject(int input)
        {
            return s_ObjectCache.Read(input);
        }

        /// <summary>
        /// Get InstanceID from Object.
        /// </summary>
        internal static int ObjectToInstanceID(Object input)
        {
            return s_ObjectCache.Process(input);
        }

        /// <summary>
        /// Remove existing handle from object cache.
        /// </summary>
        internal static void RemoveInstanceFromObjectCache(int instanceID)
        {
            s_ObjectCache.Remove(instanceID);
        }

        /// <summary>
        /// Clear the Unity object cache.
        /// </summary>
        internal static void ClearObjectCache() => s_ObjectCache.Clear();

        /// <summary>
        /// Freeze the reference counting on an object.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static void Pin(Object target)
        {
            s_ObjectCache.SetFreeze(target == null ? 0 : target.GetInstanceID(), true);
        }

        /// <summary>
        /// Unfreeze the reference counting on an object.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static void Release(Object target)
        {
            s_ObjectCache.SetFreeze(target == null ? 0 : target.GetInstanceID(), false);
        }

        /// <summary>
        /// Read Object value from a memory stream.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static Object ReadObjectValue(byte* stream, ushort offset) =>
            s_ObjectCache.Read(ReadIntegerValue(stream, offset));

        /// <summary>
        /// Write Object value to a memory stream and determine whether it has changed from before.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static bool WriteObjectValueAndGetChange(byte* stream, ushort offset, Object value)
        {
            s_ObjectCache.Remove(ReadIntegerValue(stream, offset));
            return WriteIntegerValueAndGetChange(stream, offset, s_ObjectCache.Process(value));
        }

        /// <summary>
        /// Write Object value to a memory stream without caching it and determine whether it has changed from before.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static bool WriteObjectValueNoProcessAndGetChange(byte* stream, ushort offset, Object value)
        {
            return WriteIntegerValueAndGetChange(stream, offset, value == null ? 0 : value.GetInstanceID());
        }

        /// <summary>
        /// Write Object value to a memory stream.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static void WriteObjectValue(byte* stream, ushort offset, Object value)
        {
            s_ObjectCache.Remove(ReadIntegerValue(stream, offset));
            WriteIntegerValue(stream, offset, s_ObjectCache.Process(value));
        }

        /// <summary>
        /// Write Object value to a memory stream without caching it.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static void WriteObjectValueNoProcess(byte* stream, ushort offset, Object value)
        {
            WriteIntegerValue(stream, offset, value == null ? 0 : value.GetInstanceID());
        }
#endif

        #endregion

        #region Vector

        /// <summary>
        /// Read Vector value from a memory stream.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static float3 ReadVectorValue(byte* stream, ushort offset)
        {
            return ReadGenericValue<float3>(stream, offset);
        }

        /// <summary>
        /// Write Vector value to a memory stream and determine whether it has changed from before.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static bool WriteVectorValueAndGetChange(byte* stream, ushort offset, float3 value)
        {
            return WriteGenericValueAndGetChange<float3>(stream, offset, value);
        }

        /// <summary>
        /// Write Vector value to a memory stream.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static void WriteVectorValue(byte* stream, ushort offset, float3 value)
        {
            WriteGenericValue<float3>(stream, offset, value);
        }

        #endregion

        #region Quaternion

        /// <summary>
        /// Read Quaternion value from a memory stream.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static quaternion ReadQuaternionValue(byte* stream, ushort offset)
        {
            return ReadGenericValue<quaternion>(stream, offset);
        }

        /// <summary>
        /// Write Quaternion value to a memory stream and determine whether it has changed from before.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static bool WriteQuaternionValueAndGetChange(byte* stream, ushort offset, quaternion value)
        {
            return WriteGenericValueAndGetChange<quaternion>(stream, offset, value);
        }

        /// <summary>
        /// Write Quaternion value to a memory stream.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static void WriteQuaternionValue(byte* stream, ushort offset, quaternion value)
        {
            WriteGenericValue<quaternion>(stream, offset, value);
        }

        #endregion
    }
}