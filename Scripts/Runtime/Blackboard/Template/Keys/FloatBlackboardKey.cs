using UnityEngine;

namespace HiraBots
{
    internal unsafe class FloatBlackboardKey : BlackboardKey
    {
        public FloatBlackboardKey() : base(sizeof(float), BlackboardKeyType.Float)
        {
        }

        [SerializeField] private float defaultValue = 0f;

        protected override void CompileInternal(IBlackboardKeyCompilerContext context) =>
            BlackboardUnsafeHelpers.WriteFloatValue(context.Address, 0, defaultValue);
    }
}