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
    internal abstract unsafe partial class BlackboardFunction<TFunction> : ScriptableObject, ILowLevelObjectProvider
        where TFunction : Delegate
    {
        /// <summary>
        /// All function pointers must be compiled within OnEnable().
        /// </summary>
        protected virtual void OnEnable()
        {
        }

        /// <summary>
        /// The aligned memory size required by this function.
        /// </summary>
        public int GetAlignedMemorySize() => UnsafeHelpers.GetAlignedSize(memorySize);

        /// <summary>
        /// The memory size required by the function.
        /// </summary>
        protected virtual int memorySize => ByteStreamHelpers.CombinedSizes<int, IntPtr>(); // header includes size and function-pointer

        /// <summary>
        /// Append the memory to the stream.
        /// </summary>
        public virtual byte* Compile(byte* stream)
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
}