using HiraBots;
using Unity.Collections;

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
}