using System.Linq;
using HiraBots;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
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
            public bool m_Invert;
        }

        [System.Serializable]
        public struct NavDistanceCheckProperties
        {
            [System.Serializable]
            public enum SensorNotOnNavMeshAction : byte
            {
                SkipNavDistanceCheck,
                MarkAllStimuliAsNotPerceived
            }

            [System.Serializable]
            public enum StimulusNotOnNavMeshAction : byte
            {
                ConsiderPerceived,
                ConsiderNotPerceived
            }

            public bool m_Enabled;
            public bool m_Invert;
            public float m_Range;
            public NavAgentType m_AgentType;
            public NavAreaMask m_AreaMask;
            public float m_NavmeshDistanceTolerance;
            public SensorNotOnNavMeshAction m_SensorNotOnNavMeshAction;
            public StimulusNotOnNavMeshAction m_StimulusNotOnNavMeshAction;
        }

        [Space] [Header("Detection")]
        [Tooltip("The types of stimuli this sensor can detect.")]
        [SerializeField] private StimulusMask m_StimulusMask = StimulusMask.all;

        [Tooltip("The time it takes to stop perceiving an object after all the stimulus related to it are stopped being perceived.")]
        [SerializeField] private float m_TimeToStimulusDecay = 1f;

        [Space] [Header("Callbacks")]
        [Tooltip("Callback for when a new object gets perceived.")]
        [SerializeField] private NewObjectPerceivedEvent m_OnNewObjectPerceived;

        [Tooltip("Callback for when an object stops being perceived.")]
        [SerializeField] private ObjectStoppedPerceivingEvent m_OnObjectStoppedPerceiving;

        [Space] [Header("Secondary Checks")]
        [Tooltip("Whether to check for the line of sight to the stimulus.")]
        [SerializeField] private LineOfSightCheckProperties m_LineOfSightCheck = new LineOfSightCheckProperties
        {
            m_Enabled = false,
            m_BlockingObjects = ~0,
            m_Invert = false
        };

        [Tooltip("Whether to check for the navigational distance to the stimulus.")]
        [SerializeField] private NavDistanceCheckProperties m_NavDistanceCheck = new NavDistanceCheckProperties
        {
            m_Enabled = false,
            m_Invert = false,
            m_Range = 8f,
            m_AgentType = 0,
            m_AreaMask = NavMesh.AllAreas,
            m_NavmeshDistanceTolerance = 1f,
            m_SensorNotOnNavMeshAction = NavDistanceCheckProperties.SensorNotOnNavMeshAction.MarkAllStimuliAsNotPerceived,
            m_StimulusNotOnNavMeshAction = NavDistanceCheckProperties.StimulusNotOnNavMeshAction.ConsiderNotPerceived
        };

        private ObjectCache m_ObjectCache = new ObjectCache(0);

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
                PerceptionSystem.ChangeSecondaryChecksEnabled(this, value.m_Enabled, m_NavDistanceCheck.m_Enabled);
            }
        }

        public NavDistanceCheckProperties navDistanceCheck
        {
            get => m_NavDistanceCheck;
            set
            {
                m_NavDistanceCheck = value;
                PerceptionSystem.ChangeSecondaryChecksEnabled(this, m_LineOfSightCheck.m_Enabled, value.m_Enabled);
            }
        }

        public Object[] currentlyPerceivedObjects => m_ObjectCache.GetAllObjects().ToArray();

        public NewObjectPerceivedEvent newObjectPerceived => m_OnNewObjectPerceived;

        public ObjectStoppedPerceivingEvent objectStoppedPerceiving => m_OnObjectStoppedPerceiving;

        protected void OnEnable()
        {
            m_ObjectCache.Clear();
            PerceptionSystem.AddSensor(this, m_StimulusMask, m_LineOfSightCheck.m_Enabled, m_NavDistanceCheck.m_Enabled);

            try
            {
                OnEnableCallback();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        protected virtual void OnEnableCallback()
        {
            
        }

        protected void OnDisable()
        {
            try
            {
                OnDisableCallback();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

            PerceptionSystem.RemoveSensor(this);
            m_ObjectCache.Clear();
        }

        protected virtual void OnDisableCallback()
        {
            
        }

        protected void OnValidate()
        {
            stimulusMask = m_StimulusMask;
            lineOfSightCheck = m_LineOfSightCheck;
            navDistanceCheck = m_NavDistanceCheck;

            try
            {
                OnValidateCallback();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        protected virtual void OnValidateCallback()
        {
            
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
                    m_LineOfSightCheck.m_Enabled || m_NavDistanceCheck.m_Enabled
                        ? new PerceivedObjectsLocationsList(true, m_PerceivedObjectsLocations)
                        : new PerceivedObjectsLocationsList(false, m_PerceivedObjectsLocations),
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

                var raycastCount = (m_PerceivedObjectsLocations.Count<float4>() + 3) & ~3;

                var raycastCommands = new NativeArray<RaycastCommand>(raycastCount,
                    Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

                // gotta do this mf thang because when scheduling RaycastCommand batch, they check for
                // max hits when scheduling, even if there's a job dependency, which is imo stupid
                unsafe
                {
                    var commands = (RaycastCommand*) raycastCommands.GetUnsafePtr();
                    for (var i = raycastCommands.Length - 1; i >= 0; i--)
                    {
                        commands[i].maxHits = 1;
                    }
                }

                var raycastResults = new NativeArray<RaycastHit>(raycastCount,
                    Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

                var buildJob = new PerceptionSystem.BuildRaycastCommandsJob(sensorPosFloat4, m_LineOfSightCheck.m_BlockingObjects,
                        m_PerceivedObjectsLocations, raycastCommands)
                    .Schedule();

                var raycastJob = RaycastCommand.ScheduleBatch(raycastCommands, raycastResults, 1, buildJob);

                raycastCommands.Dispose(raycastJob);

                var readJob = new PerceptionSystem.ReadRaycastHitResultsJob(m_LineOfSightCheck.m_Invert, raycastResults, m_ObjectsPerceivedThisFrame)
                    .Schedule(raycastJob);

                raycastResults.Dispose(readJob);

                lineOfSightJob = readJob;
            }

            JobHandle navDistanceJob = default;
            if (m_NavDistanceCheck.m_Enabled)
            {
                
            }

            m_UpdateJob = JobHandle.CombineDependencies(lineOfSightJob, navDistanceJob);
        }

        internal void ScheduleJobsToSortPerceivedObjectsData(float deltaTime)
        {
            m_UpdateJob = new PerceptionSystem.SortPerceivedObjectsData(
                    m_PerceivedObjects,
                    m_ObjectsPerceivedThisFrame,
                    m_NewObjectsPerceived,
                    m_ObjectsStoppedPerceiving,
                    deltaTime,
                    m_TimeToStimulusDecay)
                .Schedule(m_UpdateJob ?? default);
        }

        internal void CompleteJobs()
        {
            m_UpdateJob?.Complete();
            m_UpdateJob = null;
            m_ObjectsPerceivedThisFrame.Clear();
            m_PerceivedObjectsLocations.Clear();
        }

        internal unsafe void CollectJobResults()
        {
            // callback for new objects perceived
            {
                var newObjectsPerceived = (int*) m_NewObjectsPerceived.GetUnsafeUnmanagedListReadOnlyPtr();
                var newObjectsPerceivedCount = m_NewObjectsPerceived.Count<int>();

                for (var i = 0; i < newObjectsPerceivedCount; i++)
                {
                    var id = newObjectsPerceived[i];
                    var obj = ObjectUtils.InstanceIDToObject(id);

                    if (ReferenceEquals(obj, null))
                    {
                        continue;
                    }

                    m_ObjectCache.Add(id, obj);

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
                var objectsStoppedPerceivingCount = m_ObjectsStoppedPerceiving.Count<int>();

                for (var i = 0; i < objectsStoppedPerceivingCount; i++)
                {
                    var id = objectsStoppedPerceiving[i];

                    if (!m_ObjectCache.TryGetValue(id, out var obj))
                    {
                        continue;
                    }

                    m_ObjectCache.Remove(id);

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