using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

namespace HiraBots
{
    internal static class SampleEffectorBlackboardFunctions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("ad6818e4de9846adba80edd01785671e")]
        internal static void EnumOperatorEffector(ref byte key, SampleBlackboardFunctionsEnumOperationType operationType, [MatchTypeToEnumKey("key")] byte value)
        {
            switch (operationType)
            {
                case SampleBlackboardFunctionsEnumOperationType.Set: key = value;
                    break;
                case SampleBlackboardFunctionsEnumOperationType.AddFlags: key |= value;
                    break;
                case SampleBlackboardFunctionsEnumOperationType.RemoveFlags: key = (byte) (key & ~value);
                    break;
                default:
                    return;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void EnumOperatorEffectorUpdateDescription(UnityEngine.AI.BlackboardTemplate.KeySelector key, SampleBlackboardFunctionsEnumOperationType operationType, UnityEngine.DynamicEnum value, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            switch (operationType)
            {
                case SampleBlackboardFunctionsEnumOperationType.Set:
                    staticDescription = $"Set {key.selectedKey.name} to this value.";
                    break;
                case SampleBlackboardFunctionsEnumOperationType.AddFlags:
                    staticDescription = $"Add these flags to {key.selectedKey.name}.";
                    break;
                case SampleBlackboardFunctionsEnumOperationType.RemoveFlags:
                    staticDescription = $"Remove these flags from {key.selectedKey.name}.";
                    break;
                default:
                    staticDescription = "";
                    return;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("89af0e100fac43f1ba83126b2d7c87de")]
        internal static void FloatOperatorEffector(ref float key, SampleBlackboardFunctionsFloatOperationType operationType, float value)
        {
            switch (operationType)
            {
                case SampleBlackboardFunctionsFloatOperationType.Set:
                    key = value;
                    break;
                case SampleBlackboardFunctionsFloatOperationType.Add:
                    key += value;
                    break;
                case SampleBlackboardFunctionsFloatOperationType.Subtract:
                    key -= value;
                    break;
                case SampleBlackboardFunctionsFloatOperationType.Multiply:
                    key *= value;
                    break;
                case SampleBlackboardFunctionsFloatOperationType.Divide:
                    key /= value;
                    break;
                default:
                    return;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void FloatOperatorEffectorOnValidate(ref UnityEngine.AI.BlackboardTemplate.KeySelector key, ref SampleBlackboardFunctionsFloatOperationType operationType, ref float value)
        {
            if (operationType == SampleBlackboardFunctionsFloatOperationType.Divide && value == 0f)
            {
                value = 1f;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void FloatOperatorEffectorUpdateDescription(UnityEngine.AI.BlackboardTemplate.KeySelector key, SampleBlackboardFunctionsFloatOperationType operationType, float value, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            switch (operationType)
            {
                case SampleBlackboardFunctionsFloatOperationType.Set:
                    staticDescription = $"Set {key.selectedKey.name} to {value}.";
                    break;
                case SampleBlackboardFunctionsFloatOperationType.Add:
                    staticDescription = $"Add {value} to {key.selectedKey.name}.";
                    break;
                case SampleBlackboardFunctionsFloatOperationType.Subtract:
                    staticDescription = $"Subtract {value} from {key.selectedKey.name}.";
                    break;
                case SampleBlackboardFunctionsFloatOperationType.Multiply:
                    staticDescription = $"Multiply {key.selectedKey.name} by {value}.";
                    break;
                case SampleBlackboardFunctionsFloatOperationType.Divide:
                    staticDescription = $"Divide {key.selectedKey.name} by {value}.";
                    break;
                default:
                    staticDescription = "";
                    return;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("f073722b6c814acdb3f6485ca22b0f0e")]
        internal static void IntegerOperatorEffector(ref int key, SampleBlackboardFunctionsIntegerOperationType operationType, int value)
        {
            switch (operationType)
            {
                case SampleBlackboardFunctionsIntegerOperationType.Set:
                    key = value;
                    break;
                case SampleBlackboardFunctionsIntegerOperationType.Add:
                    key += value;
                    break;
                case SampleBlackboardFunctionsIntegerOperationType.Subtract:
                    key -= value;
                    break;
                case SampleBlackboardFunctionsIntegerOperationType.Multiply:
                    key *= value;
                    break;
                case SampleBlackboardFunctionsIntegerOperationType.Divide:
                    key /= value;
                    break;
                case SampleBlackboardFunctionsIntegerOperationType.BitwiseAnd:
                    key &= value;
                    break;
                case SampleBlackboardFunctionsIntegerOperationType.BitwiseOr:
                    key |= value;
                    break;
                case SampleBlackboardFunctionsIntegerOperationType.BitwiseXor:
                    key ^= value;
                    break;
                default:
                    return;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void IntegerOperatorEffectorOnValidate(ref UnityEngine.AI.BlackboardTemplate.KeySelector key, ref SampleBlackboardFunctionsIntegerOperationType operationType, ref int value)
        {
            if (operationType == SampleBlackboardFunctionsIntegerOperationType.Divide && value == 0)
            {
                value = 1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void IntegerOperatorEffectorUpdateDescription(UnityEngine.AI.BlackboardTemplate.KeySelector key, SampleBlackboardFunctionsIntegerOperationType operationType, int value, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            switch (operationType)
            {
                case SampleBlackboardFunctionsIntegerOperationType.Set:
                    staticDescription = $"Set {key.selectedKey.name} to {value}.";
                    break;
                case SampleBlackboardFunctionsIntegerOperationType.Add:
                    staticDescription = $"Add {value} to {key.selectedKey.name}.";
                    break;
                case SampleBlackboardFunctionsIntegerOperationType.Subtract:
                    staticDescription = $"Subtract {value} from {key.selectedKey.name}.";
                    break;
                case SampleBlackboardFunctionsIntegerOperationType.Multiply:
                    staticDescription = $"Multiply {key.selectedKey.name} by {value}.";
                    break;
                case SampleBlackboardFunctionsIntegerOperationType.Divide:
                    staticDescription = $"Divide {key.selectedKey.name} by {value}.";
                    break;
                case SampleBlackboardFunctionsIntegerOperationType.BitwiseAnd:
                    staticDescription = $"{key.selectedKey.name} &= {value}.";
                    break;
                case SampleBlackboardFunctionsIntegerOperationType.BitwiseOr:
                    staticDescription = $"{key.selectedKey.name} |= {value}.";
                    break;
                case SampleBlackboardFunctionsIntegerOperationType.BitwiseXor:
                    staticDescription = $"{key.selectedKey.name} ^= {value}.";
                    break;
                default:
                    staticDescription = "";
                    return;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("bf5cce8d3ca6458bbcf415de101c99e1")]
        internal static void BooleanIsSetEffector(ref bool key, SampleBlackboardFunctionsSetOperationType operationType)
        {
            switch (operationType)
            {
                case SampleBlackboardFunctionsSetOperationType.Set:
                    key = true;
                    break;
                case SampleBlackboardFunctionsSetOperationType.Unset:
                    key = false;
                    break;
                default:
                    return;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void BooleanIsSetEffectorUpdateDescription(UnityEngine.AI.BlackboardTemplate.KeySelector key, SampleBlackboardFunctionsSetOperationType operationType, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            switch (operationType)
            {
                case SampleBlackboardFunctionsSetOperationType.Set:
                    staticDescription = $"Set {key.selectedKey.name}.";
                    break;
                case SampleBlackboardFunctionsSetOperationType.Unset:
                    staticDescription = $"Unset {key.selectedKey.name}.";
                    break;
                default:
                    staticDescription = "";
                    return;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("106cabdfdcef44ae9638c0799151a03b")]
        internal static void BooleanInvertEffector(ref bool key)
        {
            key = !key;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void BooleanInvertEffectorUpdateDescription(UnityEngine.AI.BlackboardTemplate.KeySelector key, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"Invert {key.selectedKey.name}.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [GenerateInternalBlackboardFunction("da5818b29555439cb0a6adc4d0937280")]
        // ReSharper disable once RedundantAssignment
        internal static void ObjectEqualsEffector([HiraBotsObjectKey] ref int key, [HiraBotsObjectValue] int value)
        {
            key = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ObjectEqualsEffectorUpdateDescription(UnityEngine.AI.BlackboardTemplate.KeySelector key, Object value, out string staticDescription)
        {
            if (!key.selectedKey.isValid)
            {
                staticDescription = "";
                return;
            }

            staticDescription = $"Set {key.selectedKey.name} to {(value == null ? "null" : value.name)}.";
        }
    }
}