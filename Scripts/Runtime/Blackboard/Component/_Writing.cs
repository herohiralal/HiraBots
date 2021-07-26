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

        internal void SetBooleanValueWithoutValidation(ushort keyIndex, bool value, bool expected = false)
        {
            var keyData = _template.KeyData[keyIndex];

            if (expected)
            {
                if (keyData.InstanceSynced)
                {
                    // instance synced
                    _template.RemoveInstanceSyncListener(this);
                    _template.UpdateInstanceSyncedBooleanKeyWithoutValidation(
                        keyData.BroadcastEventOnUnexpectedChange, keyData.MemoryOffset, value);
                    _template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteBooleanValue(DataPtr, keyData.MemoryOffset, value);
            }
            else
            {
                if (keyData.InstanceSynced)
                {
                    // unexpected & instance synced
                    _template.UpdateInstanceSyncedBooleanKeyWithoutValidation(
                        keyData.BroadcastEventOnUnexpectedChange, keyData.MemoryOffset, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteBooleanValueAndGetChange(DataPtr, keyData.MemoryOffset, value))
                        HasUnexpectedChanges |= keyData.BroadcastEventOnUnexpectedChange;
                }
            }
        }

        internal void SetEnumValueWithoutValidation<T>(ushort keyIndex, T value, bool expected = false) where T : unmanaged, Enum
        {
            var keyData = _template.KeyData[keyIndex];

            if (expected)
            {
                if (keyData.InstanceSynced)
                {
                    // instance synced
                    _template.RemoveInstanceSyncListener(this);
                    _template.UpdateInstanceSyncedEnumKeyWithoutValidation<T>(
                        keyData.BroadcastEventOnUnexpectedChange, keyData.MemoryOffset, value);
                    _template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteEnumValue<T>(DataPtr, keyData.MemoryOffset, value);
            }
            else
            {
                if (keyData.InstanceSynced)
                {
                    // unexpected & instance synced
                    _template.UpdateInstanceSyncedEnumKeyWithoutValidation<T>(
                        keyData.BroadcastEventOnUnexpectedChange, keyData.MemoryOffset, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteEnumValueAndGetChange<T>(DataPtr, keyData.MemoryOffset, value))
                        HasUnexpectedChanges |= keyData.BroadcastEventOnUnexpectedChange;
                }
            }
        }

        internal void SetFloatValueWithoutValidation(ushort keyIndex, float value, bool expected = false)
        {
            var keyData = _template.KeyData[keyIndex];

            if (expected)
            {
                if (keyData.InstanceSynced)
                {
                    // instance synced
                    _template.RemoveInstanceSyncListener(this);
                    _template.UpdateInstanceSyncedFloatKeyWithoutValidation(
                        keyData.BroadcastEventOnUnexpectedChange, keyData.MemoryOffset, value);
                    _template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteFloatValue(DataPtr, keyData.MemoryOffset, value);
            }
            else
            {
                if (keyData.InstanceSynced)
                {
                    // unexpected & instance synced
                    _template.UpdateInstanceSyncedFloatKeyWithoutValidation(
                        keyData.BroadcastEventOnUnexpectedChange, keyData.MemoryOffset, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteFloatValueAndGetChange(DataPtr, keyData.MemoryOffset, value))
                        HasUnexpectedChanges |= keyData.BroadcastEventOnUnexpectedChange;
                }
            }
        }

        internal void SetIntegerValueWithoutValidation(ushort keyIndex, int value, bool expected = false)
        {
            var keyData = _template.KeyData[keyIndex];

            if (expected)
            {
                if (keyData.InstanceSynced)
                {
                    // instance synced
                    _template.RemoveInstanceSyncListener(this);
                    _template.UpdateInstanceSyncedIntegerKeyWithoutValidation(
                        keyData.BroadcastEventOnUnexpectedChange, keyData.MemoryOffset, value);
                    _template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteIntegerValue(DataPtr, keyData.MemoryOffset, value);
            }
            else
            {
                if (keyData.InstanceSynced)
                {
                    // unexpected & instance synced
                    _template.UpdateInstanceSyncedIntegerKeyWithoutValidation(
                        keyData.BroadcastEventOnUnexpectedChange, keyData.MemoryOffset, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteIntegerValueAndGetChange(DataPtr, keyData.MemoryOffset, value))
                        HasUnexpectedChanges |= keyData.BroadcastEventOnUnexpectedChange;
                }
            }
        }

        internal void SetObjectValueWithoutValidation(ushort keyIndex, Object value, bool expected = false)
        {
            var keyData = _template.KeyData[keyIndex];

            if (expected)
            {
                if (keyData.InstanceSynced)
                {
                    // instance synced
                    _template.RemoveInstanceSyncListener(this);
                    _template.UpdateInstanceSyncedObjectKeyWithoutValidation(
                        keyData.BroadcastEventOnUnexpectedChange, keyData.MemoryOffset, value);
                    _template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteObjectValueNoProcess(DataPtr, keyData.MemoryOffset, value);
            }
            else
            {
                if (keyData.InstanceSynced)
                {
                    // unexpected & instance synced
                    _template.UpdateInstanceSyncedObjectKeyWithoutValidation(
                        keyData.BroadcastEventOnUnexpectedChange, keyData.MemoryOffset, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteObjectValueAndGetChange(DataPtr, keyData.MemoryOffset, value))
                        HasUnexpectedChanges |= keyData.BroadcastEventOnUnexpectedChange;
                }
            }
        }

        internal void SetVectorValueWithoutValidation(ushort keyIndex, Vector3 value, bool expected = false)
        {
            var keyData = _template.KeyData[keyIndex];

            if (expected)
            {
                if (keyData.InstanceSynced)
                {
                    // instance synced
                    _template.RemoveInstanceSyncListener(this);
                    _template.UpdateInstanceSyncedVectorKeyWithoutValidation(
                        keyData.BroadcastEventOnUnexpectedChange, keyData.MemoryOffset, value);
                    _template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteVectorValue(DataPtr, keyData.MemoryOffset, value);
            }
            else
            {
                if (keyData.InstanceSynced)
                {
                    // unexpected & instance synced
                    _template.UpdateInstanceSyncedVectorKeyWithoutValidation(
                        keyData.BroadcastEventOnUnexpectedChange, keyData.MemoryOffset, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteVectorValueAndGetChange(DataPtr, keyData.MemoryOffset, value))
                        HasUnexpectedChanges |= keyData.BroadcastEventOnUnexpectedChange;
                }
            }
        }

        internal void SetQuaternionValueWithoutValidation(ushort keyIndex, Quaternion value, bool expected = false)
        {
            var keyData = _template.KeyData[keyIndex];

            if (expected)
            {
                if (keyData.InstanceSynced)
                {
                    // instance synced
                    _template.RemoveInstanceSyncListener(this);
                    _template.UpdateInstanceSyncedQuaternionKeyWithoutValidation(
                        keyData.BroadcastEventOnUnexpectedChange, keyData.MemoryOffset, value);
                    _template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteQuaternionValue(DataPtr, keyData.MemoryOffset, value);
            }
            else
            {
                if (keyData.InstanceSynced)
                {
                    // unexpected & instance synced
                    _template.UpdateInstanceSyncedQuaternionKeyWithoutValidation(
                        keyData.BroadcastEventOnUnexpectedChange, keyData.MemoryOffset, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteQuaternionValueAndGetChange(DataPtr, keyData.MemoryOffset, value))
                        HasUnexpectedChanges |= keyData.BroadcastEventOnUnexpectedChange;
                }
            }
        }
    }
}