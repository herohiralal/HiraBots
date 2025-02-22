using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace HiraBots
{
    internal static unsafe class UnmanagedCollections
    {
        #region Data

        /// <summary>
        /// Type-safe list data.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        [DebuggerTypeProxy(typeof(DataDebugView<>))]
        [DebuggerDisplay("Count = {m_Count}, Capacity = {m_Capacity}")]
        // ReSharper disable once UnusedTypeParameter
        internal struct Data<T> where T : struct
        {
            [NativeDisableUnsafePtrRestriction] private readonly void* m_Buffer;
            private readonly int m_Capacity;
            private readonly int m_Count;
            private readonly Allocator m_Allocator;
        }

        /// <summary>
        /// Type-safe ordered list data.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        [DebuggerTypeProxy(typeof(OrderedDataDebugView<>))]
        [DebuggerDisplay("Count = {m_Count}, Capacity = {m_Capacity}")]
        // ReSharper disable once UnusedTypeParameter
        internal struct OrderedData<T> where T : struct, System.IComparable<T>
        {
            [NativeDisableUnsafePtrRestriction] private readonly void* m_Buffer;
            private readonly int m_Capacity;
            private readonly int m_Count;
            private readonly Allocator m_Allocator;
        }

        /// <summary>
        /// Type-unsafe list data. Used only for internal functionalities.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct UntypedData
        {
            [NativeDisableUnsafePtrRestriction] internal void* m_Buffer;
            internal int m_Capacity;
            internal int m_Count;
            internal Allocator m_Allocator;
        }

        #endregion

        #region Debug

        /// <summary>
        /// Debug view of Data.
        /// </summary>
        internal sealed class DataDebugView<T> where T : struct
        {
            private readonly UntypedData m_Data;

            public DataDebugView(Data<T> data) => m_Data = *(UntypedData*) UnsafeUtility.AddressOf(ref data);

            public T[] items
            {
                get
                {
                    if (m_Data.m_Allocator < Allocator.None || m_Data.m_Count == 0 || m_Data.m_Capacity == 0 || m_Data.m_Buffer == null)
                    {
                        return new T[0];
                    }

                    var output = new T[m_Data.m_Count];
                    var handle = GCHandle.Alloc(output, GCHandleType.Pinned);
                    UnsafeUtility.MemCpy((void*) handle.AddrOfPinnedObject(), m_Data.m_Buffer, m_Data.m_Count * UnsafeUtility.SizeOf<T>());
                    handle.Free();
                    return output;
                }
            }
        }

        /// <summary>
        /// Debug view of OrderedData.
        /// </summary>
        internal sealed class OrderedDataDebugView<T> where T : struct, System.IComparable<T>
        {
            private readonly UntypedData m_Data;

            public OrderedDataDebugView(OrderedData<T> data) => m_Data = *(UntypedData*) UnsafeUtility.AddressOf(ref data);

            public T[] items
            {
                get
                {
                    if (m_Data.m_Allocator < Allocator.None || m_Data.m_Count == 0 || m_Data.m_Capacity == 0 || m_Data.m_Buffer == null)
                    {
                        return new T[0];
                    }

                    var output = new T[m_Data.m_Count];
                    var handle = GCHandle.Alloc(output, GCHandleType.Pinned);
                    UnsafeUtility.MemCpy((void*) handle.AddrOfPinnedObject(), m_Data.m_Buffer, m_Data.m_Count * UnsafeUtility.SizeOf<T>());
                    handle.Free();
                    return output;
                }
            }
        }

        #endregion

        #region Conversion

        // convert a list to untyped data ptr
        private static UntypedData* GetPtr<T>(this ref NativeArray<Data<T>> list)
            where T : struct
        {
            return (UntypedData*) list.Reinterpret<UntypedData>().GetUnsafePtr();
        }

        // convert a list to untyped readonly data ptr
        private static UntypedData* GetReadOnlyPtr<T>(this ref NativeArray<Data<T>> list)
            where T : struct
        {
            return (UntypedData*) list.Reinterpret<UntypedData>().GetUnsafeReadOnlyPtr();
        }

        // convert an ordered list to untyped data ptr
        private static UntypedData* GetPtr<T>(this ref NativeArray<OrderedData<T>> list)
            where T : struct, System.IComparable<T>
        {
            return (UntypedData*) list.Reinterpret<UntypedData>().GetUnsafePtr();
        }

        // convert an ordered list to untyped readonly data ptr
        private static UntypedData* GetReadOnlyPtr<T>(this ref NativeArray<OrderedData<T>> list)
            where T : struct, System.IComparable<T>
        {
            return (UntypedData*) list.Reinterpret<UntypedData>().GetUnsafeReadOnlyPtr();
        }

        /// <summary>
        /// Get a pointer to the memory buffer of an unmanaged list.
        /// </summary>
        internal static void* GetUnsafeUnmanagedListPtr<T>(this ref NativeArray<Data<T>> list)
            where T : struct
        {
            return list.GetPtr()->m_Buffer;
        }

        /// <summary>
        /// Get a read-only pointer to the memory buffer of an unmanaged list.
        /// </summary>
        internal static void* GetUnsafeUnmanagedListReadOnlyPtr<T>(this ref NativeArray<Data<T>> list)
            where T : struct
        {
            return list.GetReadOnlyPtr()->m_Buffer;
        }

        /// <summary>
        /// Get a pointer to the memory buffer of an unmanaged ordered list.
        /// </summary>
        internal static void* GetUnsafeUnmanagedOrderedListPtr<T>(this ref NativeArray<OrderedData<T>> list)
            where T : struct, System.IComparable<T>
        {
            return list.GetPtr()->m_Buffer;
        }

        /// <summary>
        /// Get a read-only pointer to the memory buffer of an unmanaged ordered list.
        /// </summary>
        internal static void* GetUnsafeUnmanagedOrderedListReadOnlyPtr<T>(this ref NativeArray<OrderedData<T>> list)
            where T : struct, System.IComparable<T>
        {
            return list.GetReadOnlyPtr()->m_Buffer;
        }

        #endregion

        #region Allocation

        /// <summary>
        /// Create an unmanaged list.
        /// </summary>
        internal static NativeArray<Data<T>> CreateUnmanagedList<T>(Allocator allocator, int capacity = 1)
            where T : struct
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (capacity < 1)
            {
                throw new System.ArgumentOutOfRangeException(nameof(capacity), "Capacity must be >= 1.");
            }
#endif
            var list = new NativeArray<Data<T>>(1, allocator);

            capacity = (capacity + 7) & ~7;

            *list.GetPtr() = new UntypedData
            {
                m_Buffer = UnsafeUtility.Malloc(capacity * UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), allocator),
                m_Capacity = capacity,
                m_Count = 0,
                m_Allocator = allocator
            };

            return list;
        }

        /// <summary>
        /// Create an unmanaged ordered list.
        /// </summary>
        internal static NativeArray<OrderedData<T>> CreateUnmanagedOrderedList<T>(Allocator allocator, int capacity = 1)
            where T : struct, System.IComparable<T>
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (capacity < 1)
            {
                throw new System.ArgumentOutOfRangeException(nameof(capacity), "Capacity must be >= 1.");
            }
