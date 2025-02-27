﻿using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.AI
{
    [AddComponentMenu("AI/HiraBot Sensor (Radial)")]
    public class HiraBotRadialSensor : HiraBotSensor
    {
        [Space] [Header("Shape")]
        [Tooltip("The maximum range of the sensor.")]
        [SerializeField] private float m_Range = 8f;

        [Tooltip("The angle in degrees.")]
        [SerializeField] [Range(0.1f, 360f)] private float m_Angle = 90f;

        [Tooltip("The height of the sensor.")]
        [SerializeField] private float m_Height = 2f;

        [Tooltip("The offset to the height of the sensor.")]
        [SerializeField] private float m_HeightOffset = -2f;

        [Tooltip("The secondary range of the radial sensor. Can be used for peripheral vision.")]
        [SerializeField] private float m_SecondaryRange = 1f;

        [Tooltip("The secondary angle in degrees. Can be used for peripheral vision.")]
        [SerializeField] [Range(0.1f, 360f)] private float m_SecondaryAngle = 180f;

        [Tooltip("The height of the sensor.")]
        [SerializeField] private float m_SecondaryHeight = 2f;

        [Tooltip("The offset to the height of the sensor.")]
        [SerializeField] private float m_SecondaryHeightOffset = -2f;

        public float range
        {
            get => m_Range;
            set => m_Range = Mathf.Clamp(value, 0f, float.MaxValue);
        }

        public float angle
        {
            get => m_Angle;
            set => m_Angle = Mathf.Clamp(value % 360f, 0.1f, 360f);
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
            set => m_SecondaryAngle = Mathf.Clamp(value % 360f, 0.1f, 360f);
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

        protected sealed override JobHandle ScheduleBoundsCheckJob(
            NativeArray<float4> stimuliPositions,
            NativeArray<int> stimuliAssociatedObjects,
            PerceivedObjectsList perceivedObjectsList,
            PerceivedObjectsLocationsList perceivedObjectsLocationsList,
            int stimuliCount,
            JobHandle dependencies)
        {
            var t = transform;

            var w2L = t.worldToLocalMatrix;

            return new BoundsCheckJob(
                    w2L,
                    m_Range,
                    m_Angle,
                    m_Height,
                    m_HeightOffset,
                    m_SecondaryRange,
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
        private static unsafe class GizmoHelper
        {
            [BurstCompile(DisableDirectCall = false)]
            internal static void GetArcPoints(float4x4* l2WPtr, float angle, float range, float heightOffset, float height, float3* data, int ptCount)
            {
                data[0] = new float3(0, heightOffset + height, 0);
                data[1] = new float3(0, heightOffset - height, 0);

                var singleArcPtCount = (ptCount - 2) / 2;

                for (var i = 0; i < singleArcPtCount; i++)
                {
                    var a = (90 + (angle * 0.5f)) - (((float) i / (singleArcPtCount - 1)) * angle);
                    var aRad = math.radians(a);

                    var x = math.cos(aRad) * range;
                    var y = heightOffset + height;
                    var z = math.sin(aRad) * range;

                    data[2 + i] = new float3(x, y, z);
                }

                for (var i = 0; i < singleArcPtCount; i++)
                {
                    var a = (90 - (angle * 0.5f)) + (((float) i / (singleArcPtCount - 1)) * angle);
                    var aRad = math.radians(a);

                    var x = math.cos(aRad) * range;
                    var y = heightOffset - height;
                    var z = math.sin(aRad) * range;

                    data[2 + singleArcPtCount + i] = new float3(x, y, z);
                }

                var l2W = *l2WPtr;
                for (var i = 0; i < ptCount; i++)
                {
                    var posLs = data[i];
                    var posWs = math.mul(l2W, new float4(posLs.x, posLs.y, posLs.z, 1)).xyz;
                    data[i] = posWs;
                }
            }
        }

        private unsafe void OnDrawGizmosSelected()
        {
            float4x4 l2W = transform.localToWorldMatrix;

            int GetArcPtsCount(float a)
            {
                return 2 * (Mathf.CeilToInt(a / 30) + 1);
            }

            var arcPtCount = GetArcPtsCount(m_Angle);
            var ptCount = arcPtCount + 2; // center

            var pts = new float3[ptCount];

            fixed (float3* ptsPtr = &pts[0])
            {
                GizmoHelper.GetArcPoints(&l2W,
                    m_Angle,
                    m_Range,
                    m_HeightOffset,
                    m_Height,
                    ptsPtr,
                    ptCount);

                DrawPoints(ptCount, arcPtCount, pts);
            }

            var secondaryArcPtCount = GetArcPtsCount(m_SecondaryAngle);
            var secondaryPtCount = secondaryArcPtCount + 2; // center

            System.Array.Resize(ref pts, secondaryPtCount);

            fixed (float3* ptsPtr = &pts[0])
            {
                GizmoHelper.GetArcPoints(&l2W,
                    m_SecondaryAngle,
                    m_SecondaryRange,
                    m_SecondaryHeightOffset,
                    m_SecondaryHeight,
                    ptsPtr,
                    secondaryPtCount);

                DrawPoints(secondaryPtCount, secondaryArcPtCount, pts);
            }
        }

        private static void DrawPoints(int ptCount, int arcPtCount, float3[] pts)
        {
            for (var i = 3; i < ptCount; i++)
            {
                Gizmos.DrawLine(pts[i - 1], pts[i]);
            }

            Gizmos.DrawLine(pts[2], pts[ptCount - 1]);

            Gizmos.DrawLine(pts[0], pts[1]); // center top to center bottom
            Gizmos.DrawLine(pts[0], pts[2]); // center top to one of the top arc endpoints
            Gizmos.DrawLine(pts[1], pts[ptCount - 1]); // center bottom to one of the bottom arc endpoints
            Gizmos.DrawLine(pts[0], pts[2 + (arcPtCount / 2) - 1]); // center top to the other top arc endpoint
            Gizmos.DrawLine(pts[1], pts[2 + (arcPtCount / 2)]); // center bottom to the other bottom arc endpoint
        }

        [BurstCompile]
        private unsafe struct BoundsCheckJob : IJob
        {
            internal BoundsCheckJob(
                float4x4 sensorW2L,
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
                m_SensorW2L = sensorW2L;
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

            [ReadOnly] private readonly float4x4 m_SensorW2L;
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

            public void Execute()
            {
                var vectorizedPositions = m_StimuliPositions.Reinterpret<float4x4>(sizeof(float4));
                var vectorizedLength = vectorizedPositions.Length;
                var vectorizedResults = stackalloc bool4[vectorizedLength];

                var localizedPositions = (float4x4*) UnsafeUtility.Malloc(UnsafeUtility.SizeOf<float4x4>() * vectorizedLength,
                    UnsafeUtility.AlignOf<float4x4>(),
                    Allocator.Persistent);

                {
                    // calculated localized positions
                    {
                        for (var i = 0; i < vectorizedLength; i++)
                        {
                            localizedPositions[i] = math.mul(m_SensorW2L, vectorizedPositions[i]);
                        }
                    }

                    PrimaryCheck(vectorizedLength, localizedPositions, vectorizedResults);
                    SecondaryCheck(vectorizedLength, localizedPositions, vectorizedResults);
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

            private void PrimaryCheck(int vectorizedLength, float4x4* vectorizedLocalPositions, bool4* results)
            {
                // clear
                {
                    for (var i = 0; i < vectorizedLength; i++)
                    {
                        results[i] = true;
                    }
                }

                HeightCheck(vectorizedLength, vectorizedLocalPositions, results, m_Height, m_HeightOffset);
                AngleCheck(vectorizedLength, vectorizedLocalPositions, results, m_AngleInDegrees);
                RangeCheck(vectorizedLength, vectorizedLocalPositions, results, m_Range);
            }

            private void SecondaryCheck(int vectorizedLength, float4x4* vectorizedLocalPositions, bool4* originalResults)
            {
                var secondaryVectorizedResults = stackalloc bool4[vectorizedLength];

                // clear
                {
                    for (var i = 0; i < vectorizedLength; i++)
                    {
                        secondaryVectorizedResults[i] = true;
                    }
                }

                HeightCheck(vectorizedLength, vectorizedLocalPositions, secondaryVectorizedResults, m_SecondaryHeight, m_SecondaryHeightOffset);
                AngleCheck(vectorizedLength, vectorizedLocalPositions, secondaryVectorizedResults, m_SecondaryAngleInDegrees);
                RangeCheck(vectorizedLength, vectorizedLocalPositions, secondaryVectorizedResults, m_SecondaryRange);

                for (var i = 0; i < vectorizedLength; i++)
                {
                    originalResults[i] |= secondaryVectorizedResults[i];
                }
            }

            private static void HeightCheck(int vectorizedLength, float4x4* vectorizedLocalPositions, bool4* vectorizedResults, float height, float heightOffset)
            {
                float4 localHeightMin4 = heightOffset - (height * 0.5f);
                float4 localHeightMax4 = heightOffset + (height * 0.5f);

                for (var i = 0; i < vectorizedLength; i++)
                {
                    var stimuliLocalPos4 = vectorizedLocalPositions[i];

                    float4 stimuliLocalHeight4 = default;
                    for (var j = 0; j < 4; j++)
                    {
                        stimuliLocalHeight4[j] = stimuliLocalPos4[j].y;
                    }

                    vectorizedResults[i] &= (stimuliLocalHeight4 < localHeightMax4) & (stimuliLocalHeight4 > localHeightMin4);
                }
            }

            private static void AngleCheck(int vectorizedLength, float4x4* vectorizedLocalPositions, bool4* vectorizedResults, float angleInDegrees)
            {
                var angleInRadians = math.radians(angleInDegrees);
                var halfAngleInRadians = angleInRadians * 0.5f;
                var cosHalfAngleInRadians = math.cos(halfAngleInRadians);
                var dotProductThreshold = new float4(cosHalfAngleInRadians);

                for (var i = 0; i < vectorizedLength; i++)
                {
                    var stimuliLocalPos4 = vectorizedLocalPositions[i];

                    float2x4 normalizedStimulusDirectionXZ = default;
                    for (var j = 0; j < 4; j++)
                    {
                        normalizedStimulusDirectionXZ[j] = math.normalizesafe(stimuliLocalPos4[j].xz, new float2(0, 1));
                    }

                    float4 dotProduct = default;
                    for (var j = 0; j < 4; j++)
                    {
                        dotProduct[j] = normalizedStimulusDirectionXZ[j].y;
                    }

                    vectorizedResults[i] &= (dotProduct > dotProductThreshold);
                }
            }

            private static void RangeCheck(int vectorizedLength, float4x4* vectorizedLocalPositions, bool4* vectorizedResults, float range)
            {
                var rangeSq4 = new float4(range * range);

                for (var i = 0; i < vectorizedLength; i++)
                {
                    var stimuliLocalPos4 = vectorizedLocalPositions[i];

                    float4 lengthXZSq4 = default;
                    for (var j = 0; j < 4; j++)
                    {
                        lengthXZSq4[j] = math.lengthsq(stimuliLocalPos4[j].xz);
                    }

                    vectorizedResults[i] &= (lengthXZSq4 < rangeSq4);
                }
            }
        }
    }
}