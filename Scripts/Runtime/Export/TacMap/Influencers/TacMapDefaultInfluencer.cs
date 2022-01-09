using HiraBots;
using Unity.Collections;
using Unity.Mathematics;

namespace UnityEngine
{
    public class TacMapDefaultInfluencer : TacMapInfluencer
    {
        [Tooltip("The type of normalized distance-to-influence propagation curve to follow.")]
        [SerializeField] private AnimationCurve m_InfluenceCurve = AnimationCurve.Constant(0f, 0f, 1f);

        [Tooltip("The maximum range of influence.")]
        [SerializeField] private float m_Range = 1f;

        [Tooltip("The magnitude to apply the influence with.")]
        [SerializeField] private float m_Magnitude = 1f;

        [System.NonSerialized] private int3? m_PositionInUse = null;
        [System.NonSerialized] private AnimationCurve m_InfluenceCurveInUse = null;
        [System.NonSerialized] private float? m_CellSizeInUse = null;
        [System.NonSerialized] private float? m_RangeInUse = null;
        [System.NonSerialized] private float? m_MagnitudeInUse = null;

        protected override void OnValidateCallback()
        {
            if (Application.isPlaying)
            {
                influenceCurve = m_InfluenceCurve;
                range = m_Range;
                magnitude = m_Magnitude;
            }
        }

        protected override void TickCallback(float deltaTime)
        {
            if (m_PositionInUse.HasValue)
            {
                var equality3 = TacMapUtility.PositionWToOffsetW(transform.position, map.cellSize * 0.5f) == m_PositionInUse.Value;
                var equality = equality3.x && equality3.y && equality3.z;
                if (!equality)
                {
                    MarkDirty();
                }
            }
        }

        public AnimationCurve influenceCurve
        {
            get => m_InfluenceCurve;
            set
            {
                m_InfluenceCurve = value;

                if (m_InfluenceCurveInUse != null && !m_InfluenceCurve.Equals(m_InfluenceCurveInUse))
                {
                    MarkDirty();
                }
            }
        }

        public float range
        {
            get => m_Range;
            set
            {
                m_Range = value;

                if (m_RangeInUse.HasValue && !Mathf.Approximately(m_Range, m_RangeInUse.Value))
                {
                    MarkDirty();
                }
            }
        }

        public float magnitude
        {
            get => m_Magnitude;
            set
            {
                m_Magnitude = value;

                if (m_MagnitudeInUse.HasValue && !Mathf.Approximately(m_Magnitude, m_MagnitudeInUse.Value))
                {
                    MarkDirty();
                }
            }
        }

        private static NativeArray<float> CreateManhattanDistanceToInfluenceCurveArray(float range, float cellSize, AnimationCurve influenceCurve, Allocator allocator)
        {
            var divisions = (int) (range / cellSize) + 1;
            var output = new NativeArray<float>(divisions, allocator, NativeArrayOptions.UninitializedMemory);

            for (var i = 0; i < divisions; i++)
            {
                output[i] = influenceCurve.Evaluate((float) i / divisions);
            }

            return output;
        }

        protected override void AddInfluenceToMap()
        {
            var position = TacMapUtility.PositionWToOffsetW(transform.position, map.cellSize * 0.5f);
            var md2Ic = CreateManhattanDistanceToInfluenceCurveArray(
                m_Range, map.cellSize, m_InfluenceCurve, Allocator.TempJob);

            // schedule/run the job here
            if (updateSynchronously)
            {
                TacMapDefaultInfluencePropagator.Run(map.component, position, md2Ic, m_Magnitude);
                md2Ic.Dispose();
            }
            else
            {
                var jh = TacMapDefaultInfluencePropagator.Schedule(map.component, position, md2Ic, m_Magnitude);
                md2Ic.Dispose(jh);
            }

            m_PositionInUse = position;
            if (!m_InfluenceCurve.Equals(m_InfluenceCurveInUse))
            {
                m_InfluenceCurveInUse = new AnimationCurve(m_InfluenceCurve.keys);
            }
            m_CellSizeInUse = map.cellSize;
            m_RangeInUse = m_Range;
            m_MagnitudeInUse = m_Magnitude;
        }

        protected override void RemoveInfluenceFromMap()
        {
            if (!m_PositionInUse.HasValue || m_InfluenceCurveInUse == null || !m_CellSizeInUse.HasValue || !m_RangeInUse.HasValue || !m_MagnitudeInUse.HasValue)
            {
                return;
            }

            var md2Ic = CreateManhattanDistanceToInfluenceCurveArray(
                m_RangeInUse.Value, m_CellSizeInUse.Value, m_InfluenceCurveInUse, Allocator.TempJob);

            if (updateSynchronously)
            {
                TacMapDefaultInfluencePropagator.Run(map.component, m_PositionInUse.Value, md2Ic, -1 * m_MagnitudeInUse.Value);
                md2Ic.Dispose();
            }
            else
            {
                var jh = TacMapDefaultInfluencePropagator.Schedule(map.component, m_PositionInUse.Value, md2Ic, -1 * m_MagnitudeInUse.Value);
                md2Ic.Dispose(jh);
            }

            m_MagnitudeInUse = null;
            m_RangeInUse = null;
            m_CellSizeInUse = null;
            m_PositionInUse = null;
        }
    }
}