using System.Runtime.CompilerServices;

namespace HiraBots
{
    internal static unsafe class ByteStreamHelpers
    {
        #region Combine Sizes

        /// <summary>
        /// Add the size of zero types.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int CombinedSizes()
        {
            return 0;
        }

        /// <summary>
        /// Add the size of one type.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int CombinedSizes<TType1>()
            where TType1 : unmanaged
        {
            return sizeof(TType1);
        }

        /// <summary>
        /// Add the size of two types.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int CombinedSizes<TType1, TType2>()
            where TType1 : unmanaged
            where TType2 : unmanaged
        {
            return sizeof(TType1) + sizeof(TType2);
        }

        /// <summary>
        /// Add the size of three types.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int CombinedSizes<TType1, TType2, TType3>()
            where TType1 : unmanaged
            where TType2 : unmanaged
            where TType3 : unmanaged
        {
            return sizeof(TType1) + sizeof(TType2) + sizeof(TType3);
        }

        /// <summary>
        /// Add the size of four types.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int CombinedSizes<TType1, TType2, TType3, TType4>()
            where TType1 : unmanaged
            where TType2 : unmanaged
            where TType3 : unmanaged
            where TType4 : unmanaged
        {
            return sizeof(TType1) + sizeof(TType2) + sizeof(TType3) + sizeof(TType4);
        }

        /// <summary>
        /// Add the size of five types.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int CombinedSizes<TType1, TType2, TType3, TType4, TType5>()
            where TType1 : unmanaged
            where TType2 : unmanaged
            where TType3 : unmanaged
            where TType4 : unmanaged
            where TType5 : unmanaged
        {
            return sizeof(TType1) + sizeof(TType2) + sizeof(TType3) + sizeof(TType4) + sizeof(TType5);
        }

        #endregion

        #region Jump Over

        /// <summary>
        /// Jump over zero types.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static byte* JumpOver(byte* stream)
        {
            return stream + CombinedSizes();
        }

        /// <summary>
        /// Jump over one type.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static byte* JumpOver<TType1>(byte* stream)
            where TType1 : unmanaged
        {
            return stream + CombinedSizes<TType1>();
        }

        /// <summary>
        /// Jump over two types.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static byte* JumpOver<TType1, TType2>(byte* stream)
            where TType1 : unmanaged
            where TType2 : unmanaged
        {
            return stream + CombinedSizes<TType1, TType2>();
        }

        /// <summary>
        /// Jump over three types.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static byte* JumpOver<TType1, TType2, TType3>(byte* stream)
            where TType1 : unmanaged
            where TType2 : unmanaged
            where TType3 : unmanaged
        {
            return stream + CombinedSizes<TType1, TType2, TType3>();
        }

        /// <summary>
        /// Jump over four types.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static byte* JumpOver<TType1, TType2, TType3, TType4>(byte* stream)
            where TType1 : unmanaged
            where TType2 : unmanaged
            where TType3 : unmanaged
            where TType4 : unmanaged
        {
            return stream + CombinedSizes<TType1, TType2, TType3, TType4>();
        }

        /// <summary>
        /// Jump over five types.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static byte* JumpOver<TType1, TType2, TType3, TType4, TType5>(byte* stream)
            where TType1 : unmanaged
            where TType2 : unmanaged
            where TType3 : unmanaged
            where TType4 : unmanaged
            where TType5 : unmanaged
        {
            return stream + CombinedSizes<TType1, TType2, TType3, TType4, TType5>();
        }

        #endregion

        #region Read

        /// <summary>
        /// Read a value from the stream without any skips.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ref TTarget Read<TTarget>(byte* stream)
            where TTarget : unmanaged
        {
            return ref *(TTarget*) JumpOver(stream);
        }

        /// <summary>
        /// Read a value from the stream, skipping over one type of item.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ref TTarget Read<TSkip1, TTarget>(byte* stream)
            where TSkip1 : unmanaged
            where TTarget : unmanaged
        {
            return ref *(TTarget*) JumpOver<TSkip1>(stream);
        }

        /// <summary>
        /// Read a value from the stream, skipping over two types of items.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ref TTarget Read<TSkip1, TSkip2, TTarget>(byte* stream)
            where TSkip1 : unmanaged
            where TSkip2 : unmanaged
            where TTarget : unmanaged
        {
            return ref *(TTarget*) JumpOver<TSkip1, TSkip2>(stream);
        }

        /// <summary>
        /// Read a value from the stream, skipping over three types of items.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ref TTarget Read<TSkip1, TSkip2, TSkip3, TTarget>(byte* stream)
            where TSkip1 : unmanaged
            where TSkip2 : unmanaged
            where TSkip3 : unmanaged
            where TTarget : unmanaged
        {
            return ref *(TTarget*) JumpOver<TSkip1, TSkip2, TSkip3>(stream);
        }

        /// <summary>
        /// Read a value from the stream, skipping over four types of items.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ref TTarget Read<TSkip1, TSkip2, TSkip3, TSkip4, TTarget>(byte* stream)
            where TSkip1 : unmanaged
            where TSkip2 : unmanaged
            where TSkip3 : unmanaged
            where TSkip4 : unmanaged
            where TTarget : unmanaged
        {
            return ref *(TTarget*) JumpOver<TSkip1, TSkip2, TSkip3, TSkip4>(stream);
        }

        /// <summary>
        /// Read a value from the stream, skipping over five types of items.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ref TTarget Read<TSkip1, TSkip2, TSkip3, TSkip4, TSkip5, TTarget>(byte* stream)
            where TSkip1 : unmanaged
            where TSkip2 : unmanaged
            where TSkip3 : unmanaged
            where TSkip4 : unmanaged
            where TSkip5 : unmanaged
            where TTarget : unmanaged
        {
            return ref *(TTarget*) JumpOver<TSkip1, TSkip2, TSkip3, TSkip4, TSkip5>(stream);
        }

        #endregion

        #region Write

        /// <summary>
        /// Write a value to the stream and move it forward.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Write<T>(ref byte* stream, T value) where T : unmanaged
        {
            Read<T>(stream) = value;
            stream = JumpOver<T>(stream);
        }

        #endregion
    }
}