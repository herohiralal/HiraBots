using Unity.Collections;

namespace HiraBots
{
    internal struct LGOAPPlan
    {
        internal struct ReadOnly
        {
            [ReadOnly] private NativeArray<short> m_Internal;

            internal ReadOnly(short[] plan)
            {
                m_Internal = new NativeArray<short>(plan, Allocator.Persistent);
            }

            internal void Dispose()
            {
                if (m_Internal.IsCreated)
                {
                    m_Internal.Dispose();
                }
            }

            internal short length => (short) m_Internal.Length;

            internal short this[int index]
            {
                get => m_Internal[index];
                set => m_Internal[index] = value;
            }

            internal void CopyTo(LGOAPPlan dst)
            {
                var selfLength = length;

                NativeArray<short>.Copy(m_Internal, 0, dst.m_Internal, k_HeaderSize, selfLength);
                dst.length = selfLength;
                dst.RestartPlan();
            }
        }

        internal enum Type : short
        {
            Invalid = 0,
            NotRequired = 1,
            Unchanged = 2,
            NewPlan = 3
        }

        private const short k_TypeIndex = 0;
        private const short k_CountIndex = 1;
        private const short k_CurrentIndexIndex = 2;
        private const short k_HeaderSize = 3;

        private NativeArray<short> m_Internal;

        internal LGOAPPlan(short bufferSize, Allocator allocator)
        {
            m_Internal = new NativeArray<short>(bufferSize + k_HeaderSize, allocator, NativeArrayOptions.UninitializedMemory);
            InvalidatePlan();
        }

        internal void Dispose()
        {
            if (m_Internal.IsCreated)
            {
                m_Internal.Dispose();
            }
        }

        internal Type resultType
        {
            get => (Type) m_Internal[k_TypeIndex];
            set => m_Internal[k_TypeIndex] = (short) value;
        }

        internal short length
        {
            get => m_Internal[k_CountIndex];
            set => m_Internal[k_CountIndex] = value;
        }

        internal short maxLength => (byte) (m_Internal.Length - k_HeaderSize);

        internal short this[short index]
        {
            get => m_Internal[index + k_HeaderSize];
            set => m_Internal[index + k_HeaderSize] = value;
        }

        internal void CopyTo(LGOAPPlan dst)
        {
            m_Internal.CopyTo(dst.m_Internal);
        }

        internal void InvalidatePlan()
        {
            resultType = Type.Invalid;
            length = 0;
            currentIndex = -1;
        }

        internal void RestartPlan()
        {
            currentIndex = -1;
        }

        internal short currentIndex
        {
            get => m_Internal[k_CurrentIndexIndex];
            set => m_Internal[k_CurrentIndexIndex] = value;
        }

        internal bool canMoveNext => (currentIndex + 1) < length;

        internal void MoveNext() => currentIndex++;

        internal short currentElement => this[currentIndex];
    }
}