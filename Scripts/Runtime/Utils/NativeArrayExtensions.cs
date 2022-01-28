using System;
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
            where T : struct
        {
            var newArray = new NativeArray<T>(newSize, allocator, options);

            NativeArray<T>.Copy(array, newArray, array.Length);

            array.Dispose();
            array = newArray;
        }

        /// <summary>
        /// Order a native buffer by the hash codes of its elements using quicksort algorithm.
        /// </summary>
        internal static void OrderByHashcode<T>(this NativeArray<T> array)
            where T : struct
        {
            array.OrderByHashcodeInternal(0, array.Length - 1);
        }

        /// <summary>
        /// Sort a native buffer of comparable elements using quicksort algorithm.
        /// </summary>
        internal static void Sort<T>(this NativeArray<T> array)
            where T : struct, IComparable<T>
        {
            array.SortInternal(0, array.Length - 1);
        }

        /// <summary>
        /// Search linearly through an unordered native buffer for the first instance of an element.
        /// </summary>
        internal static int FindIndex<T>(this NativeArray<T> array, T obj)
            where T : struct, IEquatable<T>
        {
            var length = array.Length;

            for (var i = 0; i < length; i++)
            {
                if (!array[i].Equals(obj))
                {
                    continue;
                }

                return i;
            }

            return -1;
        }

        /// <summary>
        /// Perform a binary-search through a sorted native buffer for the instance of an element.
        /// </summary>
        internal static int FindIndexInSortedSet<T>(this NativeArray<T> array, T obj)
            where T : struct, IComparable<T>
        {
            if (array.Length <= 0)
            {
                return -1;
            }

            int low = 0, high = array.Length - 1;
            while (low <= high)
            {
                var mid = low + ((high - low) / 2);

                var comparison = math.clamp(array[mid].CompareTo(obj), -1, 1);

                switch (comparison)
                {
                    case -1:
                        low = mid + 1;
                        break;
                    case 1:
                        high = mid - 1;
                        break;
                    case 0:
                        return mid;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    default:
                        throw new ArgumentOutOfRangeException();
#else
                        return -1;
#endif
                }
            }

            return -1;
        }

        /// <summary>
        /// Perform a binary search through a sorted native buffer to figure out the potential index to add an element at.
        /// </summary>
        internal static int FindIndexToAddAtInSortedSet<T>(this NativeArray<T> array, T obj)
            where T : struct, IComparable<T>
        {
            if (array.Length <= 0)
            {
                return -1;
            }

            int low = 0, high = array.Length - 1;
            while (low <= high)
            {
                var mid = low + ((high - low) / 2);

                var comparison = math.clamp(array[mid].CompareTo(obj), -1, 1);

                switch (comparison)
                {
                    case -1:
                        low = mid + 1;
                        break;
                    case 1:
                        high = mid - 1;
                        break;
                    case 0:
                        return -1;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    default:
                        throw new ArgumentOutOfRangeException();
#else
                        return -1;
#endif
                }
            }

            return low;
        }

        /// <summary>
        /// Insert an element into a native buffer of sorted elements.
        /// </summary>
        internal static bool AddToSortedSet<T>(this NativeArray<T> array, T obj)
            where T : struct, IComparable<T>
        {
            var insertAt = array
                .GetSubArray(0, array.Length - 1)
                .FindIndexToAddAtInSortedSet(obj);

            if (insertAt == -1)
            {
                return false;
            }

            array.InsertAt(insertAt, obj);
            return true;
        }

        /// <summary>
        /// Remove an element from a native buffer of sorted elements (and pull-forward the following elements).
        /// </summary>
        internal static bool RemoveFromSortedSet<T>(this NativeArray<T> array, T obj)
            where T : struct, IComparable<T>
        {
            var removeAt = array.FindIndexInSortedSet(obj);

            if (removeAt == -1)
            {
                return false;
            }

            array.RemoveAt(removeAt);
            return true;
        }

        /// <summary>
        /// Insert an element into a native buffer at a predefined index.
        /// </summary>
        internal static void InsertAt<T>(this NativeArray<T> array, int index, T obj)
            where T : struct
        {
            var copySize = array.Length - index - 1;

            var src = array.GetSubArray(index, copySize).GetUnsafePtr();
            var dst = array.GetSubArray(index + 1, copySize).GetUnsafePtr();

            UnsafeUtility.MemMove(dst, src, UnsafeUtility.SizeOf<T>() * copySize);

            array[index] = obj;
        }

        /// <summary>
        /// Remove an element from a native buffer at a predefined index and pull forward the following elements.
        /// </summary>
        internal static void RemoveAt<T>(this NativeArray<T> array, int index)
            where T : struct
        {
            var copySize = array.Length - index - 1;

            var dst = array.GetSubArray(index, copySize).GetUnsafePtr();
            var src = array.GetSubArray(index + 1, copySize).GetUnsafePtr();

            UnsafeUtility.MemMove(dst, src, UnsafeUtility.SizeOf<T>() * copySize);

            array[array.Length - 1] = default;
        }

        // recursively order portions of a native buffer by the hash codes
        private static void OrderByHashcodeInternal<T>(this ref NativeArray<T> array, int low, int high)
            where T : struct
        {
            var pivotHashCode = array[high].GetHashCode();

            var i = low - 1;

            for (var j = low; j < high; j++)
            {
                var b = array[j].GetHashCode() < pivotHashCode;

                i += b ? 1 : 0;

                (array[i], array[j]) = b ? (array[j], array[i]) : (array[i], array[j]);
            }

            (array[high], array[i + 1]) = (array[i + 1], array[high]);

            var pivot = i + 1;

            if (low < pivot - 1)
            {
                array.OrderByHashcodeInternal(low, pivot - 1);
            }

            if (pivot + 1 < high)
            {
                array.OrderByHashcodeInternal(pivot + 1, high);
            }
        }

        // recursively sort portions of a native buffer
        private static void SortInternal<T>(this ref NativeArray<T> array, int low, int high)
            where T : struct, IComparable<T>
        {
            var pivotObj = array[high];

            var i = low - 1;

            for (var j = low; j < high; j++)
            {
                var b = array[j].CompareTo(pivotObj) < 0;

                i += b ? 1 : 0;

                (array[i], array[j]) = b ? (array[j], array[i]) : (array[i], array[j]);
            }

            (array[high], array[i + 1]) = (array[i + 1], array[high]);

            var pivot = i + 1;

            if (low < pivot - 1)
            {
                array.SortInternal(low, pivot - 1);
            }

            if (pivot + 1 < high)
            {
                array.SortInternal(pivot + 1, high);
            }
        }
    }
}