using UnityEngine;

namespace HiraBots
{
    internal partial class ObjectBlackboardKey : BlackboardKey
    {
        internal ObjectBlackboardKey()
        {
            SizeInBytesInternal = sizeof(int);
            KeyType = BlackboardKeyType.Object;
        }

        [SerializeField] private Object defaultValue = null;
    }
}