using System.Runtime.CompilerServices;
using UnityEngine;

namespace HiraBots
{
    internal readonly unsafe struct DefaultLowLevelObjectCollection<TElement, TConverter>
        : ILowLevelObjectCollection<TElement, DefaultLowLevelObjectCollection<TElement, TConverter>.Enumerator>
        where TElement : ILowLevelObject
        where TConverter : IPointerToLowLevelObjectConverter<TElement>, new()
    {
        private readonly byte* m_Address;

        public byte* address
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Address;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal DefaultLowLevelObjectCollection(byte* address)
        {
            m_Address = address;
        }

        internal readonly struct PointerConverter : IPointerToLowLevelObjectConverter<DefaultLowLevelObjectCollection<TElement, TConverter>>
        {
            public DefaultLowLevelObjectCollection<TElement, TConverter> Convert(byte* address)
            {
                return new DefaultLowLevelObjectCollection<TElement, TConverter>(address);
            }
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

                m_Current += new TConverter().Convert(m_Current).size;
                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public byte* GetCurrentElementLowLevel()
            {
                return m_Current;
            }

            public int currentIndex
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => m_CurrentIndex;
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
        private readonly ReadOnlyArrayAccessor<TProvider> m_Providers;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal DefaultLowLevelObjectProviderCollection(ReadOnlyArrayAccessor<TProvider> providers)
        {
            m_Providers = providers;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetAlignedMemorySize()
        {
            var size = ByteStreamHelpers.CombinedSizes<int, int>(); // size & count header

            foreach (var provider in m_Providers)
            {
                size += provider.GetAlignedMemorySize();
            }

            return UnsafeHelpers.GetAlignedSize(size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte* Compile(byte* stream)
        {
            ByteStreamHelpers.Write<int>(ref stream, GetAlignedMemorySize()); // size header
            ByteStreamHelpers.Write<int>(ref stream, m_Providers.count); // count header

            foreach (var provider in m_Providers)
            {
                stream = provider.Compile(stream);
            }

            return stream;
        }
    }

    internal static unsafe class DefaultLowLevelObjectProviderCollectionHelpers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetAlignedMemorySize<T>(this T[] providers)
            where T : ILowLevelObjectProvider
        {
            return new DefaultLowLevelObjectProviderCollection<T>(providers).GetAlignedMemorySize();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static byte* Compile<T>(this T[] providers, byte* stream)
            where T : ILowLevelObjectProvider
        {
            return new DefaultLowLevelObjectProviderCollection<T>(providers).Compile(stream);
        }
    }
}