using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HiraBots
{
    internal unsafe interface IInstanceSynchronizerListener
    {
        void UpdateValue(in BlackboardKeyCompiledData keyData, byte* value, ushort size);
    }

    internal unsafe partial class BlackboardTemplateCompiledData : IInstanceSynchronizerListener
    {
        void IInstanceSynchronizerListener.UpdateValue(in BlackboardKeyCompiledData keyData, byte* value, ushort size)
        {
            var ptr = TemplatePtr + keyData.MemoryOffset;
            for (var i = 0; i < size; i++) ptr[i] = value[i];

            foreach (var listener in _listeners)
                listener.UpdateValue(in keyData, value, size);
        }
    }

    internal unsafe partial class BlackboardTemplateCompiledData
    {
        private readonly List<IInstanceSynchronizerListener> _listeners = new List<IInstanceSynchronizerListener>();

        internal BlackboardTemplateCompiledData GetOwningTemplate(ushort memoryOffset)
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

        internal void UpdateInstanceSyncedBooleanKeyWithoutValidation(in BlackboardKeyCompiledData keyData, bool value)
        {
            var memoryOffset = keyData.MemoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteBooleanValueAndGetChange(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(in keyData, valuePtr, sizeof(byte));
        }

        internal void UpdateInstanceSyncedEnumKeyWithoutValidation<T>(in BlackboardKeyCompiledData keyData, T value) where T : unmanaged, Enum
        {
            var memoryOffset = keyData.MemoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteEnumValueAndGetChange<T>(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(in keyData, valuePtr, (ushort) sizeof(T));
        }

        internal void UpdateInstanceSyncedFloatKeyWithoutValidation(in BlackboardKeyCompiledData keyData, float value)
        {
            var memoryOffset = keyData.MemoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteFloatValueAndGetChange(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(in keyData, valuePtr, sizeof(float));
        }

        internal void UpdateInstanceSyncedIntegerKeyWithoutValidation(in BlackboardKeyCompiledData keyData, int value)
        {
            var memoryOffset = keyData.MemoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteIntegerValueAndGetChange(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(in keyData, valuePtr, sizeof(int));
        }

        internal void UpdateInstanceSyncedObjectKeyWithoutValidation(in BlackboardKeyCompiledData keyData, Object value)
        {
            var memoryOffset = keyData.MemoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteObjectValueAndGetChange(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(in keyData, valuePtr, sizeof(int));
        }

        internal void UpdateInstanceSyncedVectorKeyWithoutValidation(in BlackboardKeyCompiledData keyData, Vector3 value)
        {
            var memoryOffset = keyData.MemoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteVectorValueAndGetChange(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(in keyData, valuePtr, sizeof(float) * 3);
        }

        internal void UpdateInstanceSyncedQuaternionKeyWithoutValidation(in BlackboardKeyCompiledData keyData, Quaternion value)
        {
            var memoryOffset = keyData.MemoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteQuaternionValueAndGetChange(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(in keyData, valuePtr, sizeof(float) * 4);
        }
    }
}