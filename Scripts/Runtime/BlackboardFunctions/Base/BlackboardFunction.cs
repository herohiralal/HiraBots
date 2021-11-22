using System;
using Unity.Burst;
using UnityEngine;

namespace HiraBots
{
    // this base class only exists to not have to specify a generic one everywhere.
    // it contains all the non-function-specific interface
    internal abstract unsafe partial class BlackboardFunction : ScriptableObject, ILowLevelObjectProvider
    {
        /// <summary>
        /// The aligned memory size required by this function.
        /// </summary>
        public int GetMemorySizeRequiredForCompilation() => m_MemorySize;

        // cached memory size
        protected int m_MemorySize = 0;

        /// <summary>
        /// Prepare the object for compilation, such as caching variables.
        /// </summary>
        internal abstract void PrepareForCompilation();

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
        internal override void PrepareForCompilation()
        {
            m_MemorySize = ByteStreamHelpers.CombinedSizes<int, IntPtr>(); // size & function pointer header
        }

        public override void Compile(ref byte* stream)
        {
            // no offset
            ByteStreamHelpers.Write<int>(ref stream, GetMemorySizeRequiredForCompilation());

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