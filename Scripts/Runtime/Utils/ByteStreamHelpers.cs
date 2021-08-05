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
        internal static int NoCombinedSizes()
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
        internal static JumpedPointer JumpOverNothing(byte* stream)
        {
            return stream + NoCombinedSizes();
        }

        /// <summary>
        /// Jump over one type.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JumpedPointer JumpOver<TType1>(byte* stream)
            where TType1 : unmanaged
        {
            return stream + CombinedSizes<TType1>();
        }

        /// <summary>
        /// Jump over two types.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JumpedPointer JumpOver<TType1, TType2>(byte* stream)
            where TType1 : unmanaged
            where TType2 : unmanaged
        {
            return stream + CombinedSizes<TType1, TType2>();
        }

        /// <summary>
        /// Jump over three types.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JumpedPointer JumpOver<TType1, TType2, TType3>(byte* stream)
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
        internal static JumpedPointer JumpOver<TType1, TType2, TType3, TType4>(byte* stream)
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
        internal static JumpedPointer JumpOver<TType1, TType2, TType3, TType4, TType5>(byte* stream)
            where TType1 : unmanaged
            where TType2 : unmanaged
            where TType3 : unmanaged
            where TType4 : unmanaged
            where TType5 : unmanaged
        {
            return stream + CombinedSizes<TType1, TType2, TType3, TType4, TType5>();
        }

        /// <summary>
        /// Intermediate structure for JumpOver functions, to improve readabilitiy.
        /// </summary>
        internal readonly struct JumpedPointer
        {
            private readonly void* m_Stream;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private JumpedPointer(void* stream)
            {
                m_Stream = stream;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator JumpedPointer(void* stream)
            {
                return new JumpedPointer(stream);
            }

            /// <summary>
            /// Cast the jumped pointer to a specific type.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal ref T AndAccess<T>() where T : unmanaged
            {
                return ref *AndGetAPointerOf<T>();
            }

            /// <summary>
            /// Cast the jumped pointer to a specific type.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal T* AndGetAPointerOf<T>() where T : unmanaged
            {
                return (T*) m_Stream;
            }
        }

        #endregion

        #region Write

        /// <summary>
        /// Write a value to the stream and move it forward.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Write<T>(ref byte* stream, T value) where T : unmanaged
        {
            JumpOverNothing(stream).AndAccess<T>() = value;
            stream = JumpOver<T>(stream).AndGetAPointerOf<byte>();
        }

        #endregion
    }
}