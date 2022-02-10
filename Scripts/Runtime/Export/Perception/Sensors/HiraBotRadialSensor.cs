using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.AI
{
    public sealed class HiraBotRadialSensor : HiraBotSensor
    {
        [Tooltip("The angle in degrees.")]
        [SerializeField] private float m_Angle = 90f;

        [Tooltip("The height of the sensor.")]
        [SerializeField] private float m_Height = 2f;

        [Tooltip("The offset to the height of the sensor.")]
        [SerializeField] private float m_HeightOffset = -2f;

        [Tooltip("The secondary range of the radial sensor. Can be used for peripheral vision.")]
        [SerializeField] private float m_SecondaryRange = 1f;

        [Tooltip("The secondary angle in degrees. Can be used for peripheral vision.")]
        [SerializeField] private float m_SecondaryAngle = 180f;

        [Tooltip("The height of the sensor.")]
        [SerializeField] private float m_SecondaryHeight = 2f;

        [Tooltip("The offset to the height of the sensor.")]
        [SerializeField] private float m_SecondaryHeightOffset = -2f;

        public float angle
        {
            get => m_Angle;
            set => m_Angle = value;
        }

        public float height
        {
            get => m_Height;
            set => m_Height = value;
        }

        public float heightOffset
        {
            get => m_HeightOffset;
            set => m_HeightOffset = value;
        }

        public float secondaryRange
        {
            get => m_SecondaryRange;
            set => m_SecondaryRange = value;
        }

        public float secondaryAngle
        {
            get => m_SecondaryAngle;
            set => m_SecondaryAngle = value;
        }

        public float secondaryHeight
        {
            get => m_SecondaryHeight;
            set => m_SecondaryHeight = value;
        }

        public float secondaryHeightOffset
        {
            get => m_SecondaryHeightOffset;
            set => m_SecondaryHeightOffset = value;
        }

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
            var forwardVector = t.forward;

            var scale = t.lossyScale;
            var effectiveScale = Mathf.Min(Mathf.Min(scale.x, scale.y), scale.z);
            var effectiveRadius = effectiveScale * range;
            var effectiveSecondaryRadius = effectiveScale * m_SecondaryRange;

            return new BoundsCheckJob(
                    new float4(pos.x, pos.y, pos.z, 1),
                    forwardVector,
                    effectiveRadius,
                    m_Angle,
                    m_Height,
                    m_HeightOffset,
                    effectiveSecondaryRadius,
                    m_SecondaryAngle,
                    m_SecondaryHeight,
                    m_SecondaryHeightOffset,
                    stimuliCount,
                    stimuliPositions,
                    stimuliAssociatedObjects,
                    perceivedObjectsList,
                    perceivedObjectsLocationsList
                )
                .Schedule();
        }

        [BurstCompile]
        private struct BoundsCheckJob : IJob
        {
            internal BoundsCheckJob(
                float4 sensorPosition,
                float3 sensorDirection,
                float range,
                float angleInDegrees,
                float height,
                float heightOffset,
                float secondaryRange,
                float secondaryAngleInDegrees,
                float secondaryHeight,
                float secondaryHeightOffset,
                int stimuliCount,
                NativeArray<float4> stimuliPositions,
                NativeArray<int> stimuliAssociatedObjects,
                PerceivedObjectsList perceivedObjectsList,
                PerceivedObjectsLocationsList perceivedObjectsLocationsList)
            {
                m_SensorPosition = sensorPosition;
                m_SensorDirection = sensorDirection;
                m_Range = range;
                m_AngleInDegrees = angleInDegrees;
                m_Height = height;
                m_HeightOffset = heightOffset;
                m_SecondaryRange = secondaryRange;
                m_SecondaryAngleInDegrees = secondaryAngleInDegrees;
                m_SecondaryHeight = secondaryHeight;
                m_SecondaryHeightOffset = secondaryHeightOffset;
                m_StimuliCount = stimuliCount;
                m_StimuliPositions = stimuliPositions;
                m_StimuliAssociatedObjects = stimuliAssociatedObjects;
                m_PerceivedObjectsList = perceivedObjectsList;
                m_PerceivedObjectsLocationsList = perceivedObjectsLocationsList;
            }

            [ReadOnly] private readonly float4 m_SensorPosition;
            [ReadOnly] private readonly float3 m_SensorDirection;
            [ReadOnly] private readonly float m_Range;
            [ReadOnly] private readonly float m_AngleInDegrees;
            [ReadOnly] private readonly float m_Height;
            [ReadOnly] private readonly float m_HeightOffset;
            [ReadOnly] private readonly float m_SecondaryRange;
            [ReadOnly] private readonly float m_SecondaryAngleInDegrees;
            [ReadOnly] private readonly float m_SecondaryHeight;
            [ReadOnly] private readonly float m_SecondaryHeightOffset;
            [ReadOnly] private readonly int m_StimuliCount;
            [ReadOnly] private NativeArray<float4> m_StimuliPositions;
            [ReadOnly] private readonly NativeArray<int> m_StimuliAssociatedObjects;
            private PerceivedObjectsList m_PerceivedObjectsList;
            private PerceivedObjectsLocationsList m_PerceivedObjectsLocationsList;

            public unsafe void Execute()
            {
                var vectorizedPositions = m_StimuliPositions.Reinterpret<float4x4>(sizeof(float4));
                var vectorizedLength = vectorizedPositions.Length;
                var vectorizedResults = stackalloc bool4[vectorizedLength];

                PrimaryCheck(vectorizedLength, vectorizedPositions, vectorizedResults);
                SecondaryCheck(vectorizedLength, vectorizedPositions, vectorizedResults);

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

            private unsafe void PrimaryCheck(int vectorizedLength, NativeArray<float4x4> vectorizedPositions, bool4* results)
            {
                // clear
                {
                    for (var i = 0; i < vectorizedLength; i++)
                    {
                        results[i] = true;
                    }
                }

                HeightCheck(vectorizedLength, vectorizedPositions, results, m_Height, m_HeightOffset);
                AngleCheck(vectorizedLength, vectorizedPositions, results, m_AngleInDegrees);
                RangeCheck(vectorizedLength, vectorizedPositions, results, m_Range);
            }

            private unsafe void SecondaryCheck(int vectorizedLength, NativeArray<float4x4> vectorizedPositions, bool4* originalResults)
            {
                var secondaryVectorizedResults = stackalloc bool4[vectorizedLength];

                // clear
                {
                    for (var i = 0; i < vectorizedLength; i++)
                    {
                        secondaryVectorizedResults[i] = true;
                    }
                }

                HeightCheck(vectorizedLength, vectorizedPositions, secondaryVectorizedResults, m_SecondaryHeight, m_SecondaryHeightOffset);
                AngleCheck(vectorizedLength, vectorizedPositions, secondaryVectorizedResults, m_SecondaryAngleInDegrees);
                RangeCheck(vectorizedLength, vectorizedPositions, secondaryVectorizedResults, m_SecondaryRange);

                for (var i = 0; i < vectorizedLength; i++)
                {
                    originalResults[i] |= secondaryVectorizedResults[i];
                }
            }

            private unsafe void HeightCheck(int vectorizedLength, NativeArray<float4x4> vectorizedPositions, bool4* vectorizedResults, float height, float heightOffset)
            {
                float4 heightMin4 = m_SensorPosition.y + heightOffset - (height * 0.5f);
                float4 heightMax4 = m_SensorPosition.y + heightOffset + (height * 0.5f);

                for (var i = 0; i < vectorizedLength; i++)
                {
                    var stimuliPos4 = vectorizedPositions[i];

                    float4 stimuliHeight4 = default;
                    for (var j = 0; j < 4; j++)
                    {
                        stimuliHeight4[j] = stimuliPos4[j].y;
                    }

                    vectorizedResults[i] &= (stimuliHeight4 < heightMax4) & (stimuliHeight4 > heightMin4);
                }
            }

            private unsafe void AngleCheck(int vectorizedLength, NativeArray<float4x4> vectorizedPositions, bool4* vectorizedResults, float angleInDegrees)
            {
                var angleInRadians = math.radians(angleInDegrees);
                var halfAngleInRadians = angleInRadians * 0.5f;
                var cosHalfAngleInRadians = math.cos(halfAngleInRadians);
                var dotProductThreshold = new float4(cosHalfAngleInRadians);

                var sensorPos4 = new float4x4(m_SensorPosition, m_SensorPosition, m_SensorPosition, m_SensorPosition);

                var sensorDirectionXZ = m_SensorDirection.xz;
                var normalizedSensorDirectionXZ = math.normalizesafe(sensorDirectionXZ, new float2(0, 1));
                var normalizedSensorDirectionXZ4 = new float2x4(normalizedSensorDirectionXZ, normalizedSensorDirectionXZ, normalizedSensorDirectionXZ, normalizedSensorDirectionXZ);

                for (var i = 0; i < vectorizedLength; i++)
                {
                    var stimuliPos4 = vectorizedPositions[i];

                    var sensorToStimulusVector4 = stimuliPos4 - sensorPos4;

                    float2x4 normalizedSensorToStimulusVectorXZ4 = default;
                    for (var j = 0; j < 4; j++)
                    {
                        normalizedSensorToStimulusVectorXZ4[j] = math.normalizesafe(sensorToStimulusVector4[j].xz, normalizedSensorDirectionXZ4[j]);
                    }

                    float4 dotProduct = default;
                    for (var j = 0; j < 4; j++)
                    {
                        dotProduct[j] = math.dot(normalizedSensorDirectionXZ4[j], normalizedSensorToStimulusVectorXZ4[j]);
                    }

                    vectorizedResults[i] &= (dotProduct > dotProductThreshold);
                }
            }

            private unsafe void RangeCheck(int vectorizedLength, NativeArray<float4x4> vectorizedPositions, bool4* vectorizedResults, float range)
            {
                var rangeSq4 = new float4(range);

                for (var i = 0; i < vectorizedLength; i++)
                {
                    var stimuliPosition4 = vectorizedPositions[i];

                    float4 distanceXZSq4 = default;
                    for (var j = 0; j < 4; j++)
                    {
                        distanceXZSq4[j] = math.distancesq(m_SensorPosition.xz, stimuliPosition4[j].xz);
                    }

                    vectorizedResults[i] &= (distanceXZSq4 < rangeSq4);
                }
            }
        }
    }
}