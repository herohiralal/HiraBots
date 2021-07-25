using UnityEngine;

namespace HiraBots
{
    internal partial class BooleanBlackboardKey : BlackboardKey
    {
        internal BooleanBlackboardKey()
        {
            SizeInBytesInternal = sizeof(byte);
            KeyType = BlackboardKeyType.Boolean;
        }

        [SerializeField] private bool defaultValue = false;
    }
}