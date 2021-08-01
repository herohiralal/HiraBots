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

        void IInstanceSynchronizerListener.UpdateValue(ushort memoryOffset, byte* value, ushort size, bool broadcastEventOnUnexpectedChange)
        {
            var ptr = DataPtr + memoryOffset;
            for (var i = 0; i < size; i++) ptr[i] = value[i];

            if (broadcastEventOnUnexpectedChange) _unexpectedChanges.Add(memoryOffset);
        }

        internal void SetBooleanValueWithoutValidation(ushort memoryOffset, bool value, bool expected = false)
        {
            var keyData = _template.MemoryOffsetToKeyData[memoryOffset];

            if (expected)
            {
                if (keyData.InstanceSynced)
                {
                    // instance synced
                    _template.RemoveInstanceSyncListener(this);
                    _template.UpdateInstanceSyncedBooleanKeyWithoutValidation(
                        keyData.BroadcastEventOnUnexpectedChange, memoryOffset, value);
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
                        keyData.BroadcastEventOnUnexpectedChange, memoryOffset, value);
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

        internal void SetEnumValueWithoutValidation<T>(ushort memoryOffset, T value, bool expected = false) where T : unmanaged, Enum
        {
            var keyData = _template.MemoryOffsetToKeyData[memoryOffset];

            if (expected)
            {
                if (keyData.InstanceSynced)
                {
                    // instance synced
                    _template.RemoveInstanceSyncListener(this);
                    _template.UpdateInstanceSyncedEnumKeyWithoutValidation<T>(
                        keyData.BroadcastEventOnUnexpectedChange, memoryOffset, value);
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
                        keyData.BroadcastEventOnUnexpectedChange, memoryOffset, value);
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

        internal void SetFloatValueWithoutValidation(ushort memoryOffset, float value, bool expected = false)
        {
            var keyData = _template.MemoryOffsetToKeyData[memoryOffset];

            if (expected)
            {
                if (keyData.InstanceSynced)
                {
                    // instance synced
                    _template.RemoveInstanceSyncListener(this);
                    _template.UpdateInstanceSyncedFloatKeyWithoutValidation(
                        keyData.BroadcastEventOnUnexpectedChange, memoryOffset, value);
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
                        keyData.BroadcastEventOnUnexpectedChange, memoryOffset, value);
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

        internal void SetIntegerValueWithoutValidation(ushort memoryOffset, int value, bool expected = false)
        {
            var keyData = _template.MemoryOffsetToKeyData[memoryOffset];

            if (expected)
            {
                if (keyData.InstanceSynced)
                {
                    // instance synced
                    _template.RemoveInstanceSyncListener(this);
                    _template.UpdateInstanceSyncedIntegerKeyWithoutValidation(
                        keyData.BroadcastEventOnUnexpectedChange, memoryOffset, value);
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
                        keyData.BroadcastEventOnUnexpectedChange, memoryOffset, value);
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

        internal void SetObjectValueWithoutValidation(ushort memoryOffset, Object value, bool expected = false)
        {
            var keyData = _template.MemoryOffsetToKeyData[memoryOffset];

            if (expected)
            {
                if (keyData.InstanceSynced)
                {
                    // instance synced
                    _template.RemoveInstanceSyncListener(this);
                    _template.UpdateInstanceSyncedObjectKeyWithoutValidation(
                        keyData.BroadcastEventOnUnexpectedChange, memoryOffset, value);
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
                        keyData.BroadcastEventOnUnexpectedChange, memoryOffset, value);
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

        internal void SetVectorValueWithoutValidation(ushort memoryOffset, Vector3 value, bool expected = false)
        {
            var keyData = _template.MemoryOffsetToKeyData[memoryOffset];

            if (expected)
            {
                if (keyData.InstanceSynced)
                {
                    // instance synced
                    _template.RemoveInstanceSyncListener(this);
                    _template.UpdateInstanceSyncedVectorKeyWithoutValidation(
                        keyData.BroadcastEventOnUnexpectedChange, memoryOffset, value);
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
                        keyData.BroadcastEventOnUnexpectedChange, memoryOffset, value);
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

        internal void SetQuaternionValueWithoutValidation(ushort memoryOffset, Quaternion value, bool expected = false)
        {
            var keyData = _template.MemoryOffsetToKeyData[memoryOffset];

            if (expected)
            {
                if (keyData.InstanceSynced)
                {
                    // instance synced
                    _template.RemoveInstanceSyncListener(this);
                    _template.UpdateInstanceSyncedQuaternionKeyWithoutValidation(
                        keyData.BroadcastEventOnUnexpectedChange, memoryOffset, value);
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
                        keyData.BroadcastEventOnUnexpectedChange, memoryOffset, value);
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