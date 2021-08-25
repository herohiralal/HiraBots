﻿using System.Runtime.CompilerServices;

namespace HiraBots
{
    internal readonly unsafe struct DefaultLowLevelObjectCollection<TElement, TConverter>
        : ILowLevelObjectCollection<TElement, DefaultLowLevelObjectCollection<TElement, TConverter>.Enumerator>
        where TElement : unmanaged, ILowLevelObject
        where TConverter : unmanaged, IPointerToLowLevelObjectConverter<TElement>
    {
        private readonly byte* m_Address;
        public byte* address => m_Address;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DefaultLowLevelObjectCollection(byte* address)
        {
            m_Address = address;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DefaultLowLevelObjectCollection<TElement, TConverter>(byte* stream)
        {
            return new DefaultLowLevelObjectCollection<TElement, TConverter>(stream);
        }

        // no offset
        public int size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ByteStreamHelpers.JumpOverNothing(m_Address).AndAccess<int>();
        }

        // offset size
        public int count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ByteStreamHelpers.JumpOver<int>(m_Address).AndAccess<int>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte* GetElementAtIndexLowLevel(int index)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (index >= count)
            {
                throw new System.IndexOutOfRangeException($"Index out of range[0-{count}) - (index) {index}.");
            }
#endif

            var enumerator = GetEnumerator();
            enumerator.MoveNext();

            for (var i = 0; i < index; i++)
            {
                enumerator.MoveNext();
            }

            return enumerator.GetCurrentElementLowLevel();
        }

        public TElement this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new TConverter().Convert(GetElementAtIndexLowLevel(index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator()
        {
            return new Enumerator(ByteStreamHelpers.JumpOver<int, int>(m_Address).AndGetAPointerOf<byte>(), count);
        }

        internal struct Enumerator : ILowLevelEnumerator<TElement>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(byte* current, int count)
            {
                m_Current = current;
                m_CurrentIndex = -1;
                m_Count = count;
            }

            private byte* m_Current;
            private int m_CurrentIndex;
            private readonly int m_Count;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                if (++m_CurrentIndex >= m_Count)
                {
                    return false;
                }

                m_Current += ((LowLevelBlackboardFunction) m_Current).size;
                return true;
            }

            public byte* GetCurrentElementLowLevel()
            {
                return m_Current;
            }

            public TElement current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => new TConverter().Convert(m_Current);
            }
        }
    }

    internal readonly unsafe struct DefaultLowLevelObjectProviderCollection<TProvider> : ILowLevelObjectProvider
        where TProvider : ILowLevelObjectProvider
    {
        private readonly TProvider[] m_Providers;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DefaultLowLevelObjectProviderCollection(TProvider[] providers)
        {
            m_Providers = providers;
        }

        public static implicit operator DefaultLowLevelObjectProviderCollection<TProvider>(TProvider[] providers)
        {
            return new DefaultLowLevelObjectProviderCollection<TProvider>(providers);
        }

        public int GetAlignedMemorySize()
        {
            var size = ByteStreamHelpers.CombinedSizes<int, int>(); // size & count header

            foreach (var provider in m_Providers)
            {
                size += provider.GetAlignedMemorySize();
            }

            return UnsafeHelpers.GetAlignedSize(size);
        }

        public byte* WriteLowLevelObjectAndJumpPast(byte* stream)
        {
            ByteStreamHelpers.Write<int>(ref stream, GetAlignedMemorySize()); // size header
            ByteStreamHelpers.Write<int>(ref stream, m_Providers.Length); // count header

            foreach (var provider in m_Providers)
            {
                stream = provider.WriteLowLevelObjectAndJumpPast(stream);
            }

            return stream;
        }
    }
}