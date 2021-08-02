#if !UNITY_2020_3_OR_NEWER // 2020.3 or newer can use Resources.InstanceIDToObject directly.
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace HiraBots
{
    internal readonly struct UnityObjectCache
    {
        private struct ObjectCacheData
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal ObjectCacheData(Object reference)
            {
                m_Reference = reference;
                m_Frozen = false;
                m_Count = 1;
            }

            internal readonly Object m_Reference;
            internal bool m_Frozen;
            internal uint m_Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal UnityObjectCache(int capacity) => m_ObjectCache = new Dictionary<int, ObjectCacheData>(capacity);

        private readonly Dictionary<int, ObjectCacheData> m_ObjectCache;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Clear() => m_ObjectCache.Clear();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Object Read(int instanceID) =>
            instanceID != 0 && m_ObjectCache.TryGetValue(instanceID, out var data) ? data.m_Reference : null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetFreeze(int instanceID, bool value)
        {
            if (instanceID == 0 || !m_ObjectCache.TryGetValue(instanceID, out var dataToMakePersistent))
                return;

            dataToMakePersistent.m_Frozen = value;
            m_ObjectCache[instanceID] = dataToMakePersistent;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Remove(int instanceID)
        {
            if (instanceID == 0 || !m_ObjectCache.TryGetValue(instanceID, out var dataToRemove))
                return;

            dataToRemove.m_Count -= dataToRemove.m_Frozen ? 0u : 1u;

            if (dataToRemove.m_Count == 0)
            {
                m_ObjectCache.Remove(instanceID);
                LogCacheUpdate(dataToRemove.m_Reference, false);
            }
            else m_ObjectCache[instanceID] = dataToRemove;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int Process(Object value)
        {
            if (value == null)
                return 0;

            var instanceID = value.GetInstanceID();
            
            if (m_ObjectCache.TryGetValue(instanceID, out var currentData))
            {
                currentData.m_Count += currentData.m_Frozen ? 0u : 1u;
                m_ObjectCache[instanceID] = currentData;
            }
            else
            {
                m_ObjectCache.Add(instanceID, new ObjectCacheData(value));
                LogCacheUpdate(value, true);
            }

            return instanceID;
        }

        [Conditional("LOG_HIRA_BOTS_UNITY_OBJECT_CACHE"), MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void LogCacheUpdate(Object value, bool saved) =>
            Debug.Log($"UnityObjectCache Update: {(saved ? "Stored" : "Forgot")} {value.name}.");
    }
}
#endif