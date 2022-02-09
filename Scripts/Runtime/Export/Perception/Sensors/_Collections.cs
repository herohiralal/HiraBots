using HiraBots;
using Unity.Collections;
using Unity.Mathematics;

namespace UnityEngine.AI
{
    public struct PerceivedObjectsList
    {
        internal PerceivedObjectsList(NativeArray<UnmanagedCollections.Data<int>> value)
        {
            m_Internal = value;
        }

        private NativeArray<UnmanagedCollections.Data<int>> m_Internal;

        public void Add(int obj)
        {
            m_Internal.Push(obj);
        }
    }

    public struct PerceivedObjectsLocationsList
    {
        internal PerceivedObjectsLocationsList(bool valid, NativeArray<UnmanagedCollections.Data<float4>> value)
        {
            isValid = valid;
            m_Internal = value;
        }

        public bool isValid { get; }
        private NativeArray<UnmanagedCollections.Data<float4>> m_Internal;

        public void Add(float4 location)
        {
            m_Internal.Push(location);
        }
    }
}