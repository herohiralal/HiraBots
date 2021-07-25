using UnityEngine;

namespace HiraBots
{
    internal partial class IntegerBlackboardKey : BlackboardKey
    {
        internal IntegerBlackboardKey()
        {
            SizeInBytesInternal = sizeof(int);
            KeyType = BlackboardKeyType.Integer;
        }

        [SerializeField] private int defaultValue = 0;
    }
}