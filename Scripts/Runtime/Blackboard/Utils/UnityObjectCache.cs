#if !UNITY_2020_3_OR_NEWER // 2020.3 or newer can use Resources.InstanceIDToObject directly.
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace HiraBots
{
    /// <summary>
    /// This object is a reference-counted HashMap from instance ID to a Unity Object.
    /// The primary reason it is reference counted is so that it does not hold references
    /// to objects when it doesn't need to.
    /// </summary>
    internal readonly struct UnityObjectCache
    {
        // a structure used to pack in an entry within the object cache
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

        /// <summary>
        /// Create an object cache with a starting capacity.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal UnityObjectCache(int capacity)
        {
            m_ObjectCache = new Dictionary<int, ObjectCacheData>(capacity);
        }

        // the actual cache
        private readonly Dictionary<int, ObjectCacheData> m_ObjectCache;

        /// <summary>
        /// Clear the object cache.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Clear()
        {
            m_ObjectCache.Clear();
        }

        /// <summary>
        /// Get object from its instance ID.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Object Read(int instanceID)
        {
            return instanceID != 0 && m_ObjectCache.TryGetValue(instanceID, out var data) ? data.m_Reference : null;
        }

        /// <summary>
        /// Disable the reference counting updates for a specific object.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetFreeze(int instanceID, bool value)
        {
            if (instanceID == 0 || !m_ObjectCache.TryGetValue(instanceID, out var dataToMakePersistent))
            {
                return;
            }

            dataToMakePersistent.m_Frozen = value;
            m_ObjectCache[instanceID] = dataToMakePersistent;
        }

        /// <summary>
        /// Decrement a reference from the selected instance ID.
        /// </summary>
        /// <param name="instanceID"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Remove(int instanceID)
        {
            if (instanceID == 0 || !m_ObjectCache.TryGetValue(instanceID, out var dataToRemove))
            {
                return;
            }

            dataToRemove.m_Count -= dataToRemove.m_Frozen ? 0u : 1u;

            if (dataToRemove.m_Count == 0)
            {
                m_ObjectCache.Remove(instanceID);
                LogCacheUpdate(dataToRemove.m_Reference, false);
            }
            else
            {
                m_ObjectCache[instanceID] = dataToRemove;
            }
        }

        /// <summary>
        /// Increment a reference to the selected Object.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int Process(Object value)
        {
            if (value == null)
            {
                return 0;
            }

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

        /// <summary>
        /// Log all the major updates to the cache.
        /// </summary>
        [Conditional("LOG_HIRA_BOTS_UNITY_OBJECT_CACHE"), MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void LogCacheUpdate(Object value, bool saved)
        {
            Debug.Log($"UnityObjectCache Update: {(saved ? "Stored" : "Forgot")} {value.name}.");
        }
    }
}
#endif