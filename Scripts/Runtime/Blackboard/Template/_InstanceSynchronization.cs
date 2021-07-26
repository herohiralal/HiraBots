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
        void IInstanceSynchronizerListener.UpdateValue(ushort memoryOffset, byte* value, ushort size, bool broadcastEventOnUnexpectedChange)
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

            return previous;
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

        internal void UpdateInstanceSyncedBooleanKeyWithoutValidation(bool broadcastEventOnUnexpectedChange, ushort memoryOffset, bool value)
        {
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteBooleanValueAndGetChange(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(memoryOffset, valuePtr, sizeof(byte), broadcastEventOnUnexpectedChange);
        }

        internal void UpdateInstanceSyncedEnumKeyWithoutValidation<T>(bool broadcastEventOnUnexpectedChange, ushort memoryOffset, T value) where T : unmanaged, Enum
        {
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteEnumValueAndGetChange<T>(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(memoryOffset, valuePtr, (ushort) sizeof(T), broadcastEventOnUnexpectedChange);
        }

        internal void UpdateInstanceSyncedFloatKeyWithoutValidation(bool broadcastEventOnUnexpectedChange, ushort memoryOffset, float value)
        {
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteFloatValueAndGetChange(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(memoryOffset, valuePtr, sizeof(float), broadcastEventOnUnexpectedChange);
        }

        internal void UpdateInstanceSyncedIntegerKeyWithoutValidation(bool broadcastEventOnUnexpectedChange, ushort memoryOffset, int value)
        {
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteIntegerValueAndGetChange(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(memoryOffset, valuePtr, sizeof(int), broadcastEventOnUnexpectedChange);
        }

        internal void UpdateInstanceSyncedObjectKeyWithoutValidation(bool broadcastEventOnUnexpectedChange, ushort memoryOffset, Object value)
        {
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteObjectValueAndGetChange(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(memoryOffset, valuePtr, sizeof(int), broadcastEventOnUnexpectedChange);
        }

        internal void UpdateInstanceSyncedVectorKeyWithoutValidation(bool broadcastEventOnUnexpectedChange, ushort memoryOffset, Vector3 value)
        {
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteVectorValueAndGetChange(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(memoryOffset, valuePtr, sizeof(float) * 3, broadcastEventOnUnexpectedChange);
        }

        internal void UpdateInstanceSyncedQuaternionKeyWithoutValidation(bool broadcastEventOnUnexpectedChange, ushort memoryOffset, Quaternion value)
        {
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteQuaternionValueAndGetChange(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(memoryOffset, valuePtr, sizeof(float) * 4, broadcastEventOnUnexpectedChange);
        }
    }
}