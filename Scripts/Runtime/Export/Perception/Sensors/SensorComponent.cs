using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace HiraBots
{
    public abstract class SensorComponent : MonoBehaviour
    {
        [Tooltip("The types of stimuli this sensor can detect.")]
        [SerializeField] private int m_StimulusMask;
        public abstract JobHandle ScheduleBoundsCheckJob(NativeArray<float3x4> stimuliPositions, NativeArray<float4> scores);
    }
}