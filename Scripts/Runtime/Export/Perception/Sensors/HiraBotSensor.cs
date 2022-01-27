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
        [SerializeField] private int m_StimulusMask;

        [Space] [Header("Shape")]
        [Tooltip("The maximum range of the sensor.")]
        [SerializeField] private float m_Range;

        public int stimulusMask
        {
            get => m_StimulusMask;
            set
            {
                m_StimulusMask = value;
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    return;
                }
#endif
                PerceptionSystem.ChangeStimulusMask(this);
            }
        }

        public float range
        {
            get => m_Range;
            set => m_Range = Mathf.Clamp(value, 0f, float.MaxValue);
        }

        public abstract JobHandle ScheduleBoundsCheckJob(NativeArray<float4x4> stimuliPositions, NativeArray<bool4> results);

        private void OnEnable()
        {
            PerceptionSystem.AddSensor(this);
        }

        private void OnDisable()
        {
            PerceptionSystem.RemoveSensor(this);
        }
    }
}