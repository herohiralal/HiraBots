using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HiraBots
{
    internal unsafe partial class BlackboardComponent : IInstanceSynchronizerListener
    {
        internal bool HasUnexpectedChanges { get; private set; }
        internal void ClearUnexpectedChanges() => HasUnexpectedChanges = false;

        void IInstanceSynchronizerListener.UpdateValue(ushort memoryOffset, byte* value, ushort size, bool broadcastEventOnUnexpectedChange)
        {
            var ptr = DataPtr + memoryOffset;
            for (var i = 0; i < size; i++) ptr[i] = value[i];

            HasUnexpectedChanges |= broadcastEventOnUnexpectedChange;
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
                        HasUnexpectedChanges |= keyData.BroadcastEventOnUnexpectedChange;
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
                        HasUnexpectedChanges |= keyData.BroadcastEventOnUnexpectedChange;
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
                        HasUnexpectedChanges |= keyData.BroadcastEventOnUnexpectedChange;
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
                        HasUnexpectedChanges |= keyData.BroadcastEventOnUnexpectedChange;
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
                        HasUnexpectedChanges |= keyData.BroadcastEventOnUnexpectedChange;
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
                        HasUnexpectedChanges |= keyData.BroadcastEventOnUnexpectedChange;
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
                        HasUnexpectedChanges |= keyData.BroadcastEventOnUnexpectedChange;
                }
            }
        }
    }
}