using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.AI
{
    [AddComponentMenu("AI/HiraBot Sensor (Spherical)")]
    public sealed class HiraBotSphericalSensor : HiraBotSensor
    {
        [Space] [Header("Shape")]
        [Tooltip("The maximum range of the sensor.")]
        [SerializeField] private float m_Radius = 8f;

        public float radius
        {
            get => m_Radius;
            set => m_Radius = Mathf.Clamp(value, 0f, float.MaxValue);
        }

        protected override JobHandle ScheduleBoundsCheckJob(
            NativeArray<float4> stimuliPositions,
            NativeArray<int> stimuliAssociatedObjects,
            PerceivedObjectsList perceivedObjectsList,
            PerceivedObjectsLocationsList perceivedObjectsLocationsList,
            int stimuliCount,
            JobHandle dependencies)
        {
            return new BoundsCheckJob(
                    transform.worldToLocalMatrix,
                    m_Radius,
                    stimuliCount,
                    stimuliPositions,
                    stimuliAssociatedObjects,
                    perceivedObjectsList,
                    perceivedObjectsLocationsList)
                .Schedule(dependencies);
        }

        private void OnDrawGizmosSelected()
        {
            var originalMatrix = Gizmos.matrix;
            try
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireSphere(Vector3.zero, m_Radius);
            }
            finally
            {
                Gizmos.matrix = originalMatrix;
            }
        }

        [BurstCompile]
        private struct BoundsCheckJob : IJob
        {
            internal BoundsCheckJob(
                float4x4 sensorW2L,
                float range,
                int stimuliCount,
                NativeArray<float4> stimuliPositions,
                NativeArray<int> stimuliAssociatedObjects,
                PerceivedObjectsList perceivedObjectsList,
                PerceivedObjectsLocationsList perceivedObjectsLocationsList)
            {
                m_SensorW2L = sensorW2L;
                m_Range = range;
                m_StimuliCount = stimuliCount;
                m_StimuliPositions = stimuliPositions;
                m_StimuliAssociatedObjects = stimuliAssociatedObjects;
                m_PerceivedObjectsList = perceivedObjectsList;
                m_PerceivedObjectsLocationsList = perceivedObjectsLocationsList;
            }

            [ReadOnly] private readonly float4x4 m_SensorW2L;
            [ReadOnly] private readonly float m_Range;
            [ReadOnly] private readonly int m_StimuliCount;
            [ReadOnly] private NativeArray<float4> m_StimuliPositions;
            [ReadOnly] private readonly NativeArray<int> m_StimuliAssociatedObjects;
            private PerceivedObjectsList m_PerceivedObjectsList;
            private PerceivedObjectsLocationsList m_PerceivedObjectsLocationsList;

            public unsafe void Execute()
            {
                var rangeSq4 = new float4(m_Range * m_Range);

                var vectorizedPositions = m_StimuliPositions.Reinterpret<float4x4>(sizeof(float4));
                var vectorizedLength = vectorizedPositions.Length;
                var vectorizedResults = stackalloc bool4[vectorizedLength];

                var localizedPositions = (float4x4*) UnsafeUtility.Malloc(UnsafeUtility.SizeOf<float4x4>() * vectorizedLength,
                    UnsafeUtility.AlignOf<float4x4>(),
                    Allocator.Persistent);
                {
                    // calculate localized positions
                    {
                        for (var i = 0; i < vectorizedLength; i++)
                        {
                            localizedPositions[i] = math.mul(m_SensorW2L, vectorizedPositions[i]);
                        }
                    }

                    for (var i = 0; i < vectorizedLength; i++)
                    {
                        var stimuliLocalPosition4 = localizedPositions[i];

                        float4 lengthSq4 = default;
                        for (var j = 0; j < 4; j++)
                        {
                            lengthSq4[j] = math.lengthsq(stimuliLocalPosition4[j].xyz);
                        }

                        vectorizedResults[i] = (lengthSq4 < rangeSq4);
                    }
                }

                UnsafeUtility.Free(localizedPositions, Allocator.Persistent);

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