using System.Runtime.CompilerServices;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("88371d2dafc44604aee17d7062a33f02")]
        internal static bool AlwaysFailDecorator()
        {
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void AlwaysFailDecoratorUpdateDescription(out string staticDescription)
        {
            staticDescription = "Always fail. Used to disable a goal/task for debugging purposes.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("76cf91d3f677494e89e4545c520c6644")]
        internal static bool EnumEqualsDecorator(bool invert, ref byte key, [MatchTypeToEnumKey("key")] byte value)
        {
            var result = key == value;
            return invert != result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void EnumEqualsDecoratorUpdateDescription(bool invert, UnityEngine.BlackboardTemplate.KeySelector key, UnityEngine.DynamicEnum value, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"{key.selectedKey.name} {(invert ? "must not" : "must")} be equal to the selected value.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("841fea4ba9b94948b9a5ff847474ea57")]
        internal static bool EnumHasFlagsDecorator(bool invert, ref byte key, [MatchTypeToEnumKey("key")] byte value)
        {
            var result = (key & value) != 0;
            return invert != result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void EnumHasFlagsDecoratorUpdateDescription(bool invert, UnityEngine.BlackboardTemplate.KeySelector key, UnityEngine.DynamicEnum value, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"{key.selectedKey.name} {(invert ? "must not" : "must")} have these flags.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("841fea4ba9b94948b9b5ff847474ea57")]
        internal static bool BooleanIsSetDecorator(bool invert, ref bool key)
        {
            var result = key;
            return invert != result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void BooleanIsSetDecoratorUpdateDescription(bool invert, UnityEngine.BlackboardTemplate.KeySelector key, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"{key.selectedKey.name} {(invert ? "must not" : "must")} be set.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("841fea4ba9b94948c9a5ff847474ea57")]
        internal static bool QuaternionIsSetDecorator(bool invert, ref quaternion key)
        {
            var value4 = key.value == quaternion.identity.value;
            var result = !value4.w || !value4.x || !value4.y || !value4.z;
            return invert != result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void QuaternionIsSetDecoratorUpdateDescription(bool invert, UnityEngine.BlackboardTemplate.KeySelector key, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"{key.selectedKey.name} {(invert ? "must not" : "must")} be set.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("841fea4ba9b94d48b9a5ff847474ea57")]
        internal static bool VectorIsSetDecorator(bool invert, ref float3 key)
        {
            var value3 = key == float3.zero;
            var result = !value3.x || !value3.y || !value3.z;
            return invert != result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void VectorIsSetDecoratorUpdateDescription(bool invert, UnityEngine.BlackboardTemplate.KeySelector key, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"{key.selectedKey.name} {(invert ? "must not" : "must")} be set.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("3b6d337fe0fa4b269f045171ef4871cd")]
        internal static bool IntegerComparisonDecorator(bool invert, ref int key, int secondValue, IntegerComparisonType comparisonType)
        {
            bool result;
            switch (comparisonType)
            {
                case IntegerComparisonType.Equals:
                    result = key == secondValue;
                    break;
                case IntegerComparisonType.GreaterThan:
                    result = key > secondValue;
                    break;
                case IntegerComparisonType.GreaterThanEqualTo:
                    result = key >= secondValue;
                    break;
                case IntegerComparisonType.LesserThan:
                    result = key < secondValue;
                    break;
                case IntegerComparisonType.LesserThanEqualTo:
                    result = key <= secondValue;
                    break;
                default:
                    result = false;
                    break;
            }

            return invert != result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void IntegerComparisonDecoratorUpdateDescription(bool invert, UnityEngine.BlackboardTemplate.KeySelector key, int secondValue, IntegerComparisonType comparisonType, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            string comparison;
            switch (comparisonType)
            {
                case IntegerComparisonType.Equals:
                    comparison = "equal to";
                    break;
                case IntegerComparisonType.GreaterThan:
                    comparison = "greater than";
                    break;
                case IntegerComparisonType.GreaterThanEqualTo:
                    comparison = "greater than or equal to";
                    break;
                case IntegerComparisonType.LesserThan:
                    comparison = "lesser than";
                    break;
                case IntegerComparisonType.LesserThanEqualTo:
                    comparison = "lesser than or equal to";
                    break;
                default:
                    staticDescription = "";
                    return;
            }

            staticDescription = $"{key.selectedKey.name} {(invert ? "must not" : "must")} be {comparison} {secondValue}.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("3b6d337fe0fa4b269f145171ef4871cd")]
        internal static bool FloatComparisonDecorator(bool invert, ref float key, float secondValue, float equalityTolerance, FloatComparisonType comparisonType)
        {
            bool result;
            switch (comparisonType)
            {
                case FloatComparisonType.AlmostEquals:
                    result = math.abs(key - secondValue) <= equalityTolerance;
                    break;
                case FloatComparisonType.GreaterThan:
                    result = key > secondValue;
                    break;
                case FloatComparisonType.GreaterThanEqualTo:
                    result = key >= secondValue;
                    break;
                case FloatComparisonType.LesserThan:
                    result = key < secondValue;
                    break;
                case FloatComparisonType.LesserThanEqualTo:
                    result = key <= secondValue;
                    break;
                default:
                    result = false;
                    break;
            }

            return invert != result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void FloatComparisonDecoratorUpdateDescription(bool invert, UnityEngine.BlackboardTemplate.KeySelector key, float secondValue, float equalityTolerance, FloatComparisonType comparisonType, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            var valueStr = $"{secondValue}";

            string comparison;
            switch (comparisonType)
            {
                case FloatComparisonType.AlmostEquals:
                    comparison = "equal to";
                    if (equalityTolerance != 0f)
                    {
                        valueStr = $"{valueStr} ± {equalityTolerance}";
                    }
                    break;
                case FloatComparisonType.GreaterThan:
                    comparison = "greater than";
                    break;
                case FloatComparisonType.GreaterThanEqualTo:
                    comparison = "greater than or equal to";
                    break;
                case FloatComparisonType.LesserThan:
                    comparison = "lesser than";
                    break;
                case FloatComparisonType.LesserThanEqualTo:
                    comparison = "lesser than or equal to";
                    break;
                default:
                    staticDescription = "";
                    return;
            }

            staticDescription = $"{key.selectedKey.name} {(invert ? "must not" : "must")} be {comparison} {valueStr}.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("e1ecc27574ca4a030ff85de39d096ccb")]
        internal static bool ObjectEqualsDecorator(bool invert, [HiraBotsObjectKey] ref int key, [HiraBotsObjectValue] int value)
        {
            var result = key == value;
            return invert != result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ObjectEqualsDecoratorUpdateDescription(bool invert, UnityEngine.BlackboardTemplate.KeySelector key, Object value, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"{key.selectedKey.name} {(invert ? "must not" : "must")} be set to {(value == null ? "null" : value.name)}.";
        }
    }

    internal static class SampleScoreCalculatorBlackboardFunctions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string ScoreString(float score)
        {
            return $"{(score >= 0 ? '+' : '-')}{score}.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("88371d2dafc44604aee17d7062a33f0c")]
        internal static float BaseScoreScoreCalculator(float currentScore, float score)
        {
            return currentScore + score;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void BaseScoreScoreCalculatorUpdateDescription(float score, out string staticDescription)
        {
            staticDescription = $"{ScoreString(score)}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("76cf91d3f677494e89e4545c520c664c")]
        internal static float EnumEqualsScoreCalculator(float currentScore, bool invert, ref byte key, [MatchTypeToEnumKey("key")] byte value, float score)
        {
            return currentScore + (SampleDecoratorBlackboardFunctions.EnumEqualsDecorator(invert, ref key, value) ? score : 0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void EnumEqualsScoreCalculatorUpdateDescription(bool invert, UnityEngine.BlackboardTemplate.KeySelector key, UnityEngine.DynamicEnum value, float score, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"{ScoreString(score)} if {key.selectedKey.name} {(invert ? "is not" : "is")} equal to the selected value.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("841fea4ba9b94948b9a5ff847474ea5c")]
        internal static float EnumHasFlagsScoreCalculator(float currentScore, bool invert, ref byte key, [MatchTypeToEnumKey("key")] byte value, float score)
        {
            return currentScore + (SampleDecoratorBlackboardFunctions.EnumHasFlagsDecorator(invert, ref key, value) ? score : 0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void EnumHasFlagsScoreCalculatorUpdateDescription(bool invert, UnityEngine.BlackboardTemplate.KeySelector key, UnityEngine.DynamicEnum value, float score, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"{ScoreString(score)} if {key.selectedKey.name} {(invert ? "does not have" : "has")} these flags.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("841fea4ba9b94948b9b5ff847474ea5c")]
        internal static float BooleanIsSetScoreCalculator(float currentScore, bool invert, ref bool key, float score)
        {
            return currentScore + (SampleDecoratorBlackboardFunctions.BooleanIsSetDecorator(invert, ref key) ? score : 0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void BooleanIsSetScoreCalculatorUpdateDescription(bool invert, UnityEngine.BlackboardTemplate.KeySelector key, float score, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"{ScoreString(score)} if {key.selectedKey.name} {(invert ? "is not" : "is")} set.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("841fea4ba9b94948c9a5ff847474ea5c")]
        internal static float QuaternionIsSetScoreCalculator(float currentScore, bool invert, ref quaternion key, float score)
        {
            return currentScore + (SampleDecoratorBlackboardFunctions.QuaternionIsSetDecorator(invert, ref key) ? score : 0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void QuaternionIsSetScoreCalculatorUpdateDescription(bool invert, UnityEngine.BlackboardTemplate.KeySelector key, float score, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"{ScoreString(score)} if {key.selectedKey.name} {(invert ? "is not" : "is")} set.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("841fea4ba9b94d48b9a5ff847474ea5c")]
        internal static float VectorIsSetScoreCalculator(float currentScore, bool invert, ref float3 key, float score)
        {
            return currentScore + (SampleDecoratorBlackboardFunctions.VectorIsSetDecorator(invert, ref key) ? score : 0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void VectorIsSetScoreCalculatorUpdateDescription(bool invert, UnityEngine.BlackboardTemplate.KeySelector key, float score, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"{ScoreString(score)} if {key.selectedKey.name} {(invert ? "is not" : "is")} set.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("3b6d337fe0fa4b269f045171ef4871cc")]
        internal static float IntegerComparisonScoreCalculator(float currentScore, bool invert, ref int key, int secondValue, IntegerComparisonType comparisonType, float score)
        {
            return currentScore + (SampleDecoratorBlackboardFunctions.IntegerComparisonDecorator(invert, ref key, secondValue, comparisonType) ? score : 0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void IntegerComparisonScoreCalculatorUpdateDescription(bool invert, UnityEngine.BlackboardTemplate.KeySelector key, int secondValue, IntegerComparisonType comparisonType, float score, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            string comparison;
            switch (comparisonType)
            {
                case IntegerComparisonType.Equals:
                    comparison = "equal to";
                    break;
                case IntegerComparisonType.GreaterThan:
                    comparison = "greater than";
                    break;
                case IntegerComparisonType.GreaterThanEqualTo:
                    comparison = "greater than or equal to";
                    break;
                case IntegerComparisonType.LesserThan:
                    comparison = "lesser than";
                    break;
                case IntegerComparisonType.LesserThanEqualTo:
                    comparison = "lesser than or equal to";
                    break;
                default:
                    staticDescription = "";
                    return;
            }

            staticDescription = $"{ScoreString(score)} if {key.selectedKey.name} {(invert ? "is not" : "is")} {comparison} {secondValue}.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("3b6d337fe0fa4b269f145171ef4871cc")]
        internal static float FloatComparisonScoreCalculator(float currentScore, bool invert, ref float key, float secondValue, float equalityTolerance, FloatComparisonType comparisonType, float score)
        {
            return currentScore + (SampleDecoratorBlackboardFunctions.FloatComparisonDecorator(invert, ref key, secondValue, equalityTolerance, comparisonType) ? score : 0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void FloatComparisonScoreCalculatorUpdateDescription(bool invert, UnityEngine.BlackboardTemplate.KeySelector key, float secondValue, float equalityTolerance, FloatComparisonType comparisonType, float score, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            var valueStr = $"{secondValue}";

            string comparison;
            switch (comparisonType)
            {
                case FloatComparisonType.AlmostEquals:
                    comparison = "equal to";
                    if (equalityTolerance != 0f)
                    {
                        valueStr = $"{valueStr} ± {equalityTolerance}";
                    }
                    break;
                case FloatComparisonType.GreaterThan:
                    comparison = "greater than";
                    break;
                case FloatComparisonType.GreaterThanEqualTo:
                    comparison = "greater than or equal to";
                    break;
                case FloatComparisonType.LesserThan:
                    comparison = "lesser than";
                    break;
                case FloatComparisonType.LesserThanEqualTo:
                    comparison = "lesser than or equal to";
                    break;
                default:
                    staticDescription = "";
                    return;
            }

            staticDescription = $"{ScoreString(score)} if {key.selectedKey.name} {(invert ? "is not" : "is")} {comparison} {valueStr}.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("e1ecc27574ca4a030ff85de39d096ccc")]
        internal static float ObjectEqualsScoreCalculator(float currentScore, bool invert, [HiraBotsObjectKey] ref int key, [HiraBotsObjectValue] int value, float score)
        {
            return currentScore + (SampleDecoratorBlackboardFunctions.ObjectEqualsDecorator(invert, ref key, value) ? score : 0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ObjectEqualsScoreCalculatorUpdateDescription(bool invert, UnityEngine.BlackboardTemplate.KeySelector key, Object value, float score, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"{ScoreString(score)} if {key.selectedKey.name} {(invert ? "is not" : "is")} set to {(value == null ? "null" : value.name)}.";
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void FloatOperatorEffectorOnValidate(ref UnityEngine.BlackboardTemplate.KeySelector key, ref FloatOperationType operationType, ref float value)
        {
            if (operationType == FloatOperationType.Divide && value == 0f)
            {
                value = 1f;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void IntegerOperatorEffectorOnValidate(ref UnityEngine.BlackboardTemplate.KeySelector key, ref IntegerOperationType operationType, ref int value)
        {
            if (operationType == IntegerOperationType.Divide && value == 0)
            {
                value = 1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("da5818b29555439cb0a6adc4d0937280")]
        // ReSharper disable once RedundantAssignment
        internal static void ObjectEqualsEffector([HiraBotsObjectKey] ref int key, [HiraBotsObjectValue] int value)
        {
            key = value;
        }
    }
}