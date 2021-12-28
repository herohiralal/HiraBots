using Unity.Mathematics;
using UnityEngine;

namespace HiraBots
{
    internal unsafe partial class BlackboardTemplateCompiledData
    {
        /// <summary>
        /// Set instance-synced Boolean value on blackboard template using BlackboardKeyCompiledData without validating any input.
        /// </summary>
        internal void SetInstanceSyncedBooleanValueWithoutValidation(in BlackboardKeyCompiledData keyData, bool value)
        {
            var memoryOffset = keyData.memoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteBooleanValueAndGetChange(owningTemplate.templatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.templateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate.m_Listeners)
                listener.UpdateValue(in keyData, valuePtr, sizeof(byte));
        }

        /// <summary>
        /// Set instance-synced Enum value index on blackboard template using BlackboardKeyCompiledData without validating any input.
        /// </summary>
        internal void SetInstanceSyncedEnumValueWithoutValidation(in BlackboardKeyCompiledData keyData, byte value)
        {
            var memoryOffset = keyData.memoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteEnumValueAndGetChange(owningTemplate.templatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.templateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate.m_Listeners)
                listener.UpdateValue(in keyData, valuePtr, sizeof(byte));
        }

        /// <summary>
        /// Set instance-synced Enum value on blackboard template using BlackboardKeyCompiledData without validating any input.
        /// </summary>
        internal void SetInstanceSyncedEnumValueWithoutValidation<T>(in BlackboardKeyCompiledData keyData, T value) where T : unmanaged, System.Enum
        {
            var memoryOffset = keyData.memoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteEnumValueAndGetChange<T>(owningTemplate.templatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.templateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate.m_Listeners)
                listener.UpdateValue(in keyData, valuePtr, (ushort) sizeof(T));
        }

        /// <summary>
        /// Set instance-synced Float value on blackboard template using BlackboardKeyCompiledData without validating any input.
        /// </summary>
        internal void SetInstanceSyncedFloatValueWithoutValidation(in BlackboardKeyCompiledData keyData, float value)
        {
            var memoryOffset = keyData.memoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteFloatValueAndGetChange(owningTemplate.templatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.templateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate.m_Listeners)
                listener.UpdateValue(in keyData, valuePtr, sizeof(float));
        }

        /// <summary>
        /// Set instance-synced Integer value on blackboard template using BlackboardKeyCompiledData without validating any input.
        /// </summary>
        internal void SetInstanceSyncedIntegerValueWithoutValidation(in BlackboardKeyCompiledData keyData, int value)
        {
            var memoryOffset = keyData.memoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteIntegerValueAndGetChange(owningTemplate.templatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.templateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate.m_Listeners)
                listener.UpdateValue(in keyData, valuePtr, sizeof(int));
        }

        /// <summary>
        /// Set instance-synced Object value on blackboard template using BlackboardKeyCompiledData without validating any input.
        /// </summary>
        internal void SetInstanceSyncedObjectValueWithoutValidation(in BlackboardKeyCompiledData keyData, Object value)
        {
            var memoryOffset = keyData.memoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteObjectValueAndGetChange(owningTemplate.templatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.templateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate.m_Listeners)
                listener.UpdateValue(in keyData, valuePtr, sizeof(int));
        }

        /// <summary>
        /// Set instance-synced Vector value on blackboard template using BlackboardKeyCompiledData without validating any input.
        /// </summary>
        internal void SetInstanceSyncedVectorValueWithoutValidation(in BlackboardKeyCompiledData keyData, float3 value)
        {
            var memoryOffset = keyData.memoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteVectorValueAndGetChange(owningTemplate.templatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.templateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate.m_Listeners)
                listener.UpdateValue(in keyData, valuePtr, sizeof(float) * 3);
        }

        /// <summary>
        /// Set instance-synced Quaternion value on blackboard template using BlackboardKeyCompiledData without validating any input.
        /// </summary>
        internal void SetInstanceSyncedQuaternionValueWithoutValidation(in BlackboardKeyCompiledData keyData, quaternion value)
        {
            var memoryOffset = keyData.memoryOffset;
            var owningTemplate = GetOwningTemplate(memoryOffset);
            if (!BlackboardUnsafeHelpers.WriteQuaternionValueAndGetChange(owningTemplate.templatePtr, memoryOffset, value)) return;

            var valuePtr = owningTemplate.templateReadOnlyPtr + memoryOffset;
            foreach (var listener in owningTemplate.m_Listeners)
                listener.UpdateValue(in keyData, valuePtr, sizeof(float) * 4);
        }
    }
}