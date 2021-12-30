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

    [System.Serializable]
    internal enum IntegerComparisonType : byte
    {
        Equals,
        GreaterThan,
        GreaterThanEqualTo,
        LesserThan,
        LesserThanEqualTo
    }

    [System.Serializable]
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
        [GenerateInternalBlackboardFunction("88371d2dafc44604aee17d7062a33f0c")]
        internal static float AlwaysSucceedScoreCalculator(float currentScore, float score)
        {
            return currentScore + (SampleDecoratorBlackboardFunctions.AlwaysSucceedDecorator() ? score : 0f);
        }

        [GenerateInternalBlackboardFunction("76cf91d3f677494e89e4545c520c664c")]
        internal static float EnumEqualsScoreCalculator(float currentScore, ref byte key, [MatchTypeToEnumKey("key")] byte value, float score)
        {
            return currentScore + (SampleDecoratorBlackboardFunctions.EnumEqualsDecorator(ref key, value) ? score : 0f);
        }

        [GenerateInternalBlackboardFunction("841fea4ba9b94948b9a5ff847474ea5c")]
        internal static float EnumHasFlagsScoreCalculator(float currentScore, ref byte key, [MatchTypeToEnumKey("key")] byte value, float score)
        {
            return currentScore + (SampleDecoratorBlackboardFunctions.EnumHasFlagsDecorator(ref key, value) ? score : 0f);
        }

        [GenerateInternalBlackboardFunction("841fea4ba9b94948b9b5ff847474ea5c")]
        internal static float BooleanIsSetScoreCalculator(float currentScore, ref bool key, float score)
        {
            return currentScore + (SampleDecoratorBlackboardFunctions.BooleanIsSetDecorator(ref key) ? score : 0f);
        }

        [GenerateInternalBlackboardFunction("841fea4ba9b94948c9a5ff847474ea5c")]
        internal static float QuaternionIsSetScoreCalculator(float currentScore, ref quaternion key, float score)
        {
            return currentScore + (SampleDecoratorBlackboardFunctions.QuaternionIsSetDecorator(ref key) ? score : 0f);
        }

        [GenerateInternalBlackboardFunction("841fea4ba9b94d48b9a5ff847474ea5c")]
        internal static float VectorIsSetScoreCalculator(float currentScore, ref float3 key, float score)
        {
            return currentScore + (SampleDecoratorBlackboardFunctions.VectorIsSetDecorator(ref key) ? score : 0f);
        }

        [GenerateInternalBlackboardFunction("3b6d337fe0fa4b269f045171ef4871cc")]
        internal static float IntegerComparisonScoreCalculator(float currentScore, ref int key, int secondValue, IntegerComparisonType comparisonType, float score)
        {
            return currentScore + (SampleDecoratorBlackboardFunctions.IntegerComparisonDecorator(ref key, secondValue, comparisonType) ? score : 0f);
        }

        [GenerateInternalBlackboardFunction("3b6d337fe0fa4b269f145171ef4871cc")]
        internal static float FloatComparisonScoreCalculator(float currentScore, ref float key, float secondValue, float equalityTolerance, FloatComparisonType comparisonType, float score)
        {
            return currentScore + (SampleDecoratorBlackboardFunctions.FloatComparisonDecorator(ref key, secondValue, equalityTolerance, comparisonType) ? score : 0f);
        }

        [GenerateInternalBlackboardFunction("e1ecc27574ca4a030ff85de39d096ccc")]
        internal static float ObjectEqualsScoreCalculator(float currentScore, [HiraBotsObjectKey] ref int key, [HiraBotsObjectValue] int value, float score)
        {
            return currentScore + (SampleDecoratorBlackboardFunctions.ObjectEqualsDecorator(ref key, value) ? score : 0f);
        }
    }

    [System.Serializable]
    internal enum EnumOperationType
    {
        Set,
        AddFlags,
        RemoveFlags
    }

    [System.Serializable]
    internal enum FloatOperationType
    {
        Set,
        Add,
        Subtract,
        Multiply,
        Divide,
    }

    [System.Serializable]
    internal enum IntegerOperationType
    {
        Set,
        Add,
        Subtract,
        Multiply,
        Divide,
        BitwiseAnd,
        BitwiseOr,
        BitwiseXor,
    }

    [System.Serializable]
    internal enum SetOperationType
    {
        Set,
        Unset
    }

    internal static class SampleEffectorBlackboardFunctions
    {
        [GenerateInternalBlackboardFunction("ad6818e4de9846adba80edd01785671e")]
        internal static void EnumOperatorEffector(ref byte key, EnumOperationType operationType, [MatchTypeToEnumKey("key")] byte value)
        {
            switch (operationType)
            {
                case EnumOperationType.Set: key = value;
                    break;
                case EnumOperationType.AddFlags: key |= value;
                    break;
                case EnumOperationType.RemoveFlags: key = (byte) (key & ~value);
                    break;
                default:
                    return;
            }
        }

        [GenerateInternalBlackboardFunction("89af0e100fac43f1ba83126b2d7c87de")]
        internal static void FloatOperatorEffector(ref float key, FloatOperationType operationType, float value)
        {
            switch (operationType)
            {
                case FloatOperationType.Set:
                    key = value;
                    break;
                case FloatOperationType.Add:
                    key += value;
                    break;
                case FloatOperationType.Subtract:
                    key -= value;
                    break;
                case FloatOperationType.Multiply:
                    key *= value;
                    break;
                case FloatOperationType.Divide:
                    key /= value;
                    break;
                default:
                    return;
            }
        }

        [GenerateInternalBlackboardFunction("f073722b6c814acdb3f6485ca22b0f0e")]
        internal static void IntegerOperatorEffector(ref int key, IntegerOperationType operationType, int value)
        {
            switch (operationType)
            {
                case IntegerOperationType.Set:
                    key = value;
                    break;
                case IntegerOperationType.Add:
                    key += value;
                    break;
                case IntegerOperationType.Subtract:
                    key -= value;
                    break;
                case IntegerOperationType.Multiply:
                    key *= value;
                    break;
                case IntegerOperationType.Divide:
                    key /= value;
                    break;
                case IntegerOperationType.BitwiseAnd:
                    key &= value;
                    break;
                case IntegerOperationType.BitwiseOr:
                    key |= value;
                    break;
                case IntegerOperationType.BitwiseXor:
                    key ^= value;
                    break;
                default:
                    return;
            }
        }

        [GenerateInternalBlackboardFunction("bf5cce8d3ca6458bbcf415de101c99e1")]
        internal static void BooleanIsSetEffector(ref bool key, SetOperationType operationType)
        {
            switch (operationType)
            {
                case SetOperationType.Set:
                    key = true;
                    break;
                case SetOperationType.Unset:
                    key = false;
                    break;
                default:
                    return;
            }
        }

        [GenerateInternalBlackboardFunction("3e0f93fa04dd434da94e08d68526c5b0")]
        internal static void VectorIsSetEffector(ref float3 key, SetOperationType operationType)
        {
            switch (operationType)
            {
                case SetOperationType.Set:
                    key = 1;
                    break;
                case SetOperationType.Unset:
                    key = float3.zero;
                    break;
                default:
                    return;
            }
        }

        [GenerateInternalBlackboardFunction("81995b480d8141c2903d789771e5cd7c")]
        internal static void QuaternionIsSetEffector(ref quaternion key, SetOperationType operationType)
        {
            switch (operationType)
            {
                case SetOperationType.Set:
                    key = quaternion.Euler(new float3(10));
                    break;
                case SetOperationType.Unset:
                    key = quaternion.identity;
                    break;
                default:
                    return;
            }
        }

        [GenerateInternalBlackboardFunction("da5818b29555439cb0a6adc4d0937280")]
        // ReSharper disable once RedundantAssignment
        internal static void ObjectEqualsEffector([HiraBotsObjectKey] ref int key, [HiraBotsObjectValue] int value)
        {
            key = value;
        }
    }
}