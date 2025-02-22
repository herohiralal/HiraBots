using System.Runtime.CompilerServices;

// All interfaces in this file are meant to only be used with generics.
// they're not meant to directly reference any objects.
namespace HiraBots
{
    /// <summary>
    /// Ensures that the object is a low-level representation of something.
    /// </summary>
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

    /// <summary>
    /// Ensures that the object provides a low-level representation.
    /// </summary>
    internal unsafe interface ILowLevelObjectProvider
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int GetMemorySizeRequiredForCompilation();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Compile(ref byte* stream);
    }

    /// <summary>
    /// Allows converting a pointer to a low-level object.
    /// </summary>
    internal unsafe interface IPointerToLowLevelObjectConverter<out T>
        where T : ILowLevelObject
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        T Convert(byte* address);
    }

    /// <summary>
    /// Ensures that the object is a low-level collection.
    /// </summary>
    internal unsafe interface ILowLevelObjectCollection : ILowLevelObject
    {
        /// <summary>
        /// The number of objects in the collection.
        /// </summary>
        int count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        /// <summary>
        /// Get the address of the element at index index.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        byte* GetElementAtIndexLowLevel(int index);
    }

    /// <summary>
    /// Ensures that the object is a low-level collection of a specific type.
    /// </summary>
    internal interface ILowLevelObjectCollection<out TElement> : ILowLevelObjectCollection
        where TElement : ILowLevelObject
    {
        /// <summary>
        /// Access the element at a specific index.
        /// </summary>
        TElement this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }

    /// <summary>
    /// Ensures that the object is a low-level collection of a specific type and can be iterated over.
    /// </summary>
    internal interface ILowLevelObjectCollection<out TElement, out TEnumerator> : ILowLevelObjectCollection<TElement>
        where TElement : ILowLevelObject
        where TEnumerator : ILowLevelEnumerator<TElement>
    {
        /// <summary>
        /// Get the iterator.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        TEnumerator GetEnumerator();
    }

    /// <summary>
    /// Ensures that the object is a low-level enumerator.
    /// </summary>
    internal unsafe interface ILowLevelEnumerator
    {
        /// <summary>
        /// Check whether there's a next element.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool MoveNext();

        /// <summary>
        /// Get the address to the current element.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        byte* GetCurrentElementLowLevel();

        /// <summary>
        /// Get the current index.
        /// </summary>
        int currentIndex
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }

    /// <summary>
    /// Ensures that the object is a low-level enumerator of a specific type.
    /// </summary>
    internal interface ILowLevelEnumerator<out TElement> : ILowLevelEnumerator
        where TElement : ILowLevelObject
    {
        /// <summary>
        /// Get the current element.
        /// </summary>
        TElement current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }
}