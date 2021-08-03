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
        /// Convert a boolean to a byte.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static bool ToBoolean(this byte b) => b != 0;

        /// <summary>
        /// Convert a byte to a boolean.
        /// </summary>
        [MethodImpl(k_Inline)]
        internal static byte ToByte(this bool b) => (byte) (b ? 1 : 0);
    }
}