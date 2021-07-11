using UnityEngine;

namespace HiraBots
{
    internal interface IObjectCache
    {
        int Process(Object o);
        void Release(int instanceID);
        bool TryAccess(int instanceID, out Object o);
    }
}