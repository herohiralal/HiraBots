using UnityEngine;

namespace HiraBots
{
    internal unsafe partial class BlackboardTemplateCompiledData
    {
        internal void SetInstanceSyncedBooleanKeyWithoutValidation(in BlackboardKeyCompiledData keyData, bool value)
        {
            var memoryOffset = keyData.MemoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteBooleanValueAndGetChange(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(in keyData, valuePtr, sizeof(byte));
        }

        internal void SetInstanceSyncedEnumKeyWithoutValidation<T>(in BlackboardKeyCompiledData keyData, T value) where T : unmanaged, System.Enum
        {
            var memoryOffset = keyData.MemoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteEnumValueAndGetChange<T>(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(in keyData, valuePtr, (ushort) sizeof(T));
        }

        internal void SetInstanceSyncedFloatKeyWithoutValidation(in BlackboardKeyCompiledData keyData, float value)
        {
            var memoryOffset = keyData.MemoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteFloatValueAndGetChange(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(in keyData, valuePtr, sizeof(float));
        }

        internal void SetInstanceSyncedIntegerKeyWithoutValidation(in BlackboardKeyCompiledData keyData, int value)
        {
            var memoryOffset = keyData.MemoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteIntegerValueAndGetChange(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(in keyData, valuePtr, sizeof(int));
        }

        internal void SetInstanceSyncedObjectKeyWithoutValidation(in BlackboardKeyCompiledData keyData, Object value)
        {
            var memoryOffset = keyData.MemoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteObjectValueAndGetChange(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(in keyData, valuePtr, sizeof(int));
        }

        internal void SetInstanceSyncedVectorKeyWithoutValidation(in BlackboardKeyCompiledData keyData, Vector3 value)
        {
            var memoryOffset = keyData.MemoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteVectorValueAndGetChange(owningTemplate.TemplatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.TemplateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate._listeners)
                listener.UpdateValue(in keyData, valuePtr, sizeof(float) * 3);
        }

        internal void SetInstanceSyncedQuaternionKeyWithoutValidation(in BlackboardKeyCompiledData keyData, Quaternion value)
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