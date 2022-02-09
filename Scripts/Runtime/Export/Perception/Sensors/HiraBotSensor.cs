using HiraBots;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.AI
{
    public abstract class HiraBotSensor : MonoBehaviour
    {
        [System.Serializable]
        public class NewObjectPerceivedEvent : Events.UnityEvent<Object>
        {
        }

        [System.Serializable]
        public class ObjectStoppedPerceivingEvent : Events.UnityEvent<Object>
        {
        }

        [System.Serializable]
        public struct LineOfSightCheckProperties
        {
            public bool m_Enabled;
            public LayerMask m_BlockingObjects;
        }

        // [System.Serializable]
        // public struct NavDistanceCheckProperties
        // {
        //     public bool m_Enabled;
        //     public float m_StimulusNavmeshDistanceTolerance;
        //     public int m_AgentType;
        //     public int m_AreaMask;
        // }

        [Space] [Header("Detection")]
        [Tooltip("The types of stimuli this sensor can detect.")]
        [SerializeField] private int m_StimulusMask = ~0;

        [Tooltip("The time it takes to stop perceiving an object after all the stimulus related to it are stopped being perceived.")]
        [SerializeField] private float m_TimeToStimulusDecay = 1f;

        [Space] [Header("Callbacks")]
        [Tooltip("Callback for when a new object gets perceived.")]
        [SerializeField] private NewObjectPerceivedEvent m_OnNewObjectPerceived;

        [Tooltip("Callback for when an object stops being perceived.")]
        [SerializeField] private ObjectStoppedPerceivingEvent m_OnObjectStoppedPerceiving;

        [Space] [Header("Secondary Checks")]
        [Tooltip("Whether to check for the line of sight to the stimulus.")]
        [SerializeField] private LineOfSightCheckProperties m_LineOfSightCheck;

        [Space] [Header("Shape")]
        [Tooltip("The maximum range of the sensor.")]
        [SerializeField] private float m_Range = 8f;

        internal (NativeArray<UnmanagedCollections.OrderedData<int>> objects, NativeArray<UnmanagedCollections.Data<float>> timeToStimulusDeath) m_PerceivedObjects;
        internal NativeArray<UnmanagedCollections.Data<float4>> m_PerceivedObjectsLocations;
        internal NativeArray<UnmanagedCollections.Data<int>> m_ObjectsPerceivedThisFrame;
        internal NativeArray<UnmanagedCollections.Data<int>> m_NewObjectsPerceived;
        internal NativeArray<UnmanagedCollections.Data<int>> m_ObjectsStoppedPerceiving;
        internal JobHandle? m_UpdateJob;

        public int stimulusMask
        {
            get => m_StimulusMask;
            set
            {
                m_StimulusMask = value;
                PerceptionSystem.ChangeStimulusMask(this, value);
            }
        }

        public LineOfSightCheckProperties lineOfSightCheck
        {
            get => m_LineOfSightCheck;
            set
            {
                m_LineOfSightCheck = value;
                PerceptionSystem.ChangeSecondaryChecksEnabled(this, value.m_Enabled, false); //m_NavDistanceCheck.m_Enabled);
            }
        }

        // public NavDistanceCheckProperties navDistanceCheck
        // {
        //     get => m_NavDistanceCheck;
        //     set
        //     {
        //         m_NavDistanceCheck = value;
        //         PerceptionSystem.ChangeSecondaryChecksEnabled(this, m_LineOfSightCheck.m_Enabled, value.m_Enabled);
        //     }
        // }

        public float range
        {
            get => m_Range;
            set => m_Range = Mathf.Clamp(value, 0f, float.MaxValue);
        }

        public NewObjectPerceivedEvent newObjectPerceived => m_OnNewObjectPerceived;

        public ObjectStoppedPerceivingEvent objectStoppedPerceiving => m_OnObjectStoppedPerceiving;

        private void OnEnable()
        {
            PerceptionSystem.AddSensor(this, m_StimulusMask, m_LineOfSightCheck.m_Enabled, false); // m_NavDistanceCheck.m_Enabled);
        }

        private void OnDisable()
        {
            PerceptionSystem.RemoveSensor(this);
        }

        internal bool ScheduleJobsToDetermineObjectsPerceivedThisTick(
            NativeArray<float4> stimuliPositions,
            NativeArray<int> stimuliAssociatedObjects,
            int stimuliCount)
        {
            JobHandle jh;
            try
            {
                jh = ScheduleBoundsCheckJob(
                    stimuliPositions,
                    stimuliAssociatedObjects,
                    new PerceivedObjectsList(m_ObjectsPerceivedThisFrame),
                    m_LineOfSightCheck.m_Enabled // || m_NavDistanceCheck.m_Enabled
                        ? new PerceivedObjectsLocationsList()
                        : new PerceivedObjectsLocationsList(m_PerceivedObjectsLocations),
                    stimuliCount,
                    m_UpdateJob ?? default);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            m_UpdateJob = jh;
            return true;
        }

        protected abstract JobHandle ScheduleBoundsCheckJob(
            NativeArray<float4> nativeArray,
            NativeArray<int> stimuliAssociatedObjects,
            PerceivedObjectsList perceivedObjectsList,
            PerceivedObjectsLocationsList perceivedObjectsLocationsList,
            int stimuliCount,
            JobHandle dependencies);

        internal void ScheduleSecondaryCheckJobs()
        {
            if (!m_UpdateJob.HasValue)
            {
                return;
            }

            m_UpdateJob.Value.Complete();
            m_UpdateJob = null;

            JobHandle lineOfSightJob = default;
            if (m_LineOfSightCheck.m_Enabled)
            {
                var sensorPos = transform.position;
                var sensorPosFloat4 = new float4(sensorPos.x, sensorPos.y, sensorPos.z, 1);

                var raycastCount = (m_PerceivedObjectsLocations.Count() + 3) & ~3;

                var raycastCommands = new NativeArray<RaycastCommand>(raycastCount,
                    Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

                var raycastResults = new NativeArray<RaycastHit>(raycastCount,
                    Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

                var buildJob = new PerceptionSystem.BuildRaycastCommandsJob(sensorPosFloat4, m_Range, m_LineOfSightCheck.m_BlockingObjects,
                        m_PerceivedObjectsLocations, raycastCommands)
                    .Schedule();

                var raycastJob = RaycastCommand.ScheduleBatch(raycastCommands, raycastResults, 1, buildJob);

                raycastCommands.Dispose(raycastJob);

                var readJob = new PerceptionSystem.ReadRaycastHitResultsJob(raycastResults, m_ObjectsPerceivedThisFrame)
                    .Schedule(raycastJob);

                raycastResults.Dispose(readJob);

                lineOfSightJob = readJob;
            }

            JobHandle navDistanceJob = default;
            // if (m_NavDistanceCheck.m_Enabled)
            // {
            //     
            // }

            m_UpdateJob = JobHandle.CombineDependencies(lineOfSightJob, navDistanceJob);
        }

        internal void ScheduleJobsToSortPerceivedObjectsData(float deltaTime)
        {
            if (!m_UpdateJob.HasValue)
            {
                return;
            }

            m_UpdateJob = new PerceptionSystem.SortPerceivedObjectsData(
                    m_PerceivedObjects,
                    m_ObjectsPerceivedThisFrame,
                    m_NewObjectsPerceived,
                    m_ObjectsStoppedPerceiving,
                    deltaTime,
                    m_TimeToStimulusDecay)
                .Schedule(m_UpdateJob.Value);
        }

        internal unsafe void CollectJobResults()
        {
            if (!m_UpdateJob.HasValue)
            {
                return;
            }

            m_UpdateJob.Value.Complete();
            m_UpdateJob = null;

            // callback for new objects perceived
            {
                var newObjectsPerceived = (int*) m_NewObjectsPerceived.GetUnsafeUnmanagedListReadOnlyPtr();
                var newObjectsPerceivedCount = m_NewObjectsPerceived.Count();

                for (var i = 0; i < newObjectsPerceivedCount; i++)
                {
                    var obj = ObjectUtils.InstanceIDToObject(newObjectsPerceived[i]);

                    if (ReferenceEquals(obj, null))
                    {
                        continue;
                    }

                    try
                    {
                        m_OnNewObjectPerceived.Invoke(obj);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }

            // callback for objects stopped being perceived
            {
                var objectsStoppedPerceiving = (int*) m_ObjectsStoppedPerceiving.GetUnsafeUnmanagedListReadOnlyPtr();
                var objectsStoppedPerceivingCount = m_ObjectsStoppedPerceiving.Count();

                for (var i = 0; i < objectsStoppedPerceivingCount; i++)
                {
                    var obj = ObjectUtils.InstanceIDToObject(objectsStoppedPerceiving[i]);

                    if (ReferenceEquals(obj, null))
                    {
                        continue;
                    }

                    try
                    {
                        m_OnObjectStoppedPerceiving.Invoke(obj);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }
    }
}