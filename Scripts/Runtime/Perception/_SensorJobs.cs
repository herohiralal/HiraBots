using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace HiraBots
{
    internal static unsafe partial class PerceptionSystem
    {
        [BurstCompile]
        internal struct BuildRaycastCommandsJob : IJob
        {
            internal BuildRaycastCommandsJob(
                float4 sensorPosition,
                LayerMask blockingMask,
                NativeArray<UnmanagedCollections.Data<float4>> perceivedObjectsPositions,
                NativeArray<RaycastCommand> raycastCommands)
            {
                m_SensorPosition = sensorPosition;
                m_BlockingMask = blockingMask;
                m_PerceivedObjectsPositions = perceivedObjectsPositions;
                m_RaycastCommands = raycastCommands;
            }

            [ReadOnly] private readonly float4 m_SensorPosition;
            [ReadOnly] private readonly LayerMask m_BlockingMask;
            [ReadOnly] private NativeArray<UnmanagedCollections.Data<float4>> m_PerceivedObjectsPositions;
            [WriteOnly] private NativeArray<RaycastCommand> m_RaycastCommands;

            public void Execute()
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                var capacity = m_PerceivedObjectsPositions.Capacity();
                if ((capacity % 4) != 0)
                {
                    throw new System.ArgumentOutOfRangeException(nameof(capacity), $"Count must be divisible by 4. Current value - {capacity}.");
                }
#endif

                var vectorizedCount = ((m_PerceivedObjectsPositions.Count() + 3) & ~3) / 4;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (m_RaycastCommands.Length != (vectorizedCount * 4))
                {
                    throw new System.InvalidOperationException($"Raycast command length must be equal to {vectorizedCount * 4}.");
                }
#endif

                var vectorizedSensorPosition = new float4x4(m_SensorPosition, m_SensorPosition, m_SensorPosition, m_SensorPosition);
                var vectorizedPositions = (float4x4*) m_PerceivedObjectsPositions.GetUnsafeUnmanagedListReadOnlyPtr();

                for (var i = 0; i < vectorizedCount; i++)
                {
                    var direction = vectorizedPositions[i] - vectorizedSensorPosition;

                    float4 magnitudes4 = default;
                    for (var j = 0; j < 4; j++)
                    {
                        magnitudes4[j] = math.length(direction[j].xyz);
                        direction[j].xyz = math.normalizesafe(direction[j].xyz);
                    }

                    for (var j = 0; j < 4; j++)
                    {
                        m_RaycastCommands[(i * 4) + j] = new RaycastCommand(
                            vectorizedSensorPosition[j].xyz,
                            direction[j].xyz,
                            magnitudes4[j],
                            m_BlockingMask,
                            1);
                    }
                }
            }
        }

        [BurstCompile]
        internal struct ReadRaycastHitResultsJob : IJob
        {
            internal ReadRaycastHitResultsJob(
                NativeArray<RaycastHit> results,
                NativeArray<UnmanagedCollections.Data<int>> objectsPerceivedThisFrame)
            {
                m_Results = results;
                m_ObjectsPerceivedThisFrame = objectsPerceivedThisFrame;
            }

            [ReadOnly] private NativeArray<RaycastHit> m_Results;
            private NativeArray<UnmanagedCollections.Data<int>> m_ObjectsPerceivedThisFrame;

            public void Execute()
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (m_Results.Length < m_ObjectsPerceivedThisFrame.Count())
                {
                    throw new System.InvalidOperationException("Results length must be more than or equal to the number of objects perceived this frame.");
                }
#endif

                var raycastResults = (CustomRaycastHit*) m_Results.Reinterpret<CustomRaycastHit>().GetUnsafeReadOnlyPtr();
                var objectsPerceivedThisFrame = (int*) m_ObjectsPerceivedThisFrame.GetUnsafeUnmanagedListPtr();

                var count = m_ObjectsPerceivedThisFrame.Count();
                for (var i = 0; i < count; i++)
                {
                    objectsPerceivedThisFrame[i] = raycastResults[i].m_Collider == 0 ? objectsPerceivedThisFrame[i] : 0;
                }
            }
        }

        [BurstCompile]
        internal struct SortPerceivedObjectsData : IJob
        {
            internal SortPerceivedObjectsData(
                (NativeArray<UnmanagedCollections.OrderedData<int>>, NativeArray<UnmanagedCollections.Data<float>>) perceivedObjects,
                NativeArray<UnmanagedCollections.Data<int>> objectsPerceivedThisFrame,
                NativeArray<UnmanagedCollections.Data<int>> newObjectsPerceived,
                NativeArray<UnmanagedCollections.Data<int>> objectsStoppedPerceiving,
                float deltaTime,
                float timeToStimulusDeath)
            {
                m_PerceivedObjectsObjects = perceivedObjects.Item1;
                m_PerceivedObjectsTimeToStimulusDeath = perceivedObjects.Item2;
                m_ObjectsPerceivedThisFrame = objectsPerceivedThisFrame;
                m_NewObjectsPerceived = newObjectsPerceived;
                m_ObjectsStoppedPerceiving = objectsStoppedPerceiving;
                m_DeltaTime = deltaTime;
                m_TimeToStimulusDeath = timeToStimulusDeath;
            }

            private NativeArray<UnmanagedCollections.OrderedData<int>> m_PerceivedObjectsObjects;
            private NativeArray<UnmanagedCollections.Data<float>> m_PerceivedObjectsTimeToStimulusDeath;
            private NativeArray<UnmanagedCollections.Data<int>> m_ObjectsPerceivedThisFrame;
            private NativeArray<UnmanagedCollections.Data<int>> m_NewObjectsPerceived;
            private NativeArray<UnmanagedCollections.Data<int>> m_ObjectsStoppedPerceiving;

            private readonly float m_DeltaTime;
            private readonly float m_TimeToStimulusDeath;

            public void Execute()
            {
                var perceivedObjects = (m_PerceivedObjectsObjects, m_PerceivedObjectsTimeToStimulusDeath);

                // deduct delta-time from all perceived objects
                {
                    var perceivedObjectsCount = perceivedObjects.Count();
                    var timesToStimulusDeaths = (float*) m_PerceivedObjectsTimeToStimulusDeath.GetUnsafeUnmanagedListPtr();
                    for (var i = 0; i < perceivedObjectsCount; i++)
                    {
                        timesToStimulusDeaths[i] -= m_DeltaTime;
                    }
                }

                // clear existing data
                {
                    m_NewObjectsPerceived.Clear();
                    m_ObjectsStoppedPerceiving.Clear();
                }

                // determine the new objects perceived this frame
                {
                    var actualTimeToStimulusDeath = m_TimeToStimulusDeath - m_DeltaTime;

                    // sort all the objects perceived in this frame
                    var objectsPerceivedThisFrameSet = m_ObjectsPerceivedThisFrame.Sort();

                    var objectsPerceivedThisFrameCount = objectsPerceivedThisFrameSet.Count();

                    for (var i = 0; i < objectsPerceivedThisFrameCount; i++)
                    {
                        var objectPerceivedThisFrame = objectsPerceivedThisFrameSet.GetElementAt(i);

                        // this will also update the status of the objects that have been perceived this frame
                        if (objectPerceivedThisFrame == 0 || !perceivedObjects.Add(objectPerceivedThisFrame, actualTimeToStimulusDeath, true))
                        {
                            // continue if this object was already perceived
                            continue;
                        }

                        // otherwise add it to the newly perceived objects
                        m_NewObjectsPerceived.Push(objectPerceivedThisFrame);
                    }
                }

                // determine the objects stopped perceiving
                {
                    for (var i = perceivedObjects.Count() - 1; i >= 0; i--)
                    {
                        if (m_PerceivedObjectsTimeToStimulusDeath.GetElementAt(i) > 0)
                        {
                            // continue if the current stimulus is still alive
                            continue;
                        }

                        // otherwise mark the object as not being perceived anymore
                        m_ObjectsStoppedPerceiving.Push(m_PerceivedObjectsObjects.GetElementAt(i));

                        // also remove the object from the perceived objects set
                        m_PerceivedObjectsObjects.RemoveFrom(i);
                        m_PerceivedObjectsTimeToStimulusDeath.RemoveFrom(i);
                    }
                }

                // clear unneeded data
                {
                    m_ObjectsPerceivedThisFrame.Clear();
                }
            }
        }
    }
}