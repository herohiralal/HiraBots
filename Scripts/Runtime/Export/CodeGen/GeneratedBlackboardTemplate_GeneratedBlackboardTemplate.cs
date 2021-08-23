using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace UnityEngine
{
    // ReSharper disable once InconsistentNaming
    public abstract class GeneratedBlackboardTemplate_GeneratedBlackboardTemplate : IGeneratedBlackboardInstanceSyncListener, IDisposable
    {
        protected readonly HashSet<IGeneratedBlackboardInstanceSyncListener> m_InstanceSyncListeners =
            new HashSet<IGeneratedBlackboardInstanceSyncListener>();

        protected void AddInstanceSyncListenerActual(IGeneratedBlackboardInstanceSyncListener listener)
        {
            m_InstanceSyncListeners.Add(listener);
        }

        protected void RemoveInstanceSyncListenerActual(IGeneratedBlackboardInstanceSyncListener listener)
        {
            m_InstanceSyncListeners.Remove(listener);
        }

        protected static void AddInstanceSyncListener(IGeneratedBlackboardInstanceSyncListener listener)
        {
        }

        protected static void RemoveInstanceSyncListener(IGeneratedBlackboardInstanceSyncListener listener)
        {
        }

        public abstract void Dispose();

        public abstract bool GetInstanceSyncedBooleanValue(string key);
        public abstract T GetInstanceSyncedEnumValue<T>(string key) where T : unmanaged, Enum;
        public abstract float GetInstanceSyncedFloatValue(string key);
        public abstract int GetInstanceSyncedIntegerValue(string key);
        public abstract Object GetInstanceSyncedObjectValue(string key);
        public abstract float3 GetInstanceSyncedVectorValue(string key);
        public abstract quaternion GetInstanceSyncedQuaternionValue(string key);
        public abstract void SetInstanceSyncedBooleanValue(string key, bool value, bool expected = false);
        public abstract void SetInstanceSyncedEnumValue<T>(string key, T value, bool expected = false);
        public abstract void SetInstanceSyncedFloatValue(string key, float value, bool expected = false);
        public abstract void SetInstanceSyncedIntegerValue(string key, int value, bool expected = false);
        public abstract void SetInstanceSyncedObjectValue(string key, Object value, bool expected = false);
        public abstract void SetInstanceSyncedVectorValue(string key, float3 value, bool expected = false);
        public abstract void SetInstanceSyncedQuaternionValue(string key, quaternion value, bool expected = false);
    }
}