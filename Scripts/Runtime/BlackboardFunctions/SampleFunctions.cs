using UnityEngine;

namespace HiraBots
{
    internal class GenerateInternalBlackboardFunctionAttribute : System.Attribute
    {
        internal GenerateInternalBlackboardFunctionAttribute(string guid)
        {
            this.guid = guid;
        }

        internal string guid { get; }
    }

    internal static class SampleDecoratorBlackboardFunctions
    {
        [GenerateInternalBlackboardFunction("88371d2dafc44604aee17d7062a33f02")]
        internal static bool AlwaysSucceedDecoratorBlackboardFunction(UnityEngine.BlackboardComponent blackboard, bool expected)
        {
            return true;
        }

        internal static bool AlwaysSucceedDecoratorBlackboardFunctionUnmanaged(UnityEngine.BlackboardComponent.LowLevel blackboard)
        {
            return true;
        }

        [GenerateInternalBlackboardFunction("02699b8440814f6a8f957e561f36a32c")]
        internal static bool ObjectEqualsDecoratorBlackboardFunction(UnityEngine.BlackboardComponent blackboard, bool expected, [HiraBotsBlackboardKey(UnityEngine.BlackboardKeyType.Object)] UnityEngine.BlackboardKey key, Object value)
        {
            return blackboard.GetObjectValue(key.name) == value;
        }

        internal static bool ObjectEqualsDecoratorBlackboardFunctionUnmanaged(UnityEngine.BlackboardComponent.LowLevel blackboard, UnityEngine.BlackboardKey.LowLevel key, int value)
        {
            return blackboard.Access<int>(key.offset) == value;
        }
    }

    internal static class SampleScoreCalculatorBlackboardFunctions
    {

    }

    internal static class SampleEffectorBlackboardFunctions
    {

    }
}