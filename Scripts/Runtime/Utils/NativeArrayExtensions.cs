using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace HiraBots
{
    /// <summary>
    /// A container for utility methods for NativeArrays.
    /// </summary>
    internal static unsafe class NativeArrayExtensions
    {
        /// <summary>
        /// Reallocate a native array to a larger (or smaller size).
        /// The original array will be disposed. That's kind of what "reallocate" means, you know.
        /// </summary>
        internal static void Reallocate<T>(this ref NativeArray<T> array,
            int newSize,
            Allocator allocator,
            NativeArrayOptions options = NativeArrayOptions.ClearMemory)
            where T : unmanaged
        {
            var newArray = new NativeArray<T>(newSize, allocator, options);

            var copySize = math.min(array.Length * sizeof(T), newSize * sizeof(T));

            UnsafeUtility.MemCpy(newArray.GetUnsafePtr(), array.GetUnsafeReadOnlyPtr(), copySize);

            array.Dispose();
            array = newArray;
        }
    }
}