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

        private JobHandle m_LastReadJob;
        private JobHandle? m_LastWriteJob;

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

            if (dimensions.x * dimensions.y * dimensions.z == 0)
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

            m_LastWriteJob = null;
            m_LastReadJob = default;

            m_Id = ++s_Id;
        }

        internal void Dispose()
        {
            m_LastReadJob.Complete();
            m_LastWriteJob?.Complete();

            m_LastWriteJob = null;
            m_LastReadJob = default;

            m_Internal.Dispose();
            m_Internal = default;
        }
    }
}