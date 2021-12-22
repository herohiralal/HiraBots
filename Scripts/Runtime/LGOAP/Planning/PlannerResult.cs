using Unity.Collections;

namespace HiraBots
{
    internal struct PlannerResult
    {
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

        internal PlannerResult(short bufferSize, Allocator allocator)
        {
            m_Internal = new NativeArray<short>(bufferSize + k_HeaderSize, allocator, NativeArrayOptions.UninitializedMemory);
            resultType = Type.Invalid;
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

        internal short count
        {
            get => m_Internal[k_CountIndex];
            set => m_Internal[k_CountIndex] = value;
        }

        internal short bufferSize => (byte) (m_Internal.Length - k_HeaderSize);

        internal short currentIndex
        {
            get => m_Internal[k_CurrentIndexIndex];
            set => m_Internal[k_CurrentIndexIndex] = value;
        }

        internal short this[short index]
        {
            get => m_Internal[index + k_HeaderSize];
            set => m_Internal[index + k_HeaderSize] = value;
        }

        internal void InvalidatePlan()
        {
            count = 0;
            currentIndex = short.MaxValue;
        }

        internal void RestartPlan()
        {
            currentIndex = -1;
        }

        internal bool MoveNext()
        {
            currentIndex++;
            return currentIndex < count;
        }

        internal short currentElement => this[currentIndex];

        internal void CopyTo(PlannerResult other)
        {
            m_Internal.CopyTo(other.m_Internal);
        }
    }
}