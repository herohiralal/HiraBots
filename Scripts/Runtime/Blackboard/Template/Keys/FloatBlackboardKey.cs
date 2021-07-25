using UnityEngine;

namespace HiraBots
{
    internal partial class FloatBlackboardKey : BlackboardKey
    {
        internal FloatBlackboardKey() : base(sizeof(float), BlackboardKeyType.Float)
        {
        }

        [SerializeField] private float defaultValue = 0f;
    }
}