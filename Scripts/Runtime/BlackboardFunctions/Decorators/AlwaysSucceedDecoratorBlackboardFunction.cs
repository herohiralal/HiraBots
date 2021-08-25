using AOT;
using Unity.Burst;

namespace HiraBots
{
    [BurstCompile]
    internal unsafe partial class AlwaysSucceedDecoratorBlackboardFunction : DecoratorBlackboardFunction
    {
        private static bool s_FunctionCompiled = false;
        private static FunctionPointer<DecoratorDelegate> s_Function;

        internal override void PrepareForCompilation()
        {
            base.PrepareForCompilation();
            if (!s_FunctionCompiled)
            {
                s_Function = BurstCompiler.CompileFunctionPointer<DecoratorDelegate>(ActualFunction);
                s_FunctionCompiled = true;
            }
        }

        // function override
        protected override FunctionPointer<DecoratorDelegate> function => s_Function;

        // actual function
        [BurstCompile(DisableDirectCall = true), MonoPInvokeCallback(typeof(DecoratorDelegate))]
        private static bool ActualFunction(in LowLevelBlackboard blackboard, byte* rawMemory)
        {
            return true;
        }
    }
}