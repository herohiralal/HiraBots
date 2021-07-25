using UnityEngine;

namespace HiraBots
{
    internal partial class ObjectBlackboardKey : BlackboardKey
    {
        internal ObjectBlackboardKey() : base(sizeof(int), BlackboardKeyType.Object)
        {
        }

        [SerializeField] private Object defaultValue = null;
    }
}