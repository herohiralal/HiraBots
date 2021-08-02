using System.Runtime.CompilerServices;

namespace HiraBots
{
    internal static class UnsafeHelpers
    {
        private const MethodImplOptions k_Inline = MethodImplOptions.AggressiveInlining;

        [MethodImpl(k_Inline)]
        internal static bool ToBoolean(this byte b) => b != 0;

        [MethodImpl(k_Inline)]
        internal static byte ToByte(this bool b) => (byte) (b ? 1 : 0);
    }
}