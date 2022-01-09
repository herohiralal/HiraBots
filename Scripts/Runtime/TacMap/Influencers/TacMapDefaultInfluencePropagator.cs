using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace HiraBots
{
    internal static class TacMapDefaultInfluencePropagator
    {
        internal static JobHandle Schedule(TacMapComponent map, int3 locationAxialW, NativeArray<float> manhattanDistanceToInfluence, float magnitude = 1f,
            int batchCount = 1024)
        {
            if (map == null)
            {
                return default;
            }

            return map.RequestDataForWriteJob(((m, p, d, s, dependencies) =>
                new Job(m.Reinterpret<float4>(sizeof(float)),
                        p, d,
                        locationAxialW, magnitude, manhattanDistanceToInfluence)
                    .ScheduleParallel(m.Length / 4, batchCount, dependencies)));
        }

        internal static void Run(TacMapComponent map, int3 locationAxialW, NativeArray<float> manhattanDistanceToInfluence, float magnitude = 1f)
        {
            map?.RequestDataForWriteJob(((m, p, d, s) =>
                new Job(m.Reinterpret<float4>(sizeof(float)),
                        p, d,
                        locationAxialW, magnitude, manhattanDistanceToInfluence)
                    .Run(m.Length / 4)));
        }

        [BurstCompile]
        private struct Job : IJobFor
        {
            internal Job(NativeArray<float4> map,
                int3 pivot, int3 dimensions,
                int3 influencerLocationAxialW, float influenceMagnitude, NativeArray<float> manhattanDistanceToInfluence)
            {
                m_Map = map;
                m_Pivot = pivot;
                m_Dimensions = dimensions;
                m_InfluencerLocationAxialW = influencerLocationAxialW;
                m_InfluenceMagnitude = influenceMagnitude;
                m_ManhattanDistanceToInfluence = manhattanDistanceToInfluence;
            }

            private NativeArray<float4> m_Map;

            // map data
            [ReadOnly] private readonly int3 m_Pivot;
            [ReadOnly] private readonly int3 m_Dimensions;

            // influencer data
            [ReadOnly] private readonly int3 m_InfluencerLocationAxialW;
            [ReadOnly] private readonly float m_InfluenceMagnitude;
            [ReadOnly] private readonly NativeArray<float> m_ManhattanDistanceToInfluence;

            public void Execute(int index)
            {
                var height = (index * 4) / (m_Dimensions.x * m_Dimensions.z);
                var row = (index * 4) / m_Dimensions.x;
                var column = (index * 4) % m_Dimensions.x;

                var individualOffsetsL = new int4x3(
                    new int4(column + 0, column + 1, column + 2, column + 3),
                    new int4(height, height, height, height),
                    new int4(row, row, row, row));

                var individualOffsetsW = new int4x3(m_Pivot.x, m_Pivot.y, m_Pivot.z) + individualOffsetsL;

                var individualAxesW = TacMapUtility.IndividualOffsetsWToIndividualAxesW(individualOffsetsW);

                var axialDifferences = individualAxesW - new int4x3(m_InfluencerLocationAxialW.x,
                    m_InfluencerLocationAxialW.y, m_InfluencerLocationAxialW.z);

                var manhattanLengths = TacMapUtility.ManhattanLength(axialDifferences);
                manhattanLengths = math.clamp(manhattanLengths, 0, m_ManhattanDistanceToInfluence.Length - 1);

                float4 influence = default;

                for (var i = 0; i < 4; i++)
                {
                    influence[i] = m_ManhattanDistanceToInfluence[manhattanLengths[i]];
                }

                m_Map[index] += influence * m_InfluenceMagnitude;
            }
        }
    }
}