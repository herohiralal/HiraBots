using HiraBots;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.AI
{
    public abstract class HiraBotSensor : MonoBehaviour
    {
        [Space] [Header("Detection")]
        [Tooltip("The types of stimuli this sensor can detect.")]
        [SerializeField] private int m_StimulusMask = ~0;

        [Tooltip("The time it takes to stop perceiving an object after all the stimulus related to it are stopped being perceived.")]
        [SerializeField] private float m_TimeToStimulusDecay = 1f;

        [Space] [Header("Callbacks")]
        [Tooltip("Callback for when a new object gets perceived.")]
        [SerializeField] private ObjectEvent m_OnNewObjectPerceived;

        [Tooltip("Callback for when an object stops being perceived.")]
        [SerializeField] private ObjectEvent m_OnObjectStoppedPerceiving;

        [Space] [Header("Shape")]
        [Tooltip("The maximum range of the sensor.")]
        [SerializeField] private float m_Range = 8f;

        internal (NativeArray<UnmanagedCollections.OrderedData<int>> objects, NativeArray<UnmanagedCollections.Data<float>> timeToStimulusDeath) m_PerceivedObjects;
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

        public float range
        {
            get => m_Range;
            set => m_Range = Mathf.Clamp(value, 0f, float.MaxValue);
        }

        public ObjectEvent newObjectPerceived => m_OnNewObjectPerceived;

        public ObjectEvent objectStoppedPerceiving => m_OnObjectStoppedPerceiving;

        private void OnEnable()
        {
            PerceptionSystem.AddSensor(this, m_StimulusMask);
        }

        private void OnDisable()
        {
            PerceptionSystem.RemoveSensor(this);
        }

        internal unsafe void ScheduleJobsToDetermineObjectsPerceivedThisTick(NativeArray<float4> stimuliPositions, NativeArray<int> stimuliAssociatedObjects)
        {
            var successCheckArray = new NativeArray<bool>(stimuliPositions.Length, Allocator.TempJob,
                NativeArrayOptions.ClearMemory);

            JobHandle jh;

            try
            {
                jh = ScheduleBoundsCheckJob(
                    stimuliPositions.Reinterpret<float4x4>(sizeof(float4)),
                    successCheckArray.Reinterpret<bool4>(sizeof(float4)));
                // schedule the los-check or nav-distance check jobs here
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                successCheckArray.Dispose();
                return;
            }

            var dependency = m_UpdateJob.HasValue ? JobHandle.CombineDependencies(jh, m_UpdateJob.Value) : jh;

            m_UpdateJob = new PerceptionSystem.UpdateObjectsPerceivedThisFrameJob(
                    successCheckArray,
                    stimuliAssociatedObjects,
                    m_ObjectsPerceivedThisFrame)
                .Schedule(dependency);
        }

        protected abstract JobHandle ScheduleBoundsCheckJob(NativeArray<float4x4> stimuliPositions, NativeArray<bool4> results);

        internal void ScheduleJobsToSortPerceivedObjectsData(float deltaTime)
        {
            if (m_UpdateJob.HasValue)
            {
                m_UpdateJob = new PerceptionSystem.SortPerceivedObjectsData(
                        m_PerceivedObjects,
                        m_ObjectsPerceivedThisFrame,
                        m_NewObjectsPerceived,
                        m_ObjectsStoppedPerceiving,
                        deltaTime,
                        m_TimeToStimulusDecay)
                    .Schedule(m_UpdateJob.Value);
            }
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