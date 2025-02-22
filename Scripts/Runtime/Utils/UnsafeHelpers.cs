using System.Runtime.CompilerServices;

namespace HiraBots
{
    /// <summary>
    /// Helpers for unsafe code.
    /// </summary>
    internal static class UnsafeHelpers
    {
        private const MethodImplOptions k_Inline = MethodImplOptions.AggressiveInlining;

        /// <summary>
        /// Get aligned size to allocate.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static int GetAlignedSize(int size)
        {
            // round to 4 bytes
            return (size + 3) & ~3;
        }
    }
}