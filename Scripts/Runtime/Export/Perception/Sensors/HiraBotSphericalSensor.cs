﻿using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.AI
{
    public sealed class HiraBotSphericalSensor : HiraBotSensor
    {
        protected override JobHandle ScheduleBoundsCheckJob(
            NativeArray<float4> stimuliPositions,
            NativeArray<int> stimuliAssociatedObjects,
            PerceivedObjectsList perceivedObjectsList,
            PerceivedObjectsLocationsList perceivedObjectsLocationsList,
            int stimuliCount,
            JobHandle dependencies)
        {
            var t = transform;

            var pos = (float3) t.position;

            var scale = t.lossyScale;
            var effectiveScale = Mathf.Min(Mathf.Min(scale.x, scale.y), scale.z);
            var effectiveRadius = effectiveScale * range;

            return new BoundsCheckJob(
                    new float4(pos.x, pos.y, pos.z, 1),
                    effectiveRadius,
                    stimuliCount,
                    stimuliPositions,
                    stimuliAssociatedObjects,
                    perceivedObjectsList,
                    perceivedObjectsLocationsList)
                .Schedule(dependencies);
        }

        [BurstCompile]
        private struct BoundsCheckJob : IJob
        {
            internal BoundsCheckJob(
                float4 sensorPosition,
                float range,
                int stimuliCount,
                NativeArray<float4> stimuliPositions,
                NativeArray<int> stimuliAssociatedObjects,
                PerceivedObjectsList perceivedObjectsList,
                PerceivedObjectsLocationsList perceivedObjectsLocationsList)
            {
                m_SensorPosition = sensorPosition;
                m_Range = range;
                m_StimuliCount = stimuliCount;
                m_StimuliPositions = stimuliPositions;
                m_StimuliAssociatedObjects = stimuliAssociatedObjects;
                m_PerceivedObjectsList = perceivedObjectsList;
                m_PerceivedObjectsLocationsList = perceivedObjectsLocationsList;
            }

            [ReadOnly] private readonly float4 m_SensorPosition;
            [ReadOnly] private readonly float m_Range;
            [ReadOnly] private readonly int m_StimuliCount;
            [ReadOnly] private NativeArray<float4> m_StimuliPositions;
            [ReadOnly] private readonly NativeArray<int> m_StimuliAssociatedObjects;
            private PerceivedObjectsList m_PerceivedObjectsList;
            private PerceivedObjectsLocationsList m_PerceivedObjectsLocationsList;

            public unsafe void Execute()
            {
                var rangeSq4 = new float4(m_Range * m_Range);

                var length = m_StimuliPositions.Length;

                var vectorizedResults = stackalloc bool4[length];
                var vectorizedPositions = m_StimuliPositions.Reinterpret<float4x4>(sizeof(float4));

                for (var i = 0; i < length; i++)
                {
                    var stimuliPosition4 = vectorizedPositions[i];

                    float4 distanceSq4 = default;
                    for (var j = 0; j < 4; j++)
                    {
                        distanceSq4[j] = math.distancesq(m_SensorPosition, stimuliPosition4[j]);
                    }

                    vectorizedResults[i] = distanceSq4 < rangeSq4;
                }

                var results = (bool*) vectorizedResults;

                for (var i = 0; i < m_StimuliCount; i++)
                {
                    if (results[i])
                    {
                        m_PerceivedObjectsList.Add(m_StimuliAssociatedObjects[i]);
                    }
                }

                if (!m_PerceivedObjectsLocationsList.isValid)
                {
                    return;
                }

                for (var i = 0; i < m_StimuliCount; i++)
                {
                    if (results[i])
                    {
                        m_PerceivedObjectsLocationsList.Add(m_StimuliPositions[i]);
                    }
                }
            }
        }
    }
}