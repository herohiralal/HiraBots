using UnityEngine;

namespace HiraBots
{
    internal unsafe partial class BlackboardTemplateCompiledData
    {
        internal void SetInstanceSyncedBooleanValueWithoutValidation(in BlackboardKeyCompiledData keyData, bool value)
        {
            var memoryOffset = keyData.m_MemoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteBooleanValueAndGetChange(owningTemplate.templatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.templateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate.m_Listeners)
                listener.UpdateValue(in keyData, valuePtr, sizeof(byte));
        }

        internal void SetInstanceSyncedEnumValueWithoutValidation<T>(in BlackboardKeyCompiledData keyData, T value) where T : unmanaged, System.Enum
        {
            var memoryOffset = keyData.m_MemoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteEnumValueAndGetChange<T>(owningTemplate.templatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.templateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate.m_Listeners)
                listener.UpdateValue(in keyData, valuePtr, (ushort) sizeof(T));
        }

        internal void SetInstanceSyncedFloatValueWithoutValidation(in BlackboardKeyCompiledData keyData, float value)
        {
            var memoryOffset = keyData.m_MemoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteFloatValueAndGetChange(owningTemplate.templatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.templateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate.m_Listeners)
                listener.UpdateValue(in keyData, valuePtr, sizeof(float));
        }

        internal void SetInstanceSyncedIntegerValueWithoutValidation(in BlackboardKeyCompiledData keyData, int value)
        {
            var memoryOffset = keyData.m_MemoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteIntegerValueAndGetChange(owningTemplate.templatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.templateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate.m_Listeners)
                listener.UpdateValue(in keyData, valuePtr, sizeof(int));
        }

        internal void SetInstanceSyncedObjectValueWithoutValidation(in BlackboardKeyCompiledData keyData, Object value)
        {
            var memoryOffset = keyData.m_MemoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteObjectValueAndGetChange(owningTemplate.templatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.templateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate.m_Listeners)
                listener.UpdateValue(in keyData, valuePtr, sizeof(int));
        }

        internal void SetInstanceSyncedVectorValueWithoutValidation(in BlackboardKeyCompiledData keyData, Vector3 value)
        {
            var memoryOffset = keyData.m_MemoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteVectorValueAndGetChange(owningTemplate.templatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.templateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate.m_Listeners)
                listener.UpdateValue(in keyData, valuePtr, sizeof(float) * 3);
        }

        internal void SetInstanceSyncedQuaternionValueWithoutValidation(in BlackboardKeyCompiledData keyData, Quaternion value)
        {
            var memoryOffset = keyData.m_MemoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteQuaternionValueAndGetChange(owningTemplate.templatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.templateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate.m_Listeners)
                listener.UpdateValue(in keyData, valuePtr, sizeof(float) * 4);
        }
    }
}