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

            var scale = t.lossyScale;
            var effectiveScale = Mathf.Min(Mathf.Min(scale.x, scale.y), scale.z);
            var effectiveRadius = effectiveScale * range;

            
        }

        [BurstCompile]
        private struct BoundsCheckJob : IJob
        {

            [ReadOnly] private readonly float4 m_SensorPosition;
            [ReadOnly] private readonly float m_Range;
            [ReadOnly] private readonly int m_StimuliCount;
            [ReadOnly] private NativeArray<float4> m_StimuliPositions;
            [ReadOnly] private readonly NativeArray<int> m_StimuliAssociatedObjects;
            private PerceivedObjectsList m_PerceivedObjectsList;
            private PerceivedObjectsLocationsList m_PerceivedObjectsLocationsList;

            public void Execute()
            {
                
            }
        }
    }
}