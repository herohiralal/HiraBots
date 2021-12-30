using Unity.Mathematics;
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

    internal enum IntegerComparisonType : byte
    {
        Equals,
        GreaterThan,
        GreaterThanEqualTo,
        LesserThan,
        LesserThanEqualTo
    }

    internal enum FloatComparisonType : byte
    {
        AlmostEquals,
        GreaterThan,
        GreaterThanEqualTo,
        LesserThan,
        LesserThanEqualTo
    }

    internal static class SampleDecoratorBlackboardFunctions
    {
        [GenerateInternalBlackboardFunction("88371d2dafc44604aee17d7062a33f02")]
        internal static bool AlwaysSucceedDecorator()
        {
            return true;
        }

        [GenerateInternalBlackboardFunction("76cf91d3f677494e89e4545c520c6644")]
        internal static bool EnumEqualsDecorator(ref byte key, [MatchTypeToEnumKey("key")] byte value)
        {
            return key == value;
        }

        [GenerateInternalBlackboardFunction("841fea4ba9b94948b9a5ff847474ea57")]
        internal static bool EnumHasFlagsDecorator(ref byte key, [MatchTypeToEnumKey("key")] byte value)
        {
            return (key & value) != 0;
        }

        [GenerateInternalBlackboardFunction("841fea4ba9b94948b9b5ff847474ea57")]
        internal static bool BooleanIsSetDecorator(ref bool key)
        {
            return key;
        }

        [GenerateInternalBlackboardFunction("841fea4ba9b94948c9a5ff847474ea57")]
        internal static bool QuaternionIsSetDecorator(ref quaternion key)
        {
            var value4 = key.value == quaternion.identity.value;
            return !value4.w || !value4.x || !value4.y || !value4.z;
        }

        [GenerateInternalBlackboardFunction("841fea4ba9b94d48b9a5ff847474ea57")]
        internal static bool VectorIsSetDecorator(ref float3 key)
        {
            var value3 = key == float3.zero;
            return !value3.x || !value3.y || !value3.z;
        }

        [GenerateInternalBlackboardFunction("3b6d337fe0fa4b269f045171ef4871cd")]
        internal static bool IntegerComparisonDecorator(ref int key, int secondValue, IntegerComparisonType comparisonType)
        {
            switch (comparisonType)
            {
                case IntegerComparisonType.Equals: return key == secondValue;
                case IntegerComparisonType.GreaterThan: return key > secondValue;
                case IntegerComparisonType.GreaterThanEqualTo: return key >= secondValue;
                case IntegerComparisonType.LesserThan: return key < secondValue;
                case IntegerComparisonType.LesserThanEqualTo: return key <= secondValue;
                default: return false;
            }
        }

        [GenerateInternalBlackboardFunction("3b6d337fe0fa4b269f145171ef4871cd")]
        internal static bool FloatComparisonDecorator(ref float key, float secondValue, float equalityTolerance, FloatComparisonType comparisonType)
        {
            switch (comparisonType)
            {
                case FloatComparisonType.AlmostEquals: return math.abs(key - secondValue) <= equalityTolerance;
                case FloatComparisonType.GreaterThan: return key > secondValue;
                case FloatComparisonType.GreaterThanEqualTo: return key >= secondValue;
                case FloatComparisonType.LesserThan: return key < secondValue;
                case FloatComparisonType.LesserThanEqualTo: return key <= secondValue;
                default: return false;
            }
        }

        [GenerateInternalBlackboardFunction("e1ecc27574ca4a030ff85de39d096ccb")]
        internal static bool ObjectEqualsDecorator([HiraBotsObjectKey] ref int key, [HiraBotsObjectValue] int value)
        {
            return key == value;
        }
    }

    internal static class SampleScoreCalculatorBlackboardFunctions
    {
    }

    internal static class SampleEffectorBlackboardFunctions
    {
    }
}