using Unity.Collections;

namespace HiraBots
{
    internal struct PlannerResult
    {
        private const short k_CountIndex = 0;
        private const short k_CurrentIndexIndex = 1;
        private const short k_HeaderSize = 2;

        private NativeArray<short> m_Internal;

        internal PlannerResult(short bufferSize, Allocator allocator)
        {
            m_Internal = new NativeArray<short>(bufferSize + k_HeaderSize, allocator, NativeArrayOptions.UninitializedMemory);
            count = 0;
            InvalidatePlan();
        }

        internal void Dispose()
        {
            if (m_Internal.IsCreated)
            {
                m_Internal.Dispose();
            }
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
    }
}