using UnityEngine;

namespace HiraBots
{
    internal unsafe class IntegerBlackboardKey : BlackboardKey
    {
        internal IntegerBlackboardKey() : base(sizeof(int), BlackboardKeyType.Integer)
        {
        }

        [SerializeField] private int defaultValue = 0;

        protected override void CompileInternal(IBlackboardKeyCompilerContext context) =>
            BlackboardUnsafeHelpers.WriteIntegerValue(context.Address, 0, defaultValue);
    }
}