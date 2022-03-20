using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

namespace HiraBots
{
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
        internal static void EnumEqualsDecoratorUpdateDescription(bool invert, UnityEngine.AI.BlackboardTemplate.KeySelector key, UnityEngine.DynamicEnum value, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"{key.selectedKey.name} {(invert ? "must not" : "must")} be equal to {value}.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("7c89e5910ec04893b329539512e8c1b7")]
        internal static bool DynamicEnumEqualsDecorator(bool invert, ref byte targetKey, ref byte operatorKey)
        {
            var result = targetKey == operatorKey;
            return invert != result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void DynamicEnumEqualsDecoratorUpdateDescription(bool invert, UnityEngine.AI.BlackboardTemplate.KeySelector targetKey, UnityEngine.AI.BlackboardTemplate.KeySelector operatorKey, out string staticDescription)
        {
            if (!targetKey.selectedKey.isValid || !operatorKey.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"{targetKey.selectedKey.name} {(invert ? "must not" : "must")} be equal to {operatorKey.selectedKey.name}.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("841fea4ba9b94948b9a5ff847474ea57")]
        internal static bool EnumHasFlagsDecorator(bool invert, ref byte key, [MatchTypeToEnumKey("key")] byte value)
        {
            var result = (key & value) != 0;
            return invert != result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void EnumHasFlagsDecoratorUpdateDescription(bool invert, UnityEngine.AI.BlackboardTemplate.KeySelector key, UnityEngine.DynamicEnum value, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"{key.selectedKey.name} {(invert ? "must not" : "must")} have these flags: {value}.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("b407a237055d4715a24a7c10c71d919c")]
        internal static bool DynamicEnumHasFlagsDecorator(bool invert, ref byte targetKey, ref byte operatorKey)
        {
            var result = (targetKey & operatorKey) != 0;
            return invert != result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void DynamicEnumHasFlagsDecoratorUpdateDescription(bool invert, UnityEngine.AI.BlackboardTemplate.KeySelector targetKey, UnityEngine.AI.BlackboardTemplate.KeySelector operatorKey, out string staticDescription)
        {
            if (!targetKey.selectedKey.isValid || !operatorKey.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"{targetKey.selectedKey.name} {(invert ? "must not" : "must")} have these flags: {operatorKey.selectedKey.name}.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("841fea4ba9b94948b9b5ff847474ea57")]
        internal static bool BooleanIsSetDecorator(bool invert, ref bool key)
        {
            var result = key;
            return invert != result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void BooleanIsSetDecoratorUpdateDescription(bool invert, UnityEngine.AI.BlackboardTemplate.KeySelector key, out string staticDescription)
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
        internal static void QuaternionIsSetDecoratorUpdateDescription(bool invert, UnityEngine.AI.BlackboardTemplate.KeySelector key, out string staticDescription)
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
        internal static void VectorIsSetDecoratorUpdateDescription(bool invert, UnityEngine.AI.BlackboardTemplate.KeySelector key, out string staticDescription)
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
        internal static bool IntegerComparisonDecorator(bool invert, ref int key, int secondValue, SampleBlackboardFunctionsIntegerComparisonType comparisonType)
        {
            bool result;
            switch (comparisonType)
            {
                case SampleBlackboardFunctionsIntegerComparisonType.Equals:
                    result = key == secondValue;
                    break;
                case SampleBlackboardFunctionsIntegerComparisonType.GreaterThan:
                    result = key > secondValue;
                    break;
                case SampleBlackboardFunctionsIntegerComparisonType.GreaterThanEqualTo:
                    result = key >= secondValue;
                    break;
                case SampleBlackboardFunctionsIntegerComparisonType.LesserThan:
                    result = key < secondValue;
                    break;
                case SampleBlackboardFunctionsIntegerComparisonType.LesserThanEqualTo:
                    result = key <= secondValue;
                    break;
                default:
                    result = false;
                    break;
            }

            return invert != result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void IntegerComparisonDecoratorUpdateDescription(bool invert, UnityEngine.AI.BlackboardTemplate.KeySelector key, int secondValue, SampleBlackboardFunctionsIntegerComparisonType comparisonType, out string staticDescription)
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

            staticDescription = $"{key.selectedKey.name} {(invert ? "must not" : "must")} be {comparison} {secondValue}.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("3b6d337fe0fa4b269f145171ef4871cd")]
        internal static bool FloatComparisonDecorator(bool invert, ref float key, float secondValue, float equalityTolerance, SampleBlackboardFunctionsFloatComparisonType comparisonType)
        {
            bool result;
            switch (comparisonType)
            {
                case SampleBlackboardFunctionsFloatComparisonType.AlmostEquals:
                    result = math.abs(key - secondValue) <= equalityTolerance;
                    break;
                case SampleBlackboardFunctionsFloatComparisonType.GreaterThan:
                    result = key > secondValue;
                    break;
                case SampleBlackboardFunctionsFloatComparisonType.GreaterThanEqualTo:
                    result = key >= secondValue;
                    break;
                case SampleBlackboardFunctionsFloatComparisonType.LesserThan:
                    result = key < secondValue;
                    break;
                case SampleBlackboardFunctionsFloatComparisonType.LesserThanEqualTo:
                    result = key <= secondValue;
                    break;
                default:
                    result = false;
                    break;
            }

            return invert != result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void FloatComparisonDecoratorUpdateDescription(bool invert, UnityEngine.AI.BlackboardTemplate.KeySelector key, float secondValue, float equalityTolerance, SampleBlackboardFunctionsFloatComparisonType comparisonType, out string staticDescription)
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
        internal static void ObjectEqualsDecoratorUpdateDescription(bool invert, UnityEngine.AI.BlackboardTemplate.KeySelector key, Object value, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"{key.selectedKey.name} {(invert ? "must not" : "must")} be set to {(value == null ? "null" : value.name)}.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("af98399ddc8f4698ad8fe4e5794427ed")]
        internal static bool DynamicObjectEqualsDecorator(bool invert, [HiraBotsObjectKey] ref int a, [HiraBotsObjectKey] ref int b)
        {
            var result = a == b;
            return invert != result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void DynamicObjectEqualsDecoratorUpdateDescription(bool invert, UnityEngine.AI.BlackboardTemplate.KeySelector a, UnityEngine.AI.BlackboardTemplate.KeySelector b, out string staticDescription)
        {
            if (!a.selectedKey.isValid || !b.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"{a.selectedKey.name} {(invert ? "must not" : "must")} be equal to {b.selectedKey.name}.";
        }
    }
}