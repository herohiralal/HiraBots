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

    }

    internal static class SampleScoreCalculatorBlackboardFunctions
    {

    }

    internal static class SampleEffectorBlackboardFunctions
    {

    }
}