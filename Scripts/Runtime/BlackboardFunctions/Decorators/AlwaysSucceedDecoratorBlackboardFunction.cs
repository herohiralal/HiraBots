using AOT;
using Unity.Burst;

namespace HiraBots
{
    [BurstCompile]
    internal unsafe partial class AlwaysSucceedDecoratorBlackboardFunction : DecoratorBlackboardFunction
    {
        // actual function
        [BurstCompile(DisableDirectCall = true), MonoPInvokeCallback(typeof(DecoratorDelegate))]
        private static bool ActualFunction(in LowLevelBlackboard blackboard, byte* rawMemory)
        {
            return true;
        }
    }
}