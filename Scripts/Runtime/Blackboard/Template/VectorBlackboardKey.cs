using UnityEngine;

namespace HiraBots
{
    internal partial class VectorBlackboardKey : BlackboardKey
    {
        internal VectorBlackboardKey()
        {
            SizeInBytesInternal = sizeof(float) * 3;
            KeyType = BlackboardKeyType.Vector;
        }

        [SerializeField] private Vector3 defaultValue = Vector3.zero;
    }
}