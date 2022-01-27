using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace HiraBots
{
    public sealed class SphericalSensorComponent : SensorComponent
    {
        [Space, Header("Shape")]
        [Tooltip("The radius of the sensor.")]
        [SerializeField] private float m_Radius;

        public override JobHandle ScheduleBoundsCheckJob(NativeArray<float3x4> stimuliPositions, NativeArray<float4> scores)
        {
            return new BoundsCheckJob(
                    transform.position,
                    m_Radius,
                    stimuliPositions,
                    scores)
                .Schedule();
        }

        [BurstCompile]
        private struct BoundsCheckJob : IJob
        {
            internal BoundsCheckJob(float3 sensorPosition, float range, NativeArray<float3x4> stimuliPositions, NativeArray<float4> scores)
            {
                m_SensorPosition = sensorPosition;
                m_Range = range;
                m_StimuliPositions = stimuliPositions;
                m_Scores = scores;
            }

            [ReadOnly] private readonly float3 m_SensorPosition;
            [ReadOnly] private readonly float m_Range;
            [ReadOnly] private readonly NativeArray<float3x4> m_StimuliPositions;
            [WriteOnly] private NativeArray<float4> m_Scores;

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
                        distanceSq4[i] = math.distancesq(m_SensorPosition, stimuliPosition4[i]);
                    }

                    // note that dividing squared distances will give an INCREASINGLY higher score
                    // as stimuli get closer, if this is a problem, change it here
                    var normalizedScore4 = 1 - (distanceSq4 / rangeSq4);
                    m_Scores[i] = math.select(normalizedScore4, -1f, normalizedScore4 < 0); // use -1 if outside the range
                }
            }
        }
    }
}