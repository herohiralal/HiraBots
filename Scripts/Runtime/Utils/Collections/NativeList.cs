using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace HiraBots
{
    internal unsafe struct NativeListMetadata
    {
        [NativeDisableUnsafePtrRestriction] internal void* m_Buffer;
        internal int m_Capacity;
        internal int m_Count;
    }

    [StructLayout(LayoutKind.Sequential)]
    [NativeContainer]
    [NativeContainerSupportsDeallocateOnJobCompletion]
    [DebuggerDisplay("Count = {count}, Capacity = {capacity}")]
    [DebuggerTypeProxy(typeof(NativeListDebugView<>))]
    internal unsafe struct NativeList<T> : IDisposable, IEquatable<NativeList<T>> where T : struct
    {
        [NativeDisableUnsafePtrRestriction] private NativeListMetadata* m_Metadata;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        private AtomicSafetyHandle m_Safety;
        private DisposeSentinel m_DisposeSentinel;
#endif

        private Allocator m_AllocatorLabel;

        internal NativeList(Allocator allocator)
        {
            Allocate(8, allocator, out this);
        }

        internal NativeList(int capacity, Allocator allocator)
        {
            Allocate((capacity + 7) & ~7, allocator, out this);
        }

        internal NativeList(NativeArray<T> array, Allocator allocator)
        {
            var length = (array.Length + 7) & ~7;

            Allocate(length, allocator, out this);

            var src = array.GetUnsafeReadOnlyPtr();
            var dst = m_Metadata->m_Buffer;

            UnsafeUtility.MemCpy(dst, src, UnsafeUtility.SizeOf<T>() * array.Length);
            m_Metadata->m_Count = array.Length;
        }

        private static void Allocate(int capacity, Allocator allocator, out NativeList<T> list)
        {
            var size = (long) UnsafeUtility.SizeOf<T>() * capacity;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (allocator <= Allocator.None)
            {
                throw new ArgumentException("Allocator must be Temp, TempJob or Persistent", nameof(allocator));
            }

            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be >= 0");
            }

            if (!UnsafeUtility.IsValidNativeContainerElementType<T>())
            {
                throw new InvalidOperationException($"{typeof(T)} used in NativeList<{typeof(T)}>" +
                                                    " must be unmanaged (contain no managed types) and" +
                                                    "cannot itself be a native container type.");
            }
#endif

            list = new NativeList<T>();
            list.m_Metadata = (NativeListMetadata*) UnsafeUtility.Malloc(UnsafeUtility.SizeOf<NativeListMetadata>(),
                UnsafeUtility.AlignOf<NativeListMetadata>(),
                allocator);
            list.m_Metadata->m_Buffer = UnsafeUtility.Malloc(size, UnsafeUtility.AlignOf<T>(), allocator);
            list.m_Metadata->m_Capacity = capacity;
            list.m_Metadata->m_Count = 0;
            list.m_AllocatorLabel = allocator;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Create(out list.m_Safety, out list.m_DisposeSentinel, 1, allocator);
#endif
        }

        private void EnsureCapacityToInsert(int countToInsert)
        {
            var newCapacity = m_Metadata->m_Count + countToInsert;

            if (m_Metadata->m_Capacity >= newCapacity)
            {
                return;
            }

            var cap = (newCapacity + 7) & ~7;

            var size = (long) UnsafeUtility.SizeOf<T>() * cap;

            var newBuffer = UnsafeUtility.Malloc(size, UnsafeUtility.AlignOf<T>(), m_AllocatorLabel);
            UnsafeUtility.MemCpy(newBuffer, m_Metadata->m_Buffer, m_Metadata->m_Count * UnsafeUtility.SizeOf<T>());

            UnsafeUtility.Free(m_Metadata->m_Buffer, m_AllocatorLabel);
            m_Metadata->m_Buffer = newBuffer;
            m_Metadata->m_Capacity = cap;
        }

        private void Deallocate()
        {
            UnsafeUtility.Free(m_Metadata->m_Buffer, m_AllocatorLabel);
            m_Metadata->m_Buffer = null;
            m_Metadata->m_Capacity = 0;
            m_Metadata->m_Count = 0;

            UnsafeUtility.Free(m_Metadata, m_AllocatorLabel);
            m_Metadata = null;

            m_AllocatorLabel = Allocator.None;
        }

        [WriteAccessRequired]
        public void Dispose()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (!UnsafeUtility.IsValidAllocator(m_AllocatorLabel))
            {
                throw new InvalidOperationException("The NativeList can not be Disposed because it was not allocated with a valid allocator.");
            }

            DisposeSentinel.Dispose(ref m_Safety, ref m_DisposeSentinel);
#endif
            Deallocate();
        }

        public JobHandle Dispose(JobHandle inputDependencies)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Clear(ref m_DisposeSentinel);
#endif
            var jh = new DisposeJob
            {
                m_Container = this
            }.Schedule(inputDependencies);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.Release(m_Safety);
#endif

            m_Metadata = null;
            return jh;
        }

        private struct DisposeJob : IJob
        {
            public NativeList<T> m_Container;
            public void Execute() => m_Container.Deallocate();
        }

        internal int count
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(m_Safety);

                if (m_Metadata == null)
                {
                    throw new InvalidOperationException("NativeList has not been created.");
                }
