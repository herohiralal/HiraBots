using Unity.Collections;

namespace HiraBots
{
    /// <summary>
    /// A container for utility methods for NativeArrays.
    /// </summary>
    internal static class NativeArrayExtensions
    {
        /// <summary>
        /// Reallocate a native array to a larger (or smaller size).
        /// The original array will be disposed. That's kind of what "reallocate" means, you know.
        /// </summary>
        internal static void Reallocate<T>(this ref NativeArray<T> array,
            int newSize,
            Allocator allocator,
            NativeArrayOptions options = NativeArrayOptions.ClearMemory)
            where T : struct
        {
            var newArray = new NativeArray<T>(newSize, allocator, options);

            NativeArray<T>.Copy(array, newArray, array.Length);

            array.Dispose();
            array = newArray;
        }
    }
}