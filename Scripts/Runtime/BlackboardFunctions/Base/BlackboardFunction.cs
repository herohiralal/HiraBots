using System;
using Unity.Burst;
using UnityEngine;

namespace HiraBots
{
    internal abstract unsafe class BlackboardFunction : ScriptableObject, ILowLevelObjectProvider
    {
        /// <summary>
        /// The aligned memory size required by this function.
        /// </summary>
        public int GetAlignedMemorySize() => m_CachedAlignedMemorySize;

        // cached memory size
        private int m_CachedAlignedMemorySize = 0;

        /// <summary>
        /// Prepare the object for compilation, such as caching variables.
        /// </summary>
        internal virtual void PrepareForCompilation()
        {
            m_CachedAlignedMemorySize = UnsafeHelpers.GetAlignedSize(memorySize);
        }

        /// <summary>
        /// The memory size required by the function.
        /// </summary>
        protected virtual int memorySize => ByteStreamHelpers.CombinedSizes<int, IntPtr>(); // header includes size and function-pointer

        /// <summary>
        /// Append the memory to the stream.
        /// </summary>
        public abstract void Compile(ref byte* stream);
    }

    /// <summary>
    /// A function that can be invoked on a <see cref="LowLevelBlackboard"/>.
    /// =============================================================================================
    /// Any changes to this class MUST be synchronized with <see cref="LowLevelBlackboardFunction"/>.
    /// =============================================================================================
    /// </summary>
    internal abstract unsafe partial class BlackboardFunction<TFunction> : BlackboardFunction
        where TFunction : Delegate
    {
        /// <summary>
        /// Append the memory to the stream.
        /// </summary>
        public override void Compile(ref byte* stream)
        {
            // no offset
            ByteStreamHelpers.Write<int>(ref stream, GetAlignedMemorySize());

            // offset sizeof(int)
            ByteStreamHelpers.Write<IntPtr>(ref stream, function.Value);

            // offset sizeof(int) + sizeof(IntPtr)
        }

        /// <summary>
        /// The function-pointer to execute.
        /// </summary>
        protected abstract FunctionPointer<TFunction> function { get; }
    }
}