#endif
                return m_Metadata->m_Count;
            }
        }

        internal int capacity
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(m_Safety);

                if (m_Metadata == null)
                {
                    throw new InvalidOperationException("NativeList has not been created.");
                }
#endif
                return m_Metadata->m_Capacity;
            }
        }

        internal bool isCreated => m_Metadata != null;

        internal T this[int index]
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(m_Safety);

                if (m_Metadata == null)
                {
                    throw new InvalidOperationException("NativeList has not been created.");
                }

                if (index < 0 || index >= m_Metadata->m_Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} is out of range [0, {m_Metadata->m_Count}).");
                }
#endif
                return UnsafeUtility.ReadArrayElement<T>(m_Metadata->m_Buffer, index);
            }
            [WriteAccessRequired] set
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);

                if (m_Metadata == null)
                {
                    throw new InvalidOperationException("NativeList has not been created.");
                }

                if (index < 0 || index >= m_Metadata->m_Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} is out of range [0, {m_Metadata->m_Count}).");
                }
#endif
                UnsafeUtility.WriteArrayElement<T>(m_Metadata->m_Buffer, index, value);
            }
        }

        internal int FirstIndexOf<TEquatable>(TEquatable item) where TEquatable : struct, IEquatable<T>
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);

            if (m_Metadata == null)
            {
                throw new InvalidOperationException("NativeList has not been created.");
            }
#endif
            var c = m_Metadata->m_Count;
            for (var i = 0; i < c; i++)
            {
                var element = UnsafeUtility.ReadArrayElement<T>(m_Metadata->m_Buffer, i);
                if (item.Equals(element))
                {
                    return i;
                }
            }

            return -1;
        }

        internal int LastIndexOf<TEquatable>(TEquatable item) where TEquatable : struct, IEquatable<T>
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);

            if (m_Metadata == null)
            {
                throw new InvalidOperationException("NativeList has not been created.");
            }
#endif
            for (var i = m_Metadata->m_Count - 1; i >= 0; i--)
            {
                var element = UnsafeUtility.ReadArrayElement<T>(m_Metadata->m_Buffer, i);
                if (item.Equals(element))
                {
                    return i;
                }
            }

            return -1;
        }

        internal void Add(T item)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);

            if (m_Metadata == null)
            {
                throw new InvalidOperationException("NativeList has not been created.");
            }
#endif

            EnsureCapacityToInsert(1);

            UnsafeUtility.WriteArrayElement<T>(m_Metadata->m_Buffer, m_Metadata->m_Count, item);
            m_Metadata->m_Count++;
        }

        internal void RemoveFirst<TEquatable>(TEquatable item) where TEquatable : struct, IEquatable<T>
        {
            var index = FirstIndexOf(item);

            if (index != -1)
            {
                RemoveAt(index);
            }
        }

        internal void RemoveLast<TEquatable>(TEquatable item) where TEquatable : struct, IEquatable<T>
        {
            var index = LastIndexOf(item);

            if (index != -1)
            {
                RemoveAt(index);
            }
        }

        internal void InsertAt(int index, T item)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);

            if (m_Metadata == null)
            {
                throw new InvalidOperationException("NativeList has not been created.");
            }

            if (index < 0 || index >= m_Metadata->m_Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} is out of range [0, {m_Metadata->m_Count}).");
            }
#endif

            EnsureCapacityToInsert(1);

            var copySize = m_Metadata->m_Count - index;

            var src = (void*) ((byte*) m_Metadata->m_Buffer + (index * UnsafeUtility.SizeOf<T>()));
            var dst = (void*) ((byte*) m_Metadata->m_Buffer + ((index + 1) * UnsafeUtility.SizeOf<T>()));

            UnsafeUtility.MemMove(dst, src, copySize * UnsafeUtility.SizeOf<T>());

            UnsafeUtility.WriteArrayElement(m_Metadata->m_Buffer, index, item);
            m_Metadata->m_Count++;
        }

        internal void RemoveAt(int index)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);

            if (m_Metadata == null)
            {
                throw new InvalidOperationException("NativeList has not been created.");
            }

            if (index < 0 || index >= m_Metadata->m_Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} is out of range [0, {m_Metadata->m_Count}).");
            }
#endif

            var copySize = m_Metadata->m_Count - index - 1;

            var dst = (void*) ((byte*) m_Metadata->m_Buffer + (index * UnsafeUtility.SizeOf<T>()));
            var src = (void*) ((byte*) m_Metadata->m_Buffer + ((index + 1) * UnsafeUtility.SizeOf<T>()));

            UnsafeUtility.MemMove(dst, src, copySize * UnsafeUtility.SizeOf<T>());

            m_Metadata->m_Count--;
        }

        public bool Equals(NativeList<T> other)
        {
            return m_Metadata == other.m_Metadata;
        }

        public override bool Equals(object obj)
        {
            return obj is NativeList<T> other && Equals(other);
        }

        public static bool operator ==(NativeList<T> left, NativeList<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NativeList<T> left, NativeList<T> right)
        {
            return !left.Equals(right);
        }

        public override int GetHashCode()
        {
            return unchecked((int) (long) m_Metadata);
        }
    }

    internal sealed class NativeListDebugView<T> where T : struct
    {
        private NativeList<T> m_List;

        public NativeListDebugView(NativeList<T> list)
        {
            m_List = list;
        }
    }
}