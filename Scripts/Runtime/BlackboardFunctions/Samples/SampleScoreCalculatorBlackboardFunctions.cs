using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

namespace HiraBots
{
    internal static class SampleScoreCalculatorBlackboardFunctions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string ScoreString(float score)
        {
            return $"{(score >= 0 ? '+' : '-')}{Mathf.Abs(score)}.";
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
        internal static void EnumEqualsScoreCalculatorUpdateDescription(bool invert, UnityEngine.AI.BlackboardTemplate.KeySelector key, UnityEngine.DynamicEnum value, float score, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"{ScoreString(score)} if {key.selectedKey.name} {(invert ? "is not" : "is")} equal to {value}.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("da923644764d4bc597a074f3eca02e3f")]
        internal static float DynamicEnumEqualsScoreCalculator(float currentScore, bool invert, ref byte targetKey, ref byte operatorKey, float score)
        {
            return currentScore + (SampleDecoratorBlackboardFunctions.DynamicEnumEqualsDecorator(invert, ref targetKey, ref operatorKey) ? score : 0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void DynamicEnumEqualsScoreCalculatorUpdateDescription(bool invert, UnityEngine.AI.BlackboardTemplate.KeySelector targetKey, UnityEngine.AI.BlackboardTemplate.KeySelector operatorKey, float score, out string staticDescription)
        {
            if (!targetKey.selectedKey.isValid || !operatorKey.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"{ScoreString(score)} if {targetKey.selectedKey.name} {(invert ? "is not" : "is")} equal to {operatorKey.selectedKey.name}.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("841fea4ba9b94948b9a5ff847474ea5c")]
        internal static float EnumHasFlagsScoreCalculator(float currentScore, bool invert, ref byte key, [MatchTypeToEnumKey("key")] byte value, float score)
        {
            return currentScore + (SampleDecoratorBlackboardFunctions.EnumHasFlagsDecorator(invert, ref key, value) ? score : 0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void EnumHasFlagsScoreCalculatorUpdateDescription(bool invert, UnityEngine.AI.BlackboardTemplate.KeySelector key, UnityEngine.DynamicEnum value, float score, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"{ScoreString(score)} if {key.selectedKey.name} {(invert ? "does not have" : "has")} these flags: {value}.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("7dcce05ed5c04e7aa368d41f4dbe38b4")]
        internal static float DynamicEnumHasFlagsScoreCalculator(float currentScore, bool invert, ref byte targetKey, ref byte operatorKey, float score)
        {
            return currentScore + (SampleDecoratorBlackboardFunctions.DynamicEnumHasFlagsDecorator(invert, ref targetKey, ref operatorKey) ? score : 0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void DynamicEnumHasFlagsScoreCalculatorUpdateDescription(bool invert, UnityEngine.AI.BlackboardTemplate.KeySelector targetKey, UnityEngine.AI.BlackboardTemplate.KeySelector operatorKey, float score, out string staticDescription)
        {
            if (!targetKey.selectedKey.isValid || !operatorKey.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"{ScoreString(score)} if {targetKey.selectedKey.name} {(invert ? "does not have" : "has")} these flags: {operatorKey.selectedKey.name}.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("841fea4ba9b94948b9b5ff847474ea5c")]
        internal static float BooleanIsSetScoreCalculator(float currentScore, bool invert, ref bool key, float score)
        {
            return currentScore + (SampleDecoratorBlackboardFunctions.BooleanIsSetDecorator(invert, ref key) ? score : 0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void BooleanIsSetScoreCalculatorUpdateDescription(bool invert, UnityEngine.AI.BlackboardTemplate.KeySelector key, float score, out string staticDescription)
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
        internal static void QuaternionIsSetScoreCalculatorUpdateDescription(bool invert, UnityEngine.AI.BlackboardTemplate.KeySelector key, float score, out string staticDescription)
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
        internal static void VectorIsSetScoreCalculatorUpdateDescription(bool invert, UnityEngine.AI.BlackboardTemplate.KeySelector key, float score, out string staticDescription)
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
        internal static float IntegerComparisonScoreCalculator(float currentScore, bool invert, ref int key, int secondValue, SampleBlackboardFunctionsIntegerComparisonType comparisonType, float score)
        {
            return currentScore + (SampleDecoratorBlackboardFunctions.IntegerComparisonDecorator(invert, ref key, secondValue, comparisonType) ? score : 0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void IntegerComparisonScoreCalculatorUpdateDescription(bool invert, UnityEngine.AI.BlackboardTemplate.KeySelector key, int secondValue, SampleBlackboardFunctionsIntegerComparisonType comparisonType, float score, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            string comparison;
            switch (comparisonType)
            {
                case SampleBlackboardFunctionsIntegerComparisonType.Equals:
                    comparison = "equal to";
                    break;
                case SampleBlackboardFunctionsIntegerComparisonType.GreaterThan:
                    comparison = "greater than";
                    break;
                case SampleBlackboardFunctionsIntegerComparisonType.GreaterThanEqualTo:
                    comparison = "greater than or equal to";
                    break;
                case SampleBlackboardFunctionsIntegerComparisonType.LesserThan:
                    comparison = "lesser than";
                    break;
                case SampleBlackboardFunctionsIntegerComparisonType.LesserThanEqualTo:
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
        internal static float FloatComparisonScoreCalculator(float currentScore, bool invert, ref float key, float secondValue, float equalityTolerance, SampleBlackboardFunctionsFloatComparisonType comparisonType, float score)
        {
            return currentScore + (SampleDecoratorBlackboardFunctions.FloatComparisonDecorator(invert, ref key, secondValue, equalityTolerance, comparisonType) ? score : 0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void FloatComparisonScoreCalculatorUpdateDescription(bool invert, UnityEngine.AI.BlackboardTemplate.KeySelector key, float secondValue, float equalityTolerance, SampleBlackboardFunctionsFloatComparisonType comparisonType, float score, out string staticDescription)
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
                case SampleBlackboardFunctionsFloatComparisonType.AlmostEquals:
                    comparison = "equal to";
                    if (equalityTolerance != 0f)
                    {
                        valueStr = $"{valueStr} ± {equalityTolerance}";
                    }
                    break;
                case SampleBlackboardFunctionsFloatComparisonType.GreaterThan:
                    comparison = "greater than";
                    break;
                case SampleBlackboardFunctionsFloatComparisonType.GreaterThanEqualTo:
                    comparison = "greater than or equal to";
                    break;
                case SampleBlackboardFunctionsFloatComparisonType.LesserThan:
                    comparison = "lesser than";
                    break;
                case SampleBlackboardFunctionsFloatComparisonType.LesserThanEqualTo:
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
        internal static void ObjectEqualsScoreCalculatorUpdateDescription(bool invert, UnityEngine.AI.BlackboardTemplate.KeySelector key, Object value, float score, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"{ScoreString(score)} if {key.selectedKey.name} {(invert ? "is not" : "is")} set to {(value == null ? "null" : value.name)}.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("51db4674a1244ae18bfc087bb6c6b98e")]
        internal static float DynamicObjectEqualsScoreCalculator(float currentScore, bool invert, [HiraBotsObjectKey] ref int a, [HiraBotsObjectKey] ref int b, float score)
        {
            return currentScore + (SampleDecoratorBlackboardFunctions.DynamicObjectEqualsDecorator(invert, ref a, ref b) ? score : 0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void DynamicObjectEqualsScoreCalculatorUpdateDescription(bool invert, UnityEngine.AI.BlackboardTemplate.KeySelector a, UnityEngine.AI.BlackboardTemplate.KeySelector b, float score, out string staticDescription)
        {
            if (!a.selectedKey.isValid || !b.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"{(ScoreString(score))} if {a.selectedKey.name} {(invert ? "is not" : "is")} equal to {b.selectedKey.name}.";
        }
    }
}