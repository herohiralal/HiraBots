using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HiraBots
{
    internal unsafe partial class BlackboardComponent : IInstanceSynchronizerListener
    {
        private readonly System.Collections.Generic.List<ushort> _unexpectedChanges;
        internal ReadOnlyListAccessor<ushort> UnexpectedChanges => _unexpectedChanges;
        internal bool HasUnexpectedChanges => _unexpectedChanges.Count > 0;
        internal void ClearUnexpectedChanges() => _unexpectedChanges.Clear();

        void IInstanceSynchronizerListener.UpdateValue(in BlackboardKeyCompiledData keyData, byte* value, ushort size)
        {
            var memoryOffset = keyData.MemoryOffset;
            var ptr = DataPtr + memoryOffset;
            for (var i = 0; i < size; i++) ptr[i] = value[i];

            if (keyData.BroadcastEventOnUnexpectedChange) _unexpectedChanges.Add(memoryOffset);
        }

        internal void SetBooleanValueWithoutValidation(in BlackboardKeyCompiledData keyData, bool value, bool expected = false)
        {
            var memoryOffset = keyData.MemoryOffset;

            if (expected)
            {
                if (keyData.InstanceSynced)
                {
                    // instance synced
                    _template.RemoveInstanceSyncListener(this);
                    _template.UpdateInstanceSyncedBooleanKeyWithoutValidation(
                        in keyData, value);
                    _template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteBooleanValue(DataPtr, memoryOffset, value);
            }
            else
            {
                if (keyData.InstanceSynced)
                {
                    // unexpected & instance synced
                    _template.UpdateInstanceSyncedBooleanKeyWithoutValidation(
                        in keyData, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteBooleanValueAndGetChange(DataPtr, memoryOffset, value))
                        if (keyData.BroadcastEventOnUnexpectedChange)
                            _unexpectedChanges.Add(memoryOffset);
                }
            }
        }

        internal void SetEnumValueWithoutValidation<T>(in BlackboardKeyCompiledData keyData, T value, bool expected = false) where T : unmanaged, Enum
        {
            var memoryOffset = keyData.MemoryOffset;

            if (expected)
            {
                if (keyData.InstanceSynced)
                {
                    // instance synced
                    _template.RemoveInstanceSyncListener(this);
                    _template.UpdateInstanceSyncedEnumKeyWithoutValidation<T>(
                        in keyData, value);
                    _template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteEnumValue<T>(DataPtr, memoryOffset, value);
            }
            else
            {
                if (keyData.InstanceSynced)
                {
                    // unexpected & instance synced
                    _template.UpdateInstanceSyncedEnumKeyWithoutValidation<T>(
                        in keyData, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteEnumValueAndGetChange<T>(DataPtr, memoryOffset, value))
                        if (keyData.BroadcastEventOnUnexpectedChange)
                            _unexpectedChanges.Add(memoryOffset);
                }
            }
        }

        internal void SetFloatValueWithoutValidation(in BlackboardKeyCompiledData keyData, float value, bool expected = false)
        {
            var memoryOffset = keyData.MemoryOffset;

            if (expected)
            {
                if (keyData.InstanceSynced)
                {
                    // instance synced
                    _template.RemoveInstanceSyncListener(this);
                    _template.UpdateInstanceSyncedFloatKeyWithoutValidation(
                        in keyData, value);
                    _template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteFloatValue(DataPtr, memoryOffset, value);
            }
            else
            {
                if (keyData.InstanceSynced)
                {
                    // unexpected & instance synced
                    _template.UpdateInstanceSyncedFloatKeyWithoutValidation(
                        in keyData, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteFloatValueAndGetChange(DataPtr, memoryOffset, value))
                        if (keyData.BroadcastEventOnUnexpectedChange)
                            _unexpectedChanges.Add(memoryOffset);
                }
            }
        }

        internal void SetIntegerValueWithoutValidation(in BlackboardKeyCompiledData keyData, int value, bool expected = false)
        {
            var memoryOffset = keyData.MemoryOffset;

            if (expected)
            {
                if (keyData.InstanceSynced)
                {
                    // instance synced
                    _template.RemoveInstanceSyncListener(this);
                    _template.UpdateInstanceSyncedIntegerKeyWithoutValidation(
                        in keyData, value);
                    _template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteIntegerValue(DataPtr, memoryOffset, value);
            }
            else
            {
                if (keyData.InstanceSynced)
                {
                    // unexpected & instance synced
                    _template.UpdateInstanceSyncedIntegerKeyWithoutValidation(
                        in keyData, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteIntegerValueAndGetChange(DataPtr, memoryOffset, value))
                        if (keyData.BroadcastEventOnUnexpectedChange)
                            _unexpectedChanges.Add(memoryOffset);
                }
            }
        }

        internal void SetObjectValueWithoutValidation(in BlackboardKeyCompiledData keyData, Object value, bool expected = false)
        {
            var memoryOffset = keyData.MemoryOffset;

            if (expected)
            {
                if (keyData.InstanceSynced)
                {
                    // instance synced
                    _template.RemoveInstanceSyncListener(this);
                    _template.UpdateInstanceSyncedObjectKeyWithoutValidation(
                        in keyData, value);
                    _template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteObjectValueNoProcess(DataPtr, memoryOffset, value);
            }
            else
            {
                if (keyData.InstanceSynced)
                {
                    // unexpected & instance synced
                    _template.UpdateInstanceSyncedObjectKeyWithoutValidation(
                        in keyData, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteObjectValueAndGetChange(DataPtr, memoryOffset, value))
                        if (keyData.BroadcastEventOnUnexpectedChange)
                            _unexpectedChanges.Add(memoryOffset);
                }
            }
        }

        internal void SetVectorValueWithoutValidation(in BlackboardKeyCompiledData keyData, Vector3 value, bool expected = false)
        {
            var memoryOffset = keyData.MemoryOffset;

            if (expected)
            {
                if (keyData.InstanceSynced)
                {
                    // instance synced
                    _template.RemoveInstanceSyncListener(this);
                    _template.UpdateInstanceSyncedVectorKeyWithoutValidation(
                        in keyData, value);
                    _template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteVectorValue(DataPtr, memoryOffset, value);
            }
            else
            {
                if (keyData.InstanceSynced)
                {
                    // unexpected & instance synced
                    _template.UpdateInstanceSyncedVectorKeyWithoutValidation(
                        in keyData, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteVectorValueAndGetChange(DataPtr, memoryOffset, value))
                        if (keyData.BroadcastEventOnUnexpectedChange)
                            _unexpectedChanges.Add(memoryOffset);
                }
            }
        }

        internal void SetQuaternionValueWithoutValidation(in BlackboardKeyCompiledData keyData, Quaternion value, bool expected = false)
        {
            var memoryOffset = keyData.MemoryOffset;

            if (expected)
            {
                if (keyData.InstanceSynced)
                {
                    // instance synced
                    _template.RemoveInstanceSyncListener(this);
                    _template.UpdateInstanceSyncedQuaternionKeyWithoutValidation(
                        in keyData, value);
                    _template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteQuaternionValue(DataPtr, memoryOffset, value);
            }
            else
            {
                if (keyData.InstanceSynced)
                {
                    // unexpected & instance synced
                    _template.UpdateInstanceSyncedQuaternionKeyWithoutValidation(
                        in keyData, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteQuaternionValueAndGetChange(DataPtr, memoryOffset, value))
                        if (keyData.BroadcastEventOnUnexpectedChange)
                            _unexpectedChanges.Add(memoryOffset);
                }
            }
        }
    }
}