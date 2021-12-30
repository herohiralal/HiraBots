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
    }

    internal static class SampleScoreCalculatorBlackboardFunctions
    {

    }

    internal static class SampleEffectorBlackboardFunctions
    {

    }
}