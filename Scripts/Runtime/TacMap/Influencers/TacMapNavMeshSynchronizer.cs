using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Experimental.AI;

namespace HiraBots
{
    internal static class TacMapNavMeshSynchronizer
    {
        internal static JobHandle SynchronizeAsync(TacMapComponent map, NavMeshQuery q,
            int batchCount = 32, int agentType = 0, int areaMask = UnityEngine.AI.NavMesh.AllAreas)
        {
            if (map == null)
            {
                return default;
            }

            return map.RequestDataForWriteJob(((m, p, d, s, dependencies) =>
                new Job(q, m, p, d, s, agentType, areaMask)
                    .ScheduleParallel(m.Length, batchCount, dependencies)));
        }

        internal static void Synchronize(TacMapComponent map, int agentType = 0, int areaMask = UnityEngine.AI.NavMesh.AllAreas)
        {
            map?.RequestDataForWriteJob((m, p, d, s) =>
            {
                var q = new NavMeshQuery(NavMeshWorld.GetDefaultWorld(), Allocator.TempJob);
                new Job(q, m, p, d, s, agentType, areaMask)
                    .Run(m.Length);
                q.Dispose();
            });
        }

        [BurstCompile]
        private struct Job : IJobFor
        {
            public Job(NavMeshQuery navMeshQuery, NativeArray<float4> map,
                int3 pivot, int3 dimensions, float cellSize,
                int agentType, int areaMask)
            {
                m_NavMeshQuery = navMeshQuery;
                m_Map = map;
                m_Pivot = pivot;
                m_Dimensions = dimensions;
                m_CellSize = cellSize;
                m_AgentType = agentType;
                m_AreaMask = areaMask;
            }

            private NavMeshQuery m_NavMeshQuery;
            private NativeArray<float4> m_Map;
            [ReadOnly] private readonly int3 m_Pivot;
            [ReadOnly] private readonly int3 m_Dimensions;
            [ReadOnly] private readonly float m_CellSize;
            [ReadOnly] private readonly int m_AgentType;
            [ReadOnly] private readonly int m_AreaMask;

            public void Execute(int index)
            {
                var height = index / (m_Dimensions.x * m_Dimensions.z);
                var row = index / m_Dimensions.x;
                var column = index % m_Dimensions.x;

                var offsetCoordinatesL = new int3x4(
                    new int3(column + 0, height, row),
                    new int3(column + 1, height, row),
                    new int3(column + 2, height, row),
                    new int3(column + 3, height, row));

                var offsetCoordinatesW = new int3x4(m_Pivot, m_Pivot, m_Pivot, m_Pivot) + offsetCoordinatesL;

                var positionsW = TacMapUtility.OffsetWToPositionW(offsetCoordinatesW, m_CellSize * 0.5f);

                float4 mask = default;

                for (var i = 0; i < 4; i++)
                {
                    var loc = m_NavMeshQuery.MapLocation(
                        positionsW[i], new float3(m_CellSize * 0.5f), m_AgentType, m_AreaMask);

                    mask[i] = m_NavMeshQuery.IsValid(loc) ? 1f : 0f;
                }

                m_Map[index] *= mask;
            }
        }
    }
}