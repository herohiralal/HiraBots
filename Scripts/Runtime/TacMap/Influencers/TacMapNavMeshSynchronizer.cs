using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Experimental.AI;

namespace HiraBots
{
    internal static class TacMapNavMeshSynchronizer
    {
        internal static JobHandle Schedule(TacMapComponent map, NavMeshQuery q,
            int batchCount = 32, int agentType = 0, int areaMask = UnityEngine.AI.NavMesh.AllAreas)
        {
            if (map == null)
            {
                return default;
            }

            return map.RequestDataForWriteJob(((m, p, d, s, dependencies) =>
                new Job(q, m.Reinterpret<float4>(sizeof(float)), p, d, s, agentType, areaMask)
                    .ScheduleParallel(m.Length / 4, batchCount, dependencies)));
        }

        internal static void Run(TacMapComponent map, int agentType = 0, int areaMask = UnityEngine.AI.NavMesh.AllAreas)
        {
            map?.RequestDataForWriteJob((m, p, d, s) =>
            {
                var q = new NavMeshQuery(NavMeshWorld.GetDefaultWorld(), Allocator.TempJob);
                new Job(q, m.Reinterpret<float4>(sizeof(float)), p, d, s, agentType, areaMask)
                    .Run(m.Length / 4);
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

            [ReadOnly] private NavMeshQuery m_NavMeshQuery;
            private NativeArray<float4> m_Map;
            [ReadOnly] private readonly int3 m_Pivot;
            [ReadOnly] private readonly int3 m_Dimensions;
            [ReadOnly] private readonly float m_CellSize;
            [ReadOnly] private readonly int m_AgentType;
            [ReadOnly] private readonly int m_AreaMask;

            public void Execute(int index)
            {
                var height = (index * 4) / (m_Dimensions.x * m_Dimensions.z);
                var row = ((index * 4) / m_Dimensions.x) % m_Dimensions.z;
                var column = (index * 4) % m_Dimensions.x;

                var individualOffsetCoordinatesL = new int4x3(
                    new int4(column + 0, column + 1, column + 2, column + 3),
                    height,
                    row);

                var individualOffsetCoordinatesW = new int4x3(m_Pivot.x, m_Pivot.y, m_Pivot.z) + individualOffsetCoordinatesL;

                var linearPositionsW = TacMapUtility.IndividualOffsetsWToLinearPositionsW(individualOffsetCoordinatesW, m_CellSize * 0.5f);

                var positionsW = math.transpose(linearPositionsW);

                bool4 mask = default;

                for (var i = 0; i < 4; i++)
                {
                    mask[i] = m_NavMeshQuery.IsValid(m_NavMeshQuery.MapLocation(
                        positionsW[i], new float3(m_CellSize * 0.5f), m_AgentType, m_AreaMask));
                }

                m_Map[index] *= math.select(0f, 1f, mask);
            }
        }
    }
}