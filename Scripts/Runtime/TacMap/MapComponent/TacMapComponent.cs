using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace HiraBots
{
    internal partial class TacMapComponent
    {
        private static ulong s_Id = 0;

        private readonly ulong m_Id;

        private NativeArray<float> m_Internal;

        private JobHandle? m_ActiveReadJob;
        private JobHandle? m_ActiveWriteJob;

        private Coroutine m_ReadJobTrackerCoroutine;
        private Coroutine m_WriteJobTrackerCoroutine;

        private int3 m_Pivot;
        private int3 m_Dimensions;

        /// <summary>
        /// Reset the static id assigner.
        /// </summary>
        internal static void ResetStaticIDAssigner()
        {
            s_Id = 0;
        }

        internal static bool TryCreate(Transform t, float cellSize, out TacMapComponent component)
        {
            if (t == null)
            {
                component = null;
                return false;
            }

            if (cellSize == 0f)
            {
                component = null;
                return false;
            }

            var (pivot, dimensions) = TacMapUtility.TransformToOffsetWBounds(t.localToWorldMatrix, cellSize);

            // none should be negative or zero
            if (dimensions.x * dimensions.y <= 0 || dimensions.y * dimensions.z <= 0)
            {
                component = null;
                return false;
            }

            component = new TacMapComponent(pivot, dimensions);
            return true;
        }

        private TacMapComponent(int3 pivot, int3 dimensions)
        {
            m_Pivot = pivot;
            m_Dimensions = dimensions;

            m_Internal = new NativeArray<float>(dimensions.x * dimensions.y * dimensions.z, Allocator.Persistent);

            m_ActiveWriteJob = null;
            m_ActiveReadJob = default;

            m_WriteJobTrackerCoroutine = null;
            m_ReadJobTrackerCoroutine = null;

            m_Id = ++s_Id;
        }

        internal void Dispose()
        {
            if (m_ReadJobTrackerCoroutine != null)
            {
                CoroutineRunner.Stop(m_ReadJobTrackerCoroutine);
                m_ReadJobTrackerCoroutine = null;
            }

            if (m_WriteJobTrackerCoroutine != null)
            {
                CoroutineRunner.Stop(m_WriteJobTrackerCoroutine);
                m_WriteJobTrackerCoroutine = null;
            }

            m_ActiveReadJob?.Complete();
            m_ActiveWriteJob?.Complete();

            m_ActiveReadJob = null;
            m_ActiveWriteJob = null;

            m_Internal.Dispose();
            m_Internal = default;

            m_Dimensions = int3.zero;
            m_Pivot = int3.zero;
        }
    }
}