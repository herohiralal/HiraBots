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
    internal unsafe struct NativeList<T> : IDisposable where T : struct
    {
        [NativeDisableUnsafePtrRestriction] private NativeListMetadata* m_Metadata;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        private AtomicSafetyHandle m_Safety;
        private DisposeSentinel m_DisposeSentinel;
#endif

        private Allocator m_AllocatorLabel;

        internal NativeList(int capacity, Allocator allocator)
        {
            Allocate(capacity, allocator, out this);
        }

        internal NativeList(NativeArray<T> array, Allocator allocator)
        {
            var length = array.Length;

            Allocate(length, allocator, out this);

            var src = array.GetUnsafeReadOnlyPtr();
            var dst = m_Metadata->m_Buffer;

            UnsafeUtility.MemCpy(src, dst, UnsafeUtility.SizeOf<T>() * length);
            m_Metadata->m_Count = length;
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