using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HiraBots
{
    internal unsafe interface IInstanceSynchronizerListener
    {
        void UpdateValue(ushort memoryOffset, byte* value, ushort size, bool broadcastEventOnUnexpectedChange);
    }

    internal unsafe partial class BlackboardTemplateCompiledData : IInstanceSynchronizerListener
    {
        public void UpdateValue(ushort memoryOffset, byte* value, ushort size, bool broadcastEventOnUnexpectedChange)
        {
            var ptr = TemplatePtr + memoryOffset;
            for (var i = 0; i < size; i++) ptr[i] = value[i];

            foreach (var listener in _listeners)
                listener.UpdateValue(memoryOffset, value, size, broadcastEventOnUnexpectedChange);
        }
    }

    internal unsafe partial class BlackboardTemplateCompiledData
    {
        private readonly List<IInstanceSynchronizerListener> _listeners = new List<IInstanceSynchronizerListener>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void AddListener(IInstanceSynchronizerListener listener)
        {
            if (!_listeners.Contains(listener)) _listeners.Add(listener);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RemoveListener(IInstanceSynchronizerListener listener)
        {
            if (_listeners.Contains(listener)) _listeners.Remove(listener);
        }

        internal void UpdateInstanceSyncedBooleanKey(bool broadcastEventOnUnexpectedChange, ushort memoryOffset, bool value)
        {
            BlackboardUnsafeHelpers.WriteBooleanValue(TemplatePtr, memoryOffset, value);

            var valuePtr = TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in _listeners)
                listener.UpdateValue(memoryOffset, valuePtr, sizeof(byte), broadcastEventOnUnexpectedChange);
        }

        internal void UpdateInstanceSyncedEnumKey<T>(bool broadcastEventOnUnexpectedChange, ushort memoryOffset, T value) where T : unmanaged, Enum
        {
            BlackboardUnsafeHelpers.WriteEnumValue<T>(TemplatePtr, memoryOffset, value);

            var valuePtr = TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in _listeners)
                listener.UpdateValue(memoryOffset, valuePtr, (ushort) sizeof(T), broadcastEventOnUnexpectedChange);
        }

        internal void UpdateInstanceSyncedFloatKey(bool broadcastEventOnUnexpectedChange, ushort memoryOffset, float value)
        {
            BlackboardUnsafeHelpers.WriteFloatValue(TemplatePtr, memoryOffset, value);

            var valuePtr = TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in _listeners)
                listener.UpdateValue(memoryOffset, valuePtr, sizeof(float), broadcastEventOnUnexpectedChange);
        }

        internal void UpdateInstanceSyncedIntegerKey(bool broadcastEventOnUnexpectedChange, ushort memoryOffset, int value)
        {
            BlackboardUnsafeHelpers.WriteIntegerValue(TemplatePtr, memoryOffset, value);

            var valuePtr = TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in _listeners)
                listener.UpdateValue(memoryOffset, valuePtr, sizeof(int), broadcastEventOnUnexpectedChange);
        }

        internal void UpdateInstanceSyncedObjectKey(bool broadcastEventOnUnexpectedChange, ushort memoryOffset, Object value)
        {
            BlackboardUnsafeHelpers.WriteObjectValue(TemplatePtr, memoryOffset, value);

            var valuePtr = TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in _listeners)
                listener.UpdateValue(memoryOffset, valuePtr, sizeof(int), broadcastEventOnUnexpectedChange);
        }

        internal void UpdateInstanceSyncedVectorKey(bool broadcastEventOnUnexpectedChange, ushort memoryOffset, Vector3 value)
        {
            BlackboardUnsafeHelpers.WriteVectorValue(TemplatePtr, memoryOffset, value);

            var valuePtr = TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in _listeners)
                listener.UpdateValue(memoryOffset, valuePtr, sizeof(float) * 3, broadcastEventOnUnexpectedChange);
        }

        internal void UpdateInstanceSyncedQuaternionKey(bool broadcastEventOnUnexpectedChange, ushort memoryOffset, Quaternion value)
        {
            BlackboardUnsafeHelpers.WriteQuaternionValue(TemplatePtr, memoryOffset, value);

            var valuePtr = TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in _listeners)
                listener.UpdateValue(memoryOffset, valuePtr, sizeof(float) * 4, broadcastEventOnUnexpectedChange);
        }
    }
}