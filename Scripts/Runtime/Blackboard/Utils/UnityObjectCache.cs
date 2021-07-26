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
                Reference = reference;
                Frozen = false;
                Count = 1;
            }

            internal readonly Object Reference;
            internal bool Frozen;
            internal uint Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal UnityObjectCache(int capacity) => _objectCache = new Dictionary<int, ObjectCacheData>(capacity);

        private readonly Dictionary<int, ObjectCacheData> _objectCache;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Clear() => _objectCache.Clear();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Object Read(int instanceID) =>
            instanceID != 0 && _objectCache.TryGetValue(instanceID, out var data) ? data.Reference : null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetFreeze(int instanceID, bool value)
        {
            if (instanceID == 0 || !_objectCache.TryGetValue(instanceID, out var dataToMakePersistent))
                return;

            dataToMakePersistent.Frozen = value;
            _objectCache[instanceID] = dataToMakePersistent;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Remove(int instanceID)
        {
            if (instanceID == 0 || !_objectCache.TryGetValue(instanceID, out var dataToRemove))
                return;

            dataToRemove.Count -= dataToRemove.Frozen ? 0u : 1u;

            if (dataToRemove.Count == 0)
            {
                _objectCache.Remove(instanceID);
                LogCacheUpdate(dataToRemove.Reference, false);
            }
            else _objectCache[instanceID] = dataToRemove;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int Process(Object value)
        {
            if (value == null)
                return 0;

            var instanceID = value.GetInstanceID();
            
            if (_objectCache.TryGetValue(instanceID, out var currentData))
            {
                currentData.Count += currentData.Frozen ? 0u : 1u;
                _objectCache[instanceID] = currentData;
            }
            else
            {
                _objectCache.Add(instanceID, new ObjectCacheData(value));
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