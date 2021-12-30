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
        internal static bool AlwaysSucceedDecorator(UnityEngine.BlackboardComponent blackboard, bool expected)
        {
            return true;
        }

        internal static bool AlwaysSucceedDecoratorUnmanaged(UnityEngine.BlackboardComponent.LowLevel blackboard)
        {
            return true;
        }

        [GenerateInternalBlackboardFunction("02699b8440814f6a8f957e561f36a32c")]
        internal static bool ObjectEqualsDecorator(UnityEngine.BlackboardComponent blackboard, bool expected, [HiraBotsBlackboardKey(UnityEngine.BlackboardKeyType.Object)] UnityEngine.BlackboardKey key, Object value)
        {
            return blackboard.GetObjectValue(key.name) == value;
        }

        internal static bool ObjectEqualsDecoratorUnmanaged(UnityEngine.BlackboardComponent.LowLevel blackboard, UnityEngine.BlackboardKey.LowLevel key, int value)
        {
            return blackboard.Access<int>(key.offset) == value;
        }

        [GenerateInternalBlackboardFunction("c255772ed68446cf8dc50bb72e8372c6")]
        internal static bool EnumHasFlagsDecorator(UnityEngine.BlackboardComponent blackboard, bool expected, [HiraBotsBlackboardKey(UnityEngine.BlackboardKeyType.Enum)] UnityEngine.BlackboardKey key, [MatchTypeToEnumKey("key")] byte value)
        {
            return (blackboard.GetEnumValue(key.name) | value) != 0;
        }

        internal static bool EnumHasFlagsDecoratorUnmanaged(UnityEngine.BlackboardComponent.LowLevel blackboard, UnityEngine.BlackboardKey.LowLevel key, byte value)
        {
            return (blackboard.Access<byte>(key.offset) | value) != 0;
        }
    }

    internal static class SampleScoreCalculatorBlackboardFunctions
    {

    }

    internal static class SampleEffectorBlackboardFunctions
    {

    }
}