using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HiraBots
{
    internal unsafe partial class BlackboardComponent : IInstanceSynchronizerListener
    {
        private readonly System.Collections.Generic.List<ushort> m_UnexpectedChanges;
        internal ReadOnlyListAccessor<ushort> unexpectedChanges => m_UnexpectedChanges;
        internal bool hasUnexpectedChanges => m_UnexpectedChanges.Count > 0;
        internal void ClearUnexpectedChanges() => m_UnexpectedChanges.Clear();

        void IInstanceSynchronizerListener.UpdateValue(in BlackboardKeyCompiledData keyData, byte* value, ushort size)
        {
            var memoryOffset = keyData.m_MemoryOffset;
            var ptr = dataPtr + memoryOffset;
            for (var i = 0; i < size; i++) ptr[i] = value[i];

            if (keyData.broadcastEventOnUnexpectedChange) m_UnexpectedChanges.Add(memoryOffset);
        }

        internal void SetBooleanValueWithoutValidation(in BlackboardKeyCompiledData keyData, bool value, bool expected = false)
        {
            var memoryOffset = keyData.m_MemoryOffset;

            if (expected)
            {
                if (keyData.instanceSynced)
                {
                    // instance synced
                    m_Template.RemoveInstanceSyncListener(this);
                    m_Template.SetInstanceSyncedBooleanValueWithoutValidation(
                        in keyData, value);
                    m_Template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteBooleanValue(dataPtr, memoryOffset, value);
            }
            else
            {
                if (keyData.instanceSynced)
                {
                    // unexpected & instance synced
                    m_Template.SetInstanceSyncedBooleanValueWithoutValidation(
                        in keyData, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteBooleanValueAndGetChange(dataPtr, memoryOffset, value))
                        if (keyData.broadcastEventOnUnexpectedChange)
                            m_UnexpectedChanges.Add(memoryOffset);
                }
            }
        }

        internal void SetEnumValueWithoutValidation<T>(in BlackboardKeyCompiledData keyData, T value, bool expected = false) where T : unmanaged, Enum
        {
            var memoryOffset = keyData.m_MemoryOffset;

            if (expected)
            {
                if (keyData.instanceSynced)
                {
                    // instance synced
                    m_Template.RemoveInstanceSyncListener(this);
                    m_Template.SetInstanceSyncedEnumValueWithoutValidation<T>(
                        in keyData, value);
                    m_Template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteEnumValue<T>(dataPtr, memoryOffset, value);
            }
            else
            {
                if (keyData.instanceSynced)
                {
                    // unexpected & instance synced
                    m_Template.SetInstanceSyncedEnumValueWithoutValidation<T>(
                        in keyData, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteEnumValueAndGetChange<T>(dataPtr, memoryOffset, value))
                        if (keyData.broadcastEventOnUnexpectedChange)
                            m_UnexpectedChanges.Add(memoryOffset);
                }
            }
        }

        internal void SetFloatValueWithoutValidation(in BlackboardKeyCompiledData keyData, float value, bool expected = false)
        {
            var memoryOffset = keyData.m_MemoryOffset;

            if (expected)
            {
                if (keyData.instanceSynced)
                {
                    // instance synced
                    m_Template.RemoveInstanceSyncListener(this);
                    m_Template.SetInstanceSyncedFloatValueWithoutValidation(
                        in keyData, value);
                    m_Template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteFloatValue(dataPtr, memoryOffset, value);
            }
            else
            {
                if (keyData.instanceSynced)
                {
                    // unexpected & instance synced
                    m_Template.SetInstanceSyncedFloatValueWithoutValidation(
                        in keyData, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteFloatValueAndGetChange(dataPtr, memoryOffset, value))
                        if (keyData.broadcastEventOnUnexpectedChange)
                            m_UnexpectedChanges.Add(memoryOffset);
                }
            }
        }

        internal void SetIntegerValueWithoutValidation(in BlackboardKeyCompiledData keyData, int value, bool expected = false)
        {
            var memoryOffset = keyData.m_MemoryOffset;

            if (expected)
            {
                if (keyData.instanceSynced)
                {
                    // instance synced
                    m_Template.RemoveInstanceSyncListener(this);
                    m_Template.SetInstanceSyncedIntegerValueWithoutValidation(
                        in keyData, value);
                    m_Template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteIntegerValue(dataPtr, memoryOffset, value);
            }
            else
            {
                if (keyData.instanceSynced)
                {
                    // unexpected & instance synced
                    m_Template.SetInstanceSyncedIntegerValueWithoutValidation(
                        in keyData, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteIntegerValueAndGetChange(dataPtr, memoryOffset, value))
                        if (keyData.broadcastEventOnUnexpectedChange)
                            m_UnexpectedChanges.Add(memoryOffset);
                }
            }
        }

        internal void SetObjectValueWithoutValidation(in BlackboardKeyCompiledData keyData, Object value, bool expected = false)
        {
            var memoryOffset = keyData.m_MemoryOffset;

            if (expected)
            {
                if (keyData.instanceSynced)
                {
                    // instance synced
                    m_Template.RemoveInstanceSyncListener(this);
                    m_Template.SetInstanceSyncedObjectValueWithoutValidation(
                        in keyData, value);
                    m_Template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteObjectValueNoProcess(dataPtr, memoryOffset, value);
            }
            else
            {
                if (keyData.instanceSynced)
                {
                    // unexpected & instance synced
                    m_Template.SetInstanceSyncedObjectValueWithoutValidation(
                        in keyData, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteObjectValueAndGetChange(dataPtr, memoryOffset, value))
                        if (keyData.broadcastEventOnUnexpectedChange)
                            m_UnexpectedChanges.Add(memoryOffset);
                }
            }
        }

        internal void SetVectorValueWithoutValidation(in BlackboardKeyCompiledData keyData, Vector3 value, bool expected = false)
        {
            var memoryOffset = keyData.m_MemoryOffset;

            if (expected)
            {
                if (keyData.instanceSynced)
                {
                    // instance synced
                    m_Template.RemoveInstanceSyncListener(this);
                    m_Template.SetInstanceSyncedVectorValueWithoutValidation(
                        in keyData, value);
                    m_Template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteVectorValue(dataPtr, memoryOffset, value);
            }
            else
            {
                if (keyData.instanceSynced)
                {
                    // unexpected & instance synced
                    m_Template.SetInstanceSyncedVectorValueWithoutValidation(
                        in keyData, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteVectorValueAndGetChange(dataPtr, memoryOffset, value))
                        if (keyData.broadcastEventOnUnexpectedChange)
                            m_UnexpectedChanges.Add(memoryOffset);
                }
            }
        }

        internal void SetQuaternionValueWithoutValidation(in BlackboardKeyCompiledData keyData, Quaternion value, bool expected = false)
        {
            var memoryOffset = keyData.m_MemoryOffset;

            if (expected)
            {
                if (keyData.instanceSynced)
                {
                    // instance synced
                    m_Template.RemoveInstanceSyncListener(this);
                    m_Template.SetInstanceSyncedQuaternionValueWithoutValidation(
                        in keyData, value);
                    m_Template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteQuaternionValue(dataPtr, memoryOffset, value);
            }
            else
            {
                if (keyData.instanceSynced)
                {
                    // unexpected & instance synced
                    m_Template.SetInstanceSyncedQuaternionValueWithoutValidation(
                        in keyData, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteQuaternionValueAndGetChange(dataPtr, memoryOffset, value))
                        if (keyData.broadcastEventOnUnexpectedChange)
                            m_UnexpectedChanges.Add(memoryOffset);
                }
            }
        }
    }
}