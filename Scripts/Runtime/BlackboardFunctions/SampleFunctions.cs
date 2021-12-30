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
        internal static bool AlwaysSucceedDecorator()
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