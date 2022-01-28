using System;
using NUnit.Framework;
using Unity.Collections;
using Random = UnityEngine.Random;

namespace HiraBots.Editor.Tests
{
    /// <summary>
    /// Tests to validate the native array utility functions.
    /// </summary>
    [TestFixture]
    internal class NativeArrayExtensionsTests
    {
        [Test]
        public void ReallocateToALargerBufferTest()
        {
            var firstSize = Random.Range(0, 16);
            var secondSize = Random.Range(firstSize + 1, 32);

            ArrayReallocationCheck(firstSize, secondSize, true);
        }

        [Test]
        public void ReallocateToASmallerBufferTest()
        {
            var firstSize = Random.Range(16, 32);
            var secondSize = Random.Range(0, firstSize - 1);

            ArrayReallocationCheck(firstSize, secondSize, false);
        }

        private readonly struct ReverseOrderer<T> where T : struct
        {
#pragma warning disable CS0649
            private readonly T m_Internal;
#pragma warning restore CS0649

            public override int GetHashCode()
            {
                return -m_Internal.GetHashCode();
            }
        }

        [Test]
        public void BasicIntReorderTest()
        {
            var size = Random.Range(0, 16);
            var na = new NativeArray<int>(size, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            var na2 = new NativeArray<int>(na, Allocator.Persistent);
            try
            {
                for (var i = 0; i < size; i++)
                {
                    na2[i] = na[i] = Random.Range(-1000, 1001);
                }

                ArrayReorderTest(size, true, na, na2);
            }
            finally
            {
                na.Dispose();
                na2.Dispose();
            }
        }

        [Test]
        public void ReverseIntReorderTest()
        {
            var size = Random.Range(0, 16);
            var na = new NativeArray<int>(size, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            var na2 = new NativeArray<int>(na, Allocator.Persistent);
            try
            {
                for (var i = 0; i < size; i++)
                {
                    na2[i] = na[i] = Random.Range(-1000, 1001);
                }

                ArrayReorderTest(size, false, na, na2);
            }
            finally
            {
                na.Dispose();
                na2.Dispose();
            }
        }

        [Test]
        public void BasicFloatReorderTest()
        {
            var size = Random.Range(0, 16);
            var na = new NativeArray<float>(size, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            var na2 = new NativeArray<float>(na, Allocator.Persistent);
            try
            {
                for (var i = 0; i < size; i++)
                {
                    na2[i] = na[i] = Random.Range(-1000f, 1000f);
                }

                ArrayReorderTest(size, true, na, na2);
            }
            finally
            {
                na.Dispose();
                na2.Dispose();
            }
        }

        [Test]
        public void ReverseFloatReorderTest()
        {
            var size = Random.Range(0, 16);
            var na = new NativeArray<float>(size, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            var na2 = new NativeArray<float>(na, Allocator.Persistent);
            try
            {
                for (var i = 0; i < size; i++)
                {
                    na2[i] = na[i] = Random.Range(-1000f, 1000f);
                }

                ArrayReorderTest(size, false, na, na2);
            }
            finally
            {
                na.Dispose();
                na2.Dispose();
            }
        }

        private readonly struct ReverseSorter<T> : IComparable<ReverseSorter<T>> where T : struct, IComparable<T>
        {
#pragma warning disable CS0649
            private readonly T m_Internal;
#pragma warning restore CS0649

            public int CompareTo(ReverseSorter<T> other)
            {
                return -m_Internal.CompareTo(other.m_Internal);
            }
        }

        [Test]
        public void BasicIntSortTest()
        {
            var size = Random.Range(0, 16);
            var na = new NativeArray<int>(size, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            var na2 = new NativeArray<int>(na, Allocator.Persistent);
            try
            {
                for (var i = 0; i < size; i++)
                {
                    na2[i] = na[i] = Random.Range(-1000, 1001);
                }

                ArraySortTest(size, true, na, na2);
            }
            finally
            {
                na.Dispose();
                na2.Dispose();
            }
        }

        [Test]
        public void ReverseIntSortTest()
        {
            var size = Random.Range(0, 16);
            var na = new NativeArray<int>(size, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            var na2 = new NativeArray<int>(na, Allocator.Persistent);
            try
            {
                for (var i = 0; i < size; i++)
                {
                    na2[i] = na[i] = Random.Range(-1000, 1001);
                }

                ArraySortTest(size, false, na, na2);
            }
            finally
            {
                na.Dispose();
                na2.Dispose();
            }
        }

        [Test]
        public void BasicFloatSortTest()
        {
            var size = Random.Range(0, 16);
            var na = new NativeArray<float>(size, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            var na2 = new NativeArray<float>(na, Allocator.Persistent);
            try
            {
                for (var i = 0; i < size; i++)
                {
                    na2[i] = na[i] = Random.Range(-1000f, 1000f);
                }

                ArraySortTest(size, true, na, na2);
            }
            finally
            {
                na.Dispose();
                na2.Dispose();
            }
        }

        [Test]
        public void ReverseFloatSortTest()
        {
            var size = Random.Range(0, 16);
            var na = new NativeArray<float>(size, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            var na2 = new NativeArray<float>(na, Allocator.Persistent);
            try
            {
                for (var i = 0; i < size; i++)
                {
                    na2[i] = na[i] = Random.Range(-1000f, 1000f);
                }

                ArraySortTest(size, false, na, na2);
            }
            finally
            {
                na.Dispose();
                na2.Dispose();
            }
        }

        private static void ArrayReallocationCheck(int firstSize, int secondSize, bool useFirstSizeForCheckLength)
        {
            var firstArray = new NativeArray<int>(firstSize, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            try
            {
                for (var i = 0; i < firstSize; i++)
                {
                    firstArray[i] = Random.Range(int.MinValue, int.MaxValue);
                }

                var secondArray = new NativeArray<int>(firstArray, Allocator.Persistent);
                try
                {
                    secondArray.Reallocate(secondSize, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

                    var length = useFirstSizeForCheckLength ? firstSize : secondSize;
                    for (var i = 0; i < length; i++)
                    {
                        Assert.AreEqual(firstArray[i], secondArray[i], $"Failed at index {i}.");
                    }
                }
                finally
                {
                    secondArray.Dispose();
                }
            }
            finally
            {
                firstArray.Dispose();
            }
        }

        private static void ArrayReorderTest<T>(int size, bool forwards, NativeArray<T> na, NativeArray<T> na2)
            where T : struct
        {
            if (forwards)
            {
                na.OrderByHashcode();
            }
            else
            {
                na.Reinterpret<ReverseOrderer<T>>().OrderByHashcode();
            }

            for (var i = 0; i < size - 1; i++)
            {
                var current = na[i].GetHashCode();
                var next = na[i + 1].GetHashCode();

                var expected = forwards ? current <= next : current >= next;
                if (!expected)
                {
                    if (forwards)
                    {
                        na2.OrderByHashcode();
                    }
                    else
                    {
                        na2.Reinterpret<ReverseOrderer<T>>().OrderByHashcode();
                    }

                    throw new AssertionException($"Failed at index {i} && {i + 1}.");
                }
            }
        }

        private static void ArraySortTest<T>(int size, bool forwards, NativeArray<T> na, NativeArray<T> na2)
            where T : struct, IComparable<T>
        {
            if (forwards)
            {
                na.Sort();
            }
            else
            {
                na.Reinterpret<ReverseSorter<T>>().Sort();
            }

            for (var i = 0; i < size - 1; i++)
            {
                var comparison = na[i].CompareTo(na[i + 1]);

                var expected = forwards ? comparison <= 0 : comparison >= 0;
                if (!expected)
                {
                    if (forwards)
                    {
                        na2.Sort();
                    }
                    else
                    {
                        na2.Reinterpret<ReverseSorter<T>>().Sort();
                    }

                    throw new AssertionException($"Failed at index {i} && {i + 1}.");
                }
            }
        }
    }
}