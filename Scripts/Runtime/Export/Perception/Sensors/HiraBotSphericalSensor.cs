using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.AI
{
    public sealed class HiraBotSphericalSensor : HiraBotSensor
    {
        protected override JobHandle ScheduleBoundsCheckJob(NativeArray<float4x4> stimuliPositions, NativeArray<bool4> results)
        {
            var t = transform;

            var pos = (float3) t.position;

            var scale = t.lossyScale;
            var effectiveScale = Mathf.Min(Mathf.Min(scale.x, scale.y), scale.z);
            var effectiveRadius = effectiveScale * range;

            return new BoundsCheckJob(
                    new float4(pos.x, pos.y, pos.z, 1),
                    effectiveRadius,
                    stimuliPositions,
                    results)
                .Schedule();
        }

        [BurstCompile]
        private struct BoundsCheckJob : IJob
        {
            internal BoundsCheckJob(float4 sensorPosition, float range, NativeArray<float4x4> stimuliPositions, NativeArray<bool4> results)
            {
                m_SensorPosition = sensorPosition;
                m_Range = range;
                m_StimuliPositions = stimuliPositions;
                m_Results = results;
            }

            [ReadOnly] private readonly float4 m_SensorPosition;
            [ReadOnly] private readonly float m_Range;
            [ReadOnly] private readonly NativeArray<float4x4> m_StimuliPositions;
            [WriteOnly] private NativeArray<bool4> m_Results;

            public void Execute()
            {
                var rangeSq4 = new float4(m_Range * m_Range);

                var length = m_StimuliPositions.Length;

                for (var i = 0; i < length; i++)
                {
                    var stimuliPosition4 = m_StimuliPositions[i];

                    float4 distanceSq4 = default;
                    for (var j = 0; j < 4; j++)
                    {
                        distanceSq4[j] = math.distancesq(m_SensorPosition, stimuliPosition4[j]);
                    }

                    m_Results[i] = distanceSq4 < rangeSq4;
                }
            }
        }
    }
}