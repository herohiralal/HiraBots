using System.Collections.Generic;
using Unity.Collections;

namespace UnityEngine
{
    public static class ReadOnlyCollectionAccessorsExtensions
    {
        public static ReadOnlyArrayAccessor<T> ReadOnly<T>(this T[] actual)
        {
            return actual;
        }

        public static ReadOnlyDictionaryAccessor<TKey, TValue> ReadOnly<TKey, TValue>(this Dictionary<TKey, TValue> actual)
        {
            return actual;
        }

        public static ReadOnlyHashSetAccessor<T> ReadOnly<T>(this HashSet<T> actual)
        {
            return actual;
        }

        public static ReadOnlyListAccessor<T> ReadOnly<T>(this List<T> actual)
        {
            return actual;
        }

        public static ReadOnlyNativeArrayAccessor<T> ReadOnly<T>(this NativeArray<T> actual)
            where T : struct
        {
            return actual;
        }
    }
}