using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace HiraBots
{
    internal static unsafe partial class PerceptionSystem
    {
        [BurstCompile]
        internal struct UpdateObjectsPerceivedThisFrameJob : IJob
        {
            internal UpdateObjectsPerceivedThisFrameJob(
                NativeArray<bool> successCheckArray,
                NativeArray<int> associatedObjects,
                NativeArray<UnmanagedCollections.Data<int>> objectsPerceivedThisFrame)
            {
                m_SuccessCheckArray = successCheckArray;
                m_AssociatedObjects = associatedObjects;
                m_ObjectsPerceivedThisFrame = objectsPerceivedThisFrame;
            }

            [ReadOnly] private readonly NativeArray<bool> m_SuccessCheckArray;
            [ReadOnly] private readonly NativeArray<int> m_AssociatedObjects;
            private NativeArray<UnmanagedCollections.Data<int>> m_ObjectsPerceivedThisFrame;

            public void Execute()
            {
                var length = m_SuccessCheckArray.Length;
                var successChecks = (bool*) m_SuccessCheckArray.GetUnsafeReadOnlyPtr();

                for (var i = 0; i < length; i++)
                {
                    if (successChecks[i])
                    {
                        m_ObjectsPerceivedThisFrame.Push(m_AssociatedObjects[i]);
                    }
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
                        if (!perceivedObjects.Add(objectPerceivedThisFrame, actualTimeToStimulusDeath, true))
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