#endif
            var list = new NativeArray<OrderedData<T>>(1, allocator);

            capacity = (capacity + 7) & ~7;

            *list.GetPtr() = new UntypedData
            {
                m_Buffer = UnsafeUtility.Malloc(capacity * UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), allocator),
                m_Capacity = capacity,
                m_Count = 0,
                m_Allocator = allocator
            };

            return list;
        }

        /// <summary>
        /// Create an unmanaged dictionary.
        /// </summary>
        internal static (NativeArray<OrderedData<TKey>> keys, NativeArray<Data<TValue>> values) CreateDictionary<TKey, TValue>(
            Allocator allocator, int capacity = 1)
            where TKey : struct, System.IComparable<TKey>
            where TValue : struct
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (capacity < 1)
            {
                throw new System.ArgumentOutOfRangeException(nameof(capacity), "Capacity must be >= 1.");
            }
#endif
            var keys = CreateUnmanagedOrderedList<TKey>(allocator, capacity);
            var values = CreateUnmanagedList<TValue>(allocator, capacity);

            return (keys, values);
        }

        #endregion

        #region Deallocation

        // deallocate a list
        private static void Deallocate(ref NativeArray<UntypedData> list)
        {
            var data = list[0];

            UnsafeUtility.Free(data.m_Buffer, data.m_Allocator);
            data.m_Buffer = null;
            data.m_Capacity = 0;
            data.m_Count = 0;
            data.m_Allocator = Allocator.None;

            list[0] = data;
        }

        /// <summary>
        /// Dispose an unmanaged list.
        /// </summary>
        internal static void DisposeUnmanagedList<T>(this ref NativeArray<Data<T>> list)
            where T : struct
        {
            var reinterpretedList = list.Reinterpret<UntypedData>();

            Deallocate(ref reinterpretedList);

            list.Dispose();
        }

        /// <summary>
        /// Dispose an unmanaged ordered list.
        /// </summary>
        internal static void DisposeUnmanagedOrderedList<T>(this ref NativeArray<OrderedData<T>> list)
            where T : struct, System.IComparable<T>
        {
            var reinterpretedList = list.Reinterpret<UntypedData>();

            Deallocate(ref reinterpretedList);

            list.Dispose();
        }

        /// <summary>
        /// Dispose an unmanaged dictionary.
        /// </summary>
        internal static void DisposeUnmanagedDictionary<TKey, TValue>(
            this ref (NativeArray<OrderedData<TKey>> keys, NativeArray<Data<TValue>> values) dictionary)
            where TKey: struct, System.IComparable<TKey>
            where TValue : struct
        {
            var (keys, values) = dictionary;

            keys.DisposeUnmanagedOrderedList();
            values.DisposeUnmanagedList();
        }

        /// <summary>
        /// Dispose an unmanaged list after a job.
        /// </summary>
        /// <returns>JobHandle for the disposal job.</returns>
        internal static JobHandle DisposeUnmanagedList<T>(this ref NativeArray<Data<T>> list, JobHandle dependencies)
            where T : struct
        {
            var reinterpretedList = list.Reinterpret<UntypedData>();

            var internalDisposeJob = new DisposeJob(reinterpretedList);

            var internalDisposeJobHandle = internalDisposeJob.Schedule(dependencies);

            return list.Dispose(internalDisposeJobHandle);
        }

        /// <summary>
        /// Dispose an unmanaged ordered list after a job.
        /// </summary>
        /// <returns>JobHandle for the disposal job.</returns>
        internal static JobHandle DisposeUnmanagedOrderedList<T>(this ref NativeArray<OrderedData<T>> list, JobHandle dependencies)
            where T : struct, System.IComparable<T>
        {
            var reinterpretedList = list.Reinterpret<UntypedData>();

            var internalDisposeJob = new DisposeJob(reinterpretedList);

            var internalDisposeJobHandle = internalDisposeJob.Schedule(dependencies);

            return list.Dispose(internalDisposeJobHandle);
        }

        // a job to deallocate an untyped list
        private struct DisposeJob : IJob
        {
            public DisposeJob(NativeArray<UntypedData> list) => m_List = list;
            private NativeArray<UntypedData> m_List;
            public void Execute() => Deallocate(ref m_List);
        }

        /// <summary>
        /// Dispose an unmanaged dictionary after a job.
        /// </summary>
        /// <returns>JobHandle for the disposal job.</returns>
        internal static JobHandle DisposeUnmanagedDictionary<TKey, TValue>(
            this ref (NativeArray<OrderedData<TKey>> keys, NativeArray<Data<TValue>> values) dictionary, JobHandle dependencies)
            where TKey : struct, System.IComparable<TKey>
            where TValue : struct
        {
            var (keys, values) = dictionary;

            return JobHandle.CombineDependencies(keys.DisposeUnmanagedOrderedList(dependencies), values.DisposeUnmanagedList(dependencies));
        }

        #endregion

        #region Metadata

        /// <summary>
        /// Get the number of elements in an unmanaged list.
        /// </summary>
        internal static int Count<T>(this ref NativeArray<Data<T>> list)
            where T : struct
        {
            return list.GetReadOnlyPtr()->m_Count;
        }

        /// <summary>
        /// Get the current size of the buffer in an unmanaged list.
        /// </summary>
        internal static int Capacity<T>(this ref NativeArray<Data<T>> list)
            where T : struct
        {
            return list.GetReadOnlyPtr()->m_Capacity;
        }

        /// <summary>
        /// Get the number of elements in an unmanaged ordered list.
        /// </summary>
        internal static int Count<T>(this ref NativeArray<OrderedData<T>> list)
            where T : struct, System.IComparable<T>
        {
            return list.GetReadOnlyPtr()->m_Count;
        }

        /// <summary>
        /// Get the current size of the buffer in an unmanaged ordered list.
        /// </summary>
        internal static int Capacity<T>(this ref NativeArray<OrderedData<T>> list)
            where T : struct, System.IComparable<T>
        {
            return list.GetReadOnlyPtr()->m_Capacity;
        }

        /// <summary>
        /// Get the number of keys in an unmanaged dictionary.
        /// </summary>
        internal static int Count<TKey, TValue>(this ref (NativeArray<OrderedData<TKey>> keys, NativeArray<Data<TValue>> values) dictionary)
            where TKey : struct, System.IComparable<TKey>
            where TValue : struct
        {
            return dictionary.keys.GetReadOnlyPtr()->m_Count;
        }

        /// <summary>
        /// Get the current size of the keys buffer in an unmanaged dictionary.
        /// </summary>
        internal static int Capacity<TKey, TValue>(this ref (NativeArray<OrderedData<TKey>> keys, NativeArray<Data<TValue>> values) dictionary)
            where TKey : struct, System.IComparable<TKey>
            where TValue : struct
        {
            return dictionary.keys.GetReadOnlyPtr()->m_Capacity;
        }

        // check if an index is within the appropriate range
        private static void CheckIfIndexIsInRange(UntypedData* data, int index)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (index < 0 || index >= data->m_Count)
            {
                throw new System.ArgumentOutOfRangeException(nameof(index), $"Index {index} is out of range " +
                                                                            $"[0, {data->m_Count}).");
            }
