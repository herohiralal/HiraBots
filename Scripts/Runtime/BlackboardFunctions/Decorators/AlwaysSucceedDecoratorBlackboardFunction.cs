using AOT;
using Unity.Burst;

namespace HiraBots
{
    /// <summary>
    /// This decorator will always succeed, and so the score calculator version can be
    /// used as a base score calculator.
    /// </summary>
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