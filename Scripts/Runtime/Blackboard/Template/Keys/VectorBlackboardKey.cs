using UnityEngine;

namespace HiraBots
{
    internal unsafe partial class VectorBlackboardKey : BlackboardKey
    {
        internal VectorBlackboardKey() : base(sizeof(float) * 3, BlackboardKeyType.Vector)
        {
        }

        [SerializeField] private Vector3 defaultValue = Vector3.zero;

        protected override void CompileInternal(IBlackboardKeyCompilerContext context) =>
            BlackboardUnsafeHelpers.WriteVectorValue(context.Address, 0, defaultValue);
    }
}