using UnityEngine;

namespace HiraBots
{
    internal partial class FloatBlackboardKey : BlackboardKey
    {
        internal FloatBlackboardKey()
        {
            SizeInBytesInternal = sizeof(float);
            KeyType = BlackboardKeyType.Float;
        }

        [SerializeField] private float defaultValue = 0f;
    }
}