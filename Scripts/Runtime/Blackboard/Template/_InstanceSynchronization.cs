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

        private BlackboardTemplateCompiledData GetOwningTemplate(ushort memoryOffset)
        {
            var current = this;
            var previous = (BlackboardTemplateCompiledData) null;

            do
            {
                if (current.TemplateSize <= memoryOffset) return previous;
                
                previous = current;
                current = current._parentCompiledData;
            } while (current != null);

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void AddInstanceSyncListener(IInstanceSynchronizerListener listener)
        {
            if (!_listeners.Contains(listener)) _listeners.Add(listener);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RemoveInstanceSyncListener(IInstanceSynchronizerListener listener)
        {
            if (_listeners.Contains(listener)) _listeners.Remove(listener);
        }

        internal void UpdateInstanceSyncedBooleanKey(bool broadcastEventOnUnexpectedChange, ushort memoryOffset, bool value)
        {
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteBooleanValue(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(memoryOffset, valuePtr, sizeof(byte), broadcastEventOnUnexpectedChange);
        }

        internal void UpdateInstanceSyncedEnumKey<T>(bool broadcastEventOnUnexpectedChange, ushort memoryOffset, T value) where T : unmanaged, Enum
        {
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteEnumValue<T>(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(memoryOffset, valuePtr, (ushort) sizeof(T), broadcastEventOnUnexpectedChange);
        }

        internal void UpdateInstanceSyncedFloatKey(bool broadcastEventOnUnexpectedChange, ushort memoryOffset, float value)
        {
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteFloatValue(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(memoryOffset, valuePtr, sizeof(float), broadcastEventOnUnexpectedChange);
        }

        internal void UpdateInstanceSyncedIntegerKey(bool broadcastEventOnUnexpectedChange, ushort memoryOffset, int value)
        {
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteIntegerValue(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(memoryOffset, valuePtr, sizeof(int), broadcastEventOnUnexpectedChange);
        }

        internal void UpdateInstanceSyncedObjectKey(bool broadcastEventOnUnexpectedChange, ushort memoryOffset, Object value)
        {
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteObjectValue(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(memoryOffset, valuePtr, sizeof(int), broadcastEventOnUnexpectedChange);
        }

        internal void UpdateInstanceSyncedVectorKey(bool broadcastEventOnUnexpectedChange, ushort memoryOffset, Vector3 value)
        {
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteVectorValue(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(memoryOffset, valuePtr, sizeof(float) * 3, broadcastEventOnUnexpectedChange);
        }

        internal void UpdateInstanceSyncedQuaternionKey(bool broadcastEventOnUnexpectedChange, ushort memoryOffset, Quaternion value)
        {
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteQuaternionValue(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(memoryOffset, valuePtr, sizeof(float) * 4, broadcastEventOnUnexpectedChange);
        }
    }
}