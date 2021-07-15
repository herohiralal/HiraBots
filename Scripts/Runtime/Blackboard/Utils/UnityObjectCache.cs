using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

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
                Count = 1;
            }

            internal readonly Object Reference;
            internal uint Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnityObjectCache(int capacity) => _objectCache = new Dictionary<int, ObjectCacheData>(capacity);

        private readonly Dictionary<int, ObjectCacheData> _objectCache;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Clear() => _objectCache.Clear();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Object Read(int instanceID) =>
            instanceID != 0 && _objectCache.TryGetValue(instanceID, out var data) ? data.Reference : null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Remove(int instanceID)
        {
            if (instanceID == 0 || !_objectCache.TryGetValue(instanceID, out var dataToRemove))
                return;

            dataToRemove.Count--;

            if (dataToRemove.Count == 0) _objectCache.Remove(instanceID);
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
                currentData.Count++;
                _objectCache[instanceID] = currentData;
            }
            else
            {
                _objectCache.Add(instanceID, new ObjectCacheData(value));
            }

            return instanceID;
        }
    }
}