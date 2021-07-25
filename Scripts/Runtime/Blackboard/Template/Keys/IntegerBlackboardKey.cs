using UnityEngine;

namespace HiraBots
{
    internal partial class IntegerBlackboardKey : BlackboardKey
    {
        internal IntegerBlackboardKey() : base(sizeof(int), BlackboardKeyType.Integer)
        {
        }

        [SerializeField] private int defaultValue = 0;
    }
}