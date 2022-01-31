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

        [Tooltip("The maximum number of GameObjects this sensor can detect at any given time.")]
        [SerializeField] private byte m_MaxPerceivedGameObjects = 32;

        [Space] [Header("Shape")]
        [Tooltip("The maximum range of the sensor.")]
        [SerializeField] private float m_Range = 8f;

        internal NativeArray<(byte stimulusType, JobHandle job)> m_JobHandlePerStimulusType;
        internal byte m_JobCountThisFrame;
        internal NativeArray<bool4>[] m_SuccessCheckArraysPerStimulusType;
        internal NativeArray<int> m_PerceivedGameObjects;
        internal NativeArray<int> m_GameObjectsPerceivedThisFrame;

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

        public byte maxPerceivedGameObjects
        {
            get => m_MaxPerceivedGameObjects;
            set
            {
                m_MaxPerceivedGameObjects = value;
                PerceptionSystem.ChangeSensorMaxPerceivedGameObjectsCount(this);
            }
        }

        private void OnEnable()
        {
            PerceptionSystem.AddSensor(this, m_StimulusMask);
        }

        private void OnDisable()
        {
            PerceptionSystem.RemoveSensor(this);
        }

        internal void ScheduleJobs(NativeArray<float4x4> stimuliPositions, byte stimulusTypeIndex)
        {
            var successCheckArray = new NativeArray<bool4>(stimuliPositions.Length,
                Allocator.TempJob, NativeArrayOptions.ClearMemory);

            m_SuccessCheckArraysPerStimulusType[stimulusTypeIndex] = successCheckArray;

            JobHandle boundsCheckJob;

            try
            {
                boundsCheckJob = ScheduleBoundsCheckJob(stimuliPositions, successCheckArray);
                // schedule the los-check or nav-distance check jobs here
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                m_SuccessCheckArraysPerStimulusType[stimulusTypeIndex] = default;
                successCheckArray.Dispose();
                return;
            }

            m_JobHandlePerStimulusType[m_JobCountThisFrame] = (stimulusTypeIndex, boundsCheckJob);

            m_JobCountThisFrame++;
        }

        protected abstract JobHandle ScheduleBoundsCheckJob(NativeArray<float4x4> stimuliPositions, NativeArray<bool4> results);
    }
}