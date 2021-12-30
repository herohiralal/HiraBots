using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Method)]
    public class GenerateHiraBotsDecoratorAttribute : Attribute
    {
        public GenerateHiraBotsDecoratorAttribute(string guid)
        {
            this.guid = guid ?? "";
        }

        public string guid { get; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GenerateHiraBotsScoreCalculatorAttribute : Attribute
    {
        public GenerateHiraBotsScoreCalculatorAttribute(string guid)
        {
            this.guid = guid ?? "";
        }

        public string guid { get; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GenerateHiraBotsEffectorAttribute : Attribute
    {
        public GenerateHiraBotsEffectorAttribute(string guid)
        {
            this.guid = guid ?? "";
        }

        public string guid { get; }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class MatchTypeToEnumKeyAttribute : Attribute
    {
        public MatchTypeToEnumKeyAttribute(string argumentName)
        {
            this.argumentName = argumentName ?? "";
        }

        public string argumentName { get; }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class HiraBotsBlackboardKeyAttribute : Attribute
    {
        public HiraBotsBlackboardKeyAttribute(BlackboardKeyType keyType)
        {
            this.keyType = keyType;
        }

        public BlackboardKeyType keyType { get; }
    }
}