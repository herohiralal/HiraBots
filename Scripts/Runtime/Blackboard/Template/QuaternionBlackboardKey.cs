using UnityEngine;

namespace HiraBots
{
    internal partial class QuaternionBlackboardKey : BlackboardKey
    {
        internal QuaternionBlackboardKey()
        {
            SizeInBytesInternal = sizeof(float) * 4;
            KeyType = BlackboardKeyType.Quaternion;
        }

        [SerializeField] private Vector3 defaultValue = Vector3.zero;
    }
}