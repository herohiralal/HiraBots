using System.Runtime.CompilerServices;

// All interfaces in this file are meant to only be used with generics.
// they're not meant to directly reference any objects.
namespace HiraBots
{
    internal unsafe interface ILowLevelObject
    {
        byte* address
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        int size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }

    internal unsafe interface ILowLevelObjectProvider
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int GetMemorySizeRequiredForCompilation();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Compile(ref byte* stream);
    }

    internal unsafe interface IPointerToLowLevelObjectConverter<out T>
        where T : ILowLevelObject
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        T Convert(byte* address);
    }

    internal unsafe interface ILowLevelObjectCollection : ILowLevelObject
    {
        int count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        byte* GetElementAtIndexLowLevel(int index);
    }

    internal interface ILowLevelObjectCollection<out TElement> : ILowLevelObjectCollection
        where TElement : ILowLevelObject
    {
        TElement this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }

    internal interface ILowLevelObjectCollection<out TElement, out TEnumerator> : ILowLevelObjectCollection<TElement>
        where TElement : ILowLevelObject
        where TEnumerator : ILowLevelEnumerator<TElement>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        TEnumerator GetEnumerator();
    }

    internal unsafe interface ILowLevelEnumerator
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool MoveNext();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        byte* GetCurrentElementLowLevel();

        int currentIndex
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }

    internal interface ILowLevelEnumerator<out TElement> : ILowLevelEnumerator
        where TElement : ILowLevelObject
    {
        TElement current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }
}