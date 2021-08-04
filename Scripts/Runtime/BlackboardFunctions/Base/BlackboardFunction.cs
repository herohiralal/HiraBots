﻿using System;
using Unity.Burst;
using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// A function that can be invoked on a <see cref="LowLevelBlackboard"/>.
    /// =============================================================================================
    /// Any changes to this class MUST be synchronized with <see cref="LowLevelBlackboardFunction"/>.
    /// =============================================================================================
    /// </summary>
    internal abstract unsafe class BlackboardFunction<TFunction> : ScriptableObject
        where TFunction : Delegate
    {
        /// <summary>
        /// The aligned memory size required by this function.
        /// </summary>
        internal int GetAlignedMemorySize() => UnsafeHelpers.GetAlignedSize(memorySize);
        
        /// <summary>
        /// The memory size required by the function.
        /// </summary>
        protected virtual int memorySize => ByteStreamHelpers.CombinedSizes<int, IntPtr>(); // header includes size and function-pointer

        /// <summary>
        /// Append the memory to the stream.
        /// </summary>
        internal virtual byte* AppendMemory(byte* stream)
        {
            // no offset
            ByteStreamHelpers.Write<int>(ref stream, GetAlignedMemorySize());

            // offset sizeof(int)
            ByteStreamHelpers.Write<IntPtr>(ref stream, function.Value);

            // offset sizeof(int) + sizeof(IntPtr)
            return stream;
        }

        /// <summary>
        /// The function-pointer 
        /// </summary>
        protected abstract FunctionPointer<TFunction> function { get; }
    }

    /// <summary>
    /// Helper functions to aid with collections of blackboard functions.
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
        internal static byte* AppendMemory<T>(this BlackboardFunction<T>[] functions, byte* stream) where T : Delegate
        {
            ByteStreamHelpers.Write<int>(ref stream, functions.Length); // count header

            foreach (var function in functions)
            {
                stream = function.AppendMemory(stream);
            }

            return stream;
        }
    }
}