using AOT;
using Unity.Burst;

namespace HiraBots
{
    [BurstCompile]
    internal unsafe partial class AlwaysSucceedDecoratorBlackboardFunction : DecoratorBlackboardFunction
    {
        private static readonly FunctionPointer<DecoratorDelegate> s_Function;

        static AlwaysSucceedDecoratorBlackboardFunction()
        {
            s_Function = BurstCompiler.CompileFunctionPointer<DecoratorDelegate>(ActualFunction);
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