using System;
using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// Helper functions to aid with a collection of blackboard functions.
    /// =======================================================================================================
    /// Any changes to this class MUST be synchronized with <see cref="LowLevelBlackboardFunctionCollection"/>.
    /// =======================================================================================================
    /// </summary>
    internal static unsafe class BlackboardFunctionExtensions
    {
        /// <summary>
        /// Get aligned memory size for a collection of blackboard functions.
        /// </summary>
        internal static int GetAlignedMemorySize<T>(this BlackboardFunction<T>[] functions) where T : Delegate
        {
            var size = ByteStreamHelpers.CombinedSizes<int>(); // count header

            foreach (var function in functions)
            {
                size += function.GetAlignedMemorySize();
            }

            return UnsafeHelpers.GetAlignedSize(size);
        }

        /// <summary>
        /// Append the memory to the stream.
        /// </summary>
        internal static byte* Compile<T>(this BlackboardFunction<T>[] functions, byte* stream) where T : Delegate
        {
            ByteStreamHelpers.Write<int>(ref stream, functions.Length); // count header

            foreach (var function in functions)
            {
                stream = function.Compile(stream);
            }

            return stream;
        }
    }

    /// <summary>
    /// Helper functions to aid with a collection of decorators.
    /// ================================================================================================================
    /// Any changes to this class MUST be synchronized with <see cref="LowLevelDecoratorBlackboardFunctionCollection"/>.
    /// ================================================================================================================
    /// </summary>
    internal static unsafe class DecoratorBlackboardFunctionExtensions
    {
        /// <summary>
        /// Get aligned memory size for a collection of decorators.
        /// </summary>
        internal static int GetAlignedMemorySize(this DecoratorBlackboardFunction[] functions)
        {
            // ReSharper disable once CoVariantArrayConversion
            return BlackboardFunctionExtensions.GetAlignedMemorySize(functions);
        }

        /// <summary>
        /// Append the memory to the stream.
        /// </summary>
        internal static byte* Compile(this DecoratorBlackboardFunction[] functions, byte* stream)
        {
            // ReSharper disable once CoVariantArrayConversion
            return BlackboardFunctionExtensions.Compile(functions, stream);
        }
    }

    /// <summary>
    /// Helper functions to aid with a collection of effectors.
    /// ===============================================================================================================
    /// Any changes to this class MUST be synchronized with <see cref="LowLevelEffectorBlackboardFunctionCollection"/>.
    /// ===============================================================================================================
    /// </summary>
    internal static unsafe class EffectorBlackboardFunctionExtensions
    {
        /// <summary>
        /// Get aligned memory size for a collection of effectors.
        /// </summary>
        internal static int GetAlignedMemorySize(this EffectorBlackboardFunction[] functions)
        {
            // ReSharper disable once CoVariantArrayConversion
            return BlackboardFunctionExtensions.GetAlignedMemorySize(functions);
        }

        /// <summary>
        /// Append the memory to the stream.
        /// </summary>
        internal static byte* Compile(this EffectorBlackboardFunction[] functions, byte* stream)
        {
            // ReSharper disable once CoVariantArrayConversion
            return BlackboardFunctionExtensions.Compile(functions, stream);
        }
    }

    /// <summary>
    /// Helper functions to aid with a collection of score calculators.
    /// ======================================================================================================================
    /// Any changes to this class MUST be synchronized with <see cref="LowLevelScoreCalculatorBlackboardFunctionCollection"/>.
    /// ======================================================================================================================
    /// </summary>
    internal static unsafe class ScoreCalculatorBlackboardFunctionExtensions
    {
        /// <summary>
        /// Get aligned memory size for a collection of score calculators.
        /// </summary>
        internal static int GetAlignedMemorySize(this ScoreCalculatorBlackboardFunction[] functions)
        {
            // ReSharper disable once CoVariantArrayConversion
            return BlackboardFunctionExtensions.GetAlignedMemorySize(functions);
        }

        /// <summary>
        /// Append the memory to the stream.
        /// </summary>
        internal static byte* Compile(this ScoreCalculatorBlackboardFunction[] functions, byte* stream)
        {
            // ReSharper disable once CoVariantArrayConversion
            return BlackboardFunctionExtensions.Compile(functions, stream);
        }
    }
}