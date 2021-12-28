using System;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HiraBots
{
    internal unsafe partial class BlackboardComponent : IInstanceSynchronizerListener
    {
        // list of memory offsets that have seen an unexpected change
        private readonly System.Collections.Generic.List<string> m_UnexpectedChanges;

        /// <summary>
        /// The unexpected changes present in the blackboard.
        /// </summary>
        internal ReadOnlyListAccessor<string> unexpectedChanges => m_UnexpectedChanges;

        /// <summary>
        /// Whether the blackboard component has unexpected changes.
        /// </summary>
        internal bool hasUnexpectedChanges => m_UnexpectedChanges.Count > 0;

        /// <summary>
        /// Clear up the unexpected changes.
        /// </summary>
        internal void ClearUnexpectedChanges()
        {
            m_UnexpectedChanges.Clear();
        }

        // implementing IInstanceSynchronizerListener to listen to the changes in the parent's instance synced values
        void IInstanceSynchronizerListener.UpdateValue(in BlackboardKeyCompiledData keyData, byte* value, ushort size)
        {
            var memoryOffset = keyData.memoryOffset;
            var ptr = dataPtr + memoryOffset;
            for (var i = 0; i < size; i++)
            {
                ptr[i] = value[i];
            }

            if (keyData.broadcastEventOnUnexpectedChange)
            {
                m_UnexpectedChanges.Add(keyData.keyName);
            }
        }

        /// <summary>
        /// Set Boolean value on blackboard using BlackboardKeyCompiledData without validating any input.
        /// </summary>
        internal void SetBooleanValueWithoutValidation(in BlackboardKeyCompiledData keyData, bool value, bool expected = false)
        {
            var memoryOffset = keyData.memoryOffset;

            if (expected)
            {
                if (keyData.instanceSynced)
                {
                    // instance synced
                    m_Template.RemoveInstanceSyncListener(this);
                    m_Template.SetInstanceSyncedBooleanValueWithoutValidation(in keyData, value);
                    m_Template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteBooleanValue(dataPtr, memoryOffset, value);
            }
            else
            {
                if (keyData.instanceSynced)
                {
                    // unexpected & instance synced
                    m_Template.SetInstanceSyncedBooleanValueWithoutValidation(in keyData, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteBooleanValueAndGetChange(dataPtr, memoryOffset, value))
                    {
                        if (keyData.broadcastEventOnUnexpectedChange)
                        {
                            m_UnexpectedChanges.Add(keyData.keyName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set Enum value index on blackboard using BlackboardKeyCompiledData without validating any input.
        /// </summary>
        internal void SetEnumValueWithoutValidation(in BlackboardKeyCompiledData keyData, byte value, bool expected = false)
        {
            var memoryOffset = keyData.memoryOffset;

            if (expected)
            {
                if (keyData.instanceSynced)
                {
                    // instance synced
                    m_Template.RemoveInstanceSyncListener(this);
                    m_Template.SetInstanceSyncedEnumValueWithoutValidation(in keyData, value);
                    m_Template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteEnumValue(dataPtr, memoryOffset, value);
            }
            else
            {
                if (keyData.instanceSynced)
                {
                    // unexpected & instance synced
                    m_Template.SetInstanceSyncedEnumValueWithoutValidation(in keyData, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteEnumValueAndGetChange(dataPtr, memoryOffset, value))
                    {
                        if (keyData.broadcastEventOnUnexpectedChange)
                        {
                            m_UnexpectedChanges.Add(keyData.keyName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set Enum value on blackboard using BlackboardKeyCompiledData without validating any input.
        /// </summary>
        internal void SetEnumValueWithoutValidation<T>(in BlackboardKeyCompiledData keyData, T value, bool expected = false) where T : unmanaged, Enum
        {
            var memoryOffset = keyData.memoryOffset;

            if (expected)
            {
                if (keyData.instanceSynced)
                {
                    // instance synced
                    m_Template.RemoveInstanceSyncListener(this);
                    m_Template.SetInstanceSyncedEnumValueWithoutValidation<T>(in keyData, value);
                    m_Template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteEnumValue<T>(dataPtr, memoryOffset, value);
            }
            else
            {
                if (keyData.instanceSynced)
                {
                    // unexpected & instance synced
                    m_Template.SetInstanceSyncedEnumValueWithoutValidation<T>(in keyData, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteEnumValueAndGetChange<T>(dataPtr, memoryOffset, value))
                    {
                        if (keyData.broadcastEventOnUnexpectedChange)
                        {
                            m_UnexpectedChanges.Add(keyData.keyName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set Float value on blackboard using BlackboardKeyCompiledData without validating any input.
        /// </summary>
        internal void SetFloatValueWithoutValidation(in BlackboardKeyCompiledData keyData, float value, bool expected = false)
        {
            var memoryOffset = keyData.memoryOffset;

            if (expected)
            {
                if (keyData.instanceSynced)
                {
                    // instance synced
                    m_Template.RemoveInstanceSyncListener(this);
                    m_Template.SetInstanceSyncedFloatValueWithoutValidation(in keyData, value);
                    m_Template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteFloatValue(dataPtr, memoryOffset, value);
            }
            else
            {
                if (keyData.instanceSynced)
                {
                    // unexpected & instance synced
                    m_Template.SetInstanceSyncedFloatValueWithoutValidation(in keyData, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteFloatValueAndGetChange(dataPtr, memoryOffset, value))
                    {
                        if (keyData.broadcastEventOnUnexpectedChange)
                        {
                            m_UnexpectedChanges.Add(keyData.keyName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set Integer value on blackboard using BlackboardKeyCompiledData without validating any input.
        /// </summary>
        internal void SetIntegerValueWithoutValidation(in BlackboardKeyCompiledData keyData, int value, bool expected = false)
        {
            var memoryOffset = keyData.memoryOffset;

            if (expected)
            {
                if (keyData.instanceSynced)
                {
                    // instance synced
                    m_Template.RemoveInstanceSyncListener(this);
                    m_Template.SetInstanceSyncedIntegerValueWithoutValidation(in keyData, value);
                    m_Template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteIntegerValue(dataPtr, memoryOffset, value);
            }
            else
            {
                if (keyData.instanceSynced)
                {
                    // unexpected & instance synced
                    m_Template.SetInstanceSyncedIntegerValueWithoutValidation(in keyData, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteIntegerValueAndGetChange(dataPtr, memoryOffset, value))
                    {
                        if (keyData.broadcastEventOnUnexpectedChange)
                        {
                            m_UnexpectedChanges.Add(keyData.keyName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set Object value on blackboard using BlackboardKeyCompiledData without validating any input.
        /// </summary>
        internal void SetObjectValueWithoutValidation(in BlackboardKeyCompiledData keyData, Object value, bool expected = false)
        {
            var memoryOffset = keyData.memoryOffset;

            if (expected)
            {
                if (keyData.instanceSynced)
                {
                    // instance synced
                    m_Template.RemoveInstanceSyncListener(this);
                    m_Template.SetInstanceSyncedObjectValueWithoutValidation(in keyData, value);
                    m_Template.AddInstanceSyncListener(this);
                }

                // since this is an instance-synced value, it is undesirable to add to the reference counter
                // so a NoProcess variant is used, which skips over the caching
                BlackboardUnsafeHelpers.WriteObjectValueNoProcess(dataPtr, memoryOffset, value);
            }
            else
            {
                if (keyData.instanceSynced)
                {
                    // unexpected & instance synced
                    m_Template.SetInstanceSyncedObjectValueWithoutValidation(in keyData, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteObjectValueAndGetChange(dataPtr, memoryOffset, value))
                    {
                        if (keyData.broadcastEventOnUnexpectedChange)
                        {
                            m_UnexpectedChanges.Add(keyData.keyName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set Vector value on blackboard using BlackboardKeyCompiledData without validating any input.
        /// </summary>
        internal void SetVectorValueWithoutValidation(in BlackboardKeyCompiledData keyData, float3 value, bool expected = false)
        {
            var memoryOffset = keyData.memoryOffset;

            if (expected)
            {
                if (keyData.instanceSynced)
                {
                    // instance synced
                    m_Template.RemoveInstanceSyncListener(this);
                    m_Template.SetInstanceSyncedVectorValueWithoutValidation(in keyData, value);
                    m_Template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteVectorValue(dataPtr, memoryOffset, value);
            }
            else
            {
                if (keyData.instanceSynced)
                {
                    // unexpected & instance synced
                    m_Template.SetInstanceSyncedVectorValueWithoutValidation(in keyData, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteVectorValueAndGetChange(dataPtr, memoryOffset, value))
                    {
                        if (keyData.broadcastEventOnUnexpectedChange)
                        {
                            m_UnexpectedChanges.Add(keyData.keyName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set Quaternion value on blackboard using BlackboardKeyCompiledData without validating any input.
        /// </summary>
        internal void SetQuaternionValueWithoutValidation(in BlackboardKeyCompiledData keyData, quaternion value, bool expected = false)
        {
            var memoryOffset = keyData.memoryOffset;

            if (expected)
            {
                if (keyData.instanceSynced)
                {
                    // instance synced
                    m_Template.RemoveInstanceSyncListener(this);
                    m_Template.SetInstanceSyncedQuaternionValueWithoutValidation(in keyData, value);
                    m_Template.AddInstanceSyncListener(this);
                }

                BlackboardUnsafeHelpers.WriteQuaternionValue(dataPtr, memoryOffset, value);
            }
            else
            {
                if (keyData.instanceSynced)
                {
                    // unexpected & instance synced
                    m_Template.SetInstanceSyncedQuaternionValueWithoutValidation(in keyData, value);
                }
                else
                {
                    // unexpected & not instance synced
                    if (BlackboardUnsafeHelpers.WriteQuaternionValueAndGetChange(dataPtr, memoryOffset, value))
                    {
                        if (keyData.broadcastEventOnUnexpectedChange)
                        {
                            m_UnexpectedChanges.Add(keyData.keyName);
                        }
                    }
                }
            }
        }
    }
}