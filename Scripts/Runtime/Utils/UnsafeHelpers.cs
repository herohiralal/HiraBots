using System.Runtime.CompilerServices;

namespace HiraBots
{
    internal static class UnsafeHelpers
    {
        private const MethodImplOptions inline = MethodImplOptions.AggressiveInlining;

        [MethodImpl(inline)]
        internal static bool ToBoolean(this byte b) => b != 0;

        [MethodImpl(inline)]
        internal static byte ToByte(this bool b) => (byte) (b ? 1 : 0);
    }
}