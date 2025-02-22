#if UNITY_EDITOR
using HiraBots;

namespace UnityEngine.AI
{
    public partial class TacMap
    {
        [SerializeField] private bool m_DrawDebugView = false;
        [SerializeField] private float m_InfluenceA = 0f;
        [ColorUsage(false, false)] [SerializeField] private Color m_ColorA = Color.green;
        [SerializeField] private float m_InfluenceB = 1f;
        [ColorUsage(false, false)] [SerializeField] private Color m_ColorB = Color.red;

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                cellSize = m_CellSize;
            }
        }

        private void OnDrawGizmos()
        {
            if (!m_DrawDebugView)
            {
                return;
            }

            var t = transform;

            if (m_TacMapComponent == null
                || t.position != m_CurrentPosition || t.rotation != m_CurrentRotation || t.localScale != m_CurrentScale
                || !Mathf.Approximately(m_CurrentCellSize, m_CellSize))
            {
                TacMapComponent.DrawGizmosDisabled(t, m_CellSize);
            }

            m_TacMapComponent?.DrawGizmos(m_InfluenceA, m_InfluenceB, m_ColorA, m_ColorB);
        }
    }
}
#endif