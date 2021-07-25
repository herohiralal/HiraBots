using UnityEngine;

namespace HiraBots
{
    internal partial class BooleanBlackboardKey : BlackboardKey
    {
        internal BooleanBlackboardKey() : base(sizeof(byte), BlackboardKeyType.Boolean)
        {
        }

        [SerializeField] private bool defaultValue = false;
    }
}