#endif
        }

        #endregion

        #region Index Accessors

        /// <summary>
        /// Get an element at a given index in an unmanaged list.
        /// </summary>
        internal static T GetElementAt<T>(this ref NativeArray<Data<T>> list, int index)
            where T : struct
        {
            var data = list.GetReadOnlyPtr();
            CheckIfIndexIsInRange(data, index);
            return UnsafeUtility.ReadArrayElement<T>(data->m_Buffer, index);
        }

        /// <summary>
        /// Get an element at a given index in an unmanaged ordered list.
        /// </summary>
        internal static T GetElementAt<T>(this ref NativeArray<OrderedData<T>> list, int index)
            where T : struct, System.IComparable<T>
        {
            var data = list.GetReadOnlyPtr();
            CheckIfIndexIsInRange(data, index);
            return UnsafeUtility.ReadArrayElement<T>(data->m_Buffer, index);
        }

        /// <summary>
        /// Set an element at a given index in an unmanaged list.
        /// </summary>
        internal static void SetElementAt<T>(this ref NativeArray<Data<T>> list, int index, T value)
            where T : struct
        {
            var data = list.GetPtr();
            CheckIfIndexIsInRange(data, index);
            UnsafeUtility.WriteArrayElement<T>(data->m_Buffer, index, value);
        }

        /// <summary>
        /// Set an element at a given index in an unmanaged ordered list.
        /// </summary>
        internal static void SetElementAt<T>(this ref NativeArray<OrderedData<T>> list, int index, T value)
            where T : struct, System.IComparable<T>
        {
            var data = list.GetPtr();
            CheckIfIndexIsInRange(data, index);
            UnsafeUtility.WriteArrayElement<T>(data->m_Buffer, index, value);
        }

        #endregion

        #region Clearing

        /// <summary>
        /// Clear an unmanaged list.
        /// </summary>
        internal static void Clear<T>(this ref NativeArray<Data<T>> list)
            where T : struct
        {
            list.GetPtr()->m_Count = 0;
        }

        /// <summary>
        /// Clear an unmanaged ordered list.
        /// </summary>
        internal static void Clear<T>(this ref NativeArray<OrderedData<T>> list)
            where T : struct, System.IComparable<T>
        {
            list.GetPtr()->m_Count = 0;
        }

        /// <summary>
        /// Clear an unmanaged dictionary.
        /// </summary>
        internal static void Clear<TKey, TValue>(this ref (NativeArray<OrderedData<TKey>> keys, NativeArray<Data<TValue>> values) dictionary)
            where TKey : struct, System.IComparable<TKey>
            where TValue : struct
        {
            var (keys, values) = dictionary;

            keys.Clear();
            values.Clear();
        }

        #endregion

        #region Reallocation

        // ensure capacity of an untyped list
        private static void EnsureCapacityInternal<T>(UntypedData* data, int min) where T : struct
        {
            var proposedCap = data->m_Capacity;
            do
            {
                proposedCap *= 2;
            } while (proposedCap < min);

            var cap = proposedCap;

            var newBuffer = UnsafeUtility.Malloc(cap * UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), data->m_Allocator);

            UnsafeUtility.MemCpy(newBuffer, data->m_Buffer, data->m_Count * UnsafeUtility.SizeOf<T>());

            UnsafeUtility.Free(data->m_Buffer, data->m_Allocator);
            data->m_Buffer = newBuffer;
            data->m_Capacity = cap;
        }

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

            NativeArray<T>.Copy(array, newArray, math.min(array.Length, newSize));

            array.Dispose();
            array = newArray;
        }

        /// <summary>
        /// Ensure capacity of an unmanaged list. Primary usage is avoiding repeated allocations.
        /// </summary>
        internal static void EnsureCapacity<T>(this ref NativeArray<Data<T>> list, int min)
            where T : struct
        {
            if (list.GetReadOnlyPtr()->m_Capacity >= min)
            {
                return;
            }

            EnsureCapacityInternal<T>(list.GetPtr(), min);
        }

        /// <summary>
        /// Ensure capacity of an unmanaged ordered list. Primary usage is avoiding repeated allocations.
        /// </summary>
        internal static void EnsureCapacity<T>(this ref NativeArray<OrderedData<T>> list, int min)
            where T : struct, System.IComparable<T>
        {
            if (list.GetReadOnlyPtr()->m_Capacity >= min)
            {
                return;
            }

            EnsureCapacityInternal<T>(list.GetPtr(), min);
        }

        /// <summary>
        /// Ensure capacity of an unmanaged dictionary. Primary usage is avoiding repeated allocations.
        /// </summary>
        internal static void EnsureCapacity<TKey, TValue>(this ref (NativeArray<OrderedData<TKey>> keys, NativeArray<Data<TValue>> values) dictionary,
            int min)
            where TKey : struct, System.IComparable<TKey>
            where TValue : struct
        {
            var (keys, values) = dictionary;

            keys.EnsureCapacity(min);
            values.EnsureCapacity(min);
        }

        #endregion

        #region (Insert at / Remove from) the end

        /// <summary>
        /// Push an item to the back of an unmanaged list.
        /// </summary>
        internal static void Push<T>(this ref NativeArray<Data<T>> list, T item)
            where T : struct
        {
            var data = list.GetPtr();

            list.EnsureCapacity(data->m_Count + 1);

            UnsafeUtility.WriteArrayElement<T>(data->m_Buffer, data->m_Count, item);
            data->m_Count++;
        }

        /// <summary>
        /// Pop the last item from an unmanaged list.
        /// </summary>
        internal static T Pop<T>(this ref NativeArray<Data<T>> list)
            where T : struct
        {
            var data = list.GetPtr();

            data->m_Count--;
            return UnsafeUtility.ReadArrayElement<T>(data->m_Buffer, data->m_Count);
        }

        #endregion

        #region (Insert at / Remove from)

        // insert an element at a given index in an untyped list
        private static void InsertAt<T>(UntypedData* data, int index, T item)
            where T : struct
        {
            var copyCount = data->m_Count - index;
            var copySize = copyCount * UnsafeUtility.SizeOf<T>();

            var src = (void*) ((byte*) data->m_Buffer + ((index + 0) * UnsafeUtility.SizeOf<T>()));
            var dst = (void*) ((byte*) data->m_Buffer + ((index + 1) * UnsafeUtility.SizeOf<T>()));

            UnsafeUtility.MemMove(dst, src, copySize);

            UnsafeUtility.WriteArrayElement(data->m_Buffer, index, item);
            data->m_Count++;
        }

        // remove an element from a given index in an untyped list
        private static void RemoveFrom<T>(UntypedData* data, int index)
            where T : struct
        {
            var copyCount = data->m_Count - index - 1;
            var copySize = copyCount * UnsafeUtility.SizeOf<T>();

            var dst = (void*) ((byte*) data->m_Buffer + ((index + 0) * UnsafeUtility.SizeOf<T>()));
            var src = (void*) ((byte*) data->m_Buffer + ((index + 1) * UnsafeUtility.SizeOf<T>()));

            UnsafeUtility.MemMove(dst, src, copySize);
            data->m_Count--;
        }

        /// <summary>
        /// Insert an element at a given index in an unmanaged list.
        /// </summary>
        internal static void InsertAt<T>(this ref NativeArray<Data<T>> list, int index, T item)
            where T : struct
        {
            var data = list.GetPtr();
            CheckIfIndexIsInRange(data, index);
            list.EnsureCapacity(data->m_Count + 1);
            InsertAt<T>(data, index, item);
        }

        /// <summary>
        /// Insert an element at a given index in an unmanaged ordered list.
        /// </summary>
        internal static void InsertAt<T>(this ref NativeArray<OrderedData<T>> list, int index, T item)
            where T : struct, System.IComparable<T>
        {
            var data = list.GetPtr();
            CheckIfIndexIsInRange(data, index);
            list.EnsureCapacity(data->m_Count + 1);
            InsertAt<T>(data, index, item);
        }

        /// <summary>
        /// Remove an element from a given index in an unmanaged list.
        /// </summary>
        internal static void RemoveFrom<T>(this ref NativeArray<Data<T>> list, int index)
            where T : struct
        {
            var data = list.GetPtr();
            CheckIfIndexIsInRange(data, index);
            RemoveFrom<T>(data, index);
        }

        /// <summary>
        /// Remove an element from a given index in an unmanaged orderedlist.
        /// </summary>
        internal static void RemoveFrom<T>(this ref NativeArray<OrderedData<T>> list, int index)
            where T : struct, System.IComparable<T>
        {
            var data = list.GetPtr();
            CheckIfIndexIsInRange(data, index);
            RemoveFrom<T>(data, index);
        }

        #endregion

        #region Search

        /// <summary>
        /// Get the index of the first occurence of an item in a list.
        /// </summary>
        internal static int FirstIndexOf<T, TEquatable>(this ref NativeArray<Data<T>> list, TEquatable item)
            where T : struct
            where TEquatable : struct, System.IEquatable<T>
        {
            var data = list.GetReadOnlyPtr();

            var c = data->m_Count;
            for (var i = 0; i < c; i++)
            {
                var element = UnsafeUtility.ReadArrayElement<T>(data->m_Buffer, i);

                if (item.Equals(element))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Get the index of the last occurence of an item in a list.
        /// </summary>
        internal static int LastIndexOf<T, TEquatable>(this ref NativeArray<Data<T>> list, TEquatable item)
            where T : struct
            where TEquatable : struct, System.IEquatable<T>
        {
            var data = list.GetReadOnlyPtr();

            var c = data->m_Count;
            for (var i = c - 1; i >= 0; i--)
            {
                var element = UnsafeUtility.ReadArrayElement<T>(data->m_Buffer, i);

                if (item.Equals(element))
                {
                    return i;
                }
            }

            return -1;
        }

        // perform binary search to get the index of an element in an ordered list, as well as a potential
        // index to insert the said element at, to maintain the order
        private static (int actualIndex, int potentialIndex) IndexOf<T>(this ref NativeArray<OrderedData<T>> list, T item)
            where T : struct, System.IComparable<T>
        {
            var data = list.GetReadOnlyPtr();

            var low = 0;
            var high = data->m_Count - 1;

            while (low <= high)
            {
                var mid = low + ((high - low) / 2);

                var element = UnsafeUtility.ReadArrayElement<T>(data->m_Buffer, mid);

                var comparison = math.clamp(element.CompareTo(item), -1, 1);

                switch (comparison)
                {
                    case -1:
                        low = mid + 1;
                        break;
                    case 1:
                        high = mid - 1;
                        break;
                    case 0:
                        return (mid, mid);
                    default:
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                        throw new System.ArgumentOutOfRangeException();
#else
                        return (-1, low);
#endif
                }
            }

            return (-1, low);
        }

        /// <summary>
        /// Find an index of an element in an unmanaged ordered list.
        /// </summary>
        internal static int Find<T>(this ref NativeArray<OrderedData<T>> list, T item)
            where T : struct, System.IComparable<T>
        {
            return list.IndexOf(item).actualIndex;
        }

        /// <summary>
        /// Check whether an unmanaged ordered list contains an item.
        /// </summary>
        internal static bool Contains<T>(this ref NativeArray<OrderedData<T>> list, T item)
            where T : struct, System.IComparable<T>
        {
            return list.IndexOf(item).actualIndex != -1;
        }

        /// <summary>
        /// Find a value in a dictionary based on a key.
        /// </summary>
        internal static TValue Find<TKey, TValue>(this ref (NativeArray<OrderedData<TKey>> keys, NativeArray<Data<TValue>> values) dictionary,
            TKey key)
            where TKey : struct, System.IComparable<TKey>
            where TValue : struct
        {
            var (keys, values) = dictionary;

            var (actualIndex, _) = keys.IndexOf(key);

            if (actualIndex == -1)
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                throw new System.Collections.Generic.KeyNotFoundException($"Could not find key {key}.");
#else
                return default;
#endif
            }

            var valueData = values.GetReadOnlyPtr();
            return UnsafeUtility.ReadArrayElement<TValue>(valueData->m_Buffer, actualIndex);
        }

        #endregion

        #region Sort

        /// <summary>
        /// Treat a sorted list as unsorted.
        /// </summary>
        internal static NativeArray<Data<T>> AsUnsorted<T>(this ref NativeArray<OrderedData<T>> list)
            where T : struct, System.IComparable<T>
        {
            return list.Reinterpret<Data<T>>();
        }

        /// <summary>
        /// Sort an unmanaged list.
        /// </summary>
        internal static NativeArray<OrderedData<T>> Sort<T>(this ref NativeArray<Data<T>> list)
            where T : struct, System.IComparable<T>
        {
            var data = list.GetPtr();
            SortInternal<T>(data->m_Buffer, 0, data->m_Count - 1);
            return list.Reinterpret<OrderedData<T>>();
        }

        // recursively sort a data stream using quick sort on comparable items
        private static void SortInternal<T>(void* buffer, int low, int high)
            where T : struct, System.IComparable<T>
        {
            if (low >= high)
            {
                return;
            }

            var pivotObj = UnsafeUtility.ReadArrayElement<T>(buffer, high);
            var i = low - 1;

            for (var j = low; j < high; j++)
            {
                var jElement = UnsafeUtility.ReadArrayElement<T>(buffer, j);
                if (jElement.CompareTo(pivotObj) < 0)
                {
                    i++;

                    var iElement = UnsafeUtility.ReadArrayElement<T>(buffer, i);

                    UnsafeUtility.WriteArrayElement<T>(buffer, j, iElement);
                    UnsafeUtility.WriteArrayElement<T>(buffer, i, jElement);
                }
            }

            var highElement = UnsafeUtility.ReadArrayElement<T>(buffer, high);
            var iPlusOneElement = UnsafeUtility.ReadArrayElement<T>(buffer, i + 1);

            UnsafeUtility.WriteArrayElement<T>(buffer, high, iPlusOneElement);
            UnsafeUtility.WriteArrayElement<T>(buffer, i + 1, highElement);

            var pivot = i + 1;
            SortInternal<T>(buffer, low, pivot - 1);
            SortInternal<T>(buffer, pivot + 1, high);
        }

        /// <summary>
        /// Remove duplicates from an unmanaged ordered list.
        /// </summary>
        internal static void RemoveDuplicates<T>(this ref NativeArray<OrderedData<T>> list)
            where T : struct, System.IEquatable<T>, System.IComparable<T>
        {
            if (list.GetReadOnlyPtr()->m_Count <= 1)
            {
                return;
            }

            var data = list.GetPtr();

            // perform in reverse to not remove from in front like a moron
            for (var i = data->m_Count - 1; i > 0; i--)
            {
                var currentElement = UnsafeUtility.ReadArrayElement<T>(data->m_Buffer, i);
                var priorElement = UnsafeUtility.ReadArrayElement<T>(data->m_Buffer, i - 1);

                if (priorElement.Equals(currentElement))
                {
                    RemoveFrom<T>(data, i);
                }
            }
        }

        #endregion

        #region Add/Remove

        /// <summary>
        /// Add an element to an unmanaged ordered list, while maintaining the order.
        /// </summary>
        internal static bool Add<T>(this ref NativeArray<OrderedData<T>> list, T item, bool allowDuplicate = false)
            where T : struct, System.IComparable<T>
        {
            var (actualIndex, potentialIndex) = list.IndexOf(item);

            if (!allowDuplicate && actualIndex != -1)
            {
                return false;
            }

            var data = list.GetPtr();
            list.EnsureCapacity(data->m_Count + 1);
            InsertAt<T>(data, potentialIndex, item);
            return true;
        }

        /// <summary>
        /// Remove an element from an unmanaged ordered list, while maintaining the order.
        /// </summary>
        internal static bool Remove<T>(this ref NativeArray<OrderedData<T>> list, T item)
            where T : struct, System.IComparable<T>
        {
            var (actualIndex, _) = list.IndexOf(item);

            if (actualIndex == -1)
            {
                return false;
            }

            var data = list.GetPtr();
            RemoveFrom<T>(data, actualIndex);
            return true;
        }

        /// <summary>
        /// Add a key-value pair to an unmanaged dictionary.
        /// </summary>
        internal static bool Add<TKey, TValue>(this ref (NativeArray<OrderedData<TKey>> keys, NativeArray<Data<TValue>> values) dictionary,
            TKey key, TValue value, bool overrideExisting = false)
            where TKey : struct, System.IComparable<TKey>
            where TValue : struct
        {
            var (keys, values) = dictionary;

            var (actualIndex, potentialIndex) = keys.IndexOf(key);
            if (actualIndex != -1)
            {
                if (overrideExisting)
                {
                    values.SetElementAt(actualIndex, value);
                }

                return false;
            }

            var keyData = keys.GetPtr();
            var valueData = values.GetPtr();

            keys.EnsureCapacity(keyData->m_Count + 1);
            InsertAt<TKey>(keyData, potentialIndex, key);
            values.EnsureCapacity(valueData->m_Count + 1);
            InsertAt<TValue>(valueData, potentialIndex, value);

            return true;
        }

        /// <summary>
        /// Remove a key (and its corresponding value) from an unmanaged dictionary.
        /// </summary>
        internal static bool Remove<TKey, TValue>(this ref (NativeArray<OrderedData<TKey>> keys, NativeArray<Data<TValue>> values) dictionary,
            TKey key)
            where TKey : struct, System.IComparable<TKey>
            where TValue : struct
        {
            var (keys, values) = dictionary;

            var (actualIndex, _) = keys.IndexOf(key);

            if (actualIndex == -1)
            {
                return false;
            }

            var keyData = keys.GetPtr();
            var valueData = values.GetPtr();

            RemoveFrom<TKey>(keyData, actualIndex);
            RemoveFrom<TValue>(valueData, actualIndex);

            return true;
        }

        #endregion
    }
}