using HiraBots;

namespace UnityEngine
{
    public class TacMap : MonoBehaviour
    {
        [Tooltip("The size of a hexagonal cell.")]
        [SerializeField] private float m_CellSize = 0.1f;

        private TacMapComponent m_TacMapComponent = null;

        private Vector3 m_CurrentPosition;
        private Quaternion m_CurrentRotation;
        private Vector3 m_CurrentScale;
        private float m_CurrentCellSize;

        private void OnEnable()
        {
            StartUsingNewTacMapComponent();
        }

        private void OnDisable()
        {
            StopUsingOldTacMapComponent();
        }

        [ContextMenu("Synchronize to Transform")]
        public void SynchronizeToTransform()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            var t = transform;

            if (m_CurrentPosition != t.position || m_CurrentRotation != t.rotation || m_CurrentScale != t.localScale)
            {
                StopUsingOldTacMapComponent();
                StartUsingNewTacMapComponent();
            }
        }

        public float cellSize
        {
            get => m_CellSize;
            set
            {
                m_CellSize = Mathf.Clamp(value, 0.01f, float.MaxValue);

                if (!isActiveAndEnabled)
                {
                    return;
                }

                if (!Mathf.Approximately(m_CurrentCellSize, value))
                {
                    StopUsingOldTacMapComponent();
                    StartUsingNewTacMapComponent();
                }
            }
        }

        private void StartUsingNewTacMapComponent()
        {
            if (m_TacMapComponent != null)
            {
                return;
            }

            var t = transform;

            if (!TacMapComponent.TryCreate(t, m_CellSize, out var component))
            {
                return;
            }

            m_TacMapComponent = component;
            m_CurrentPosition = t.position;
            m_CurrentRotation = t.rotation;
            m_CurrentScale = t.localScale;
            m_CurrentCellSize = m_CellSize;
        }

        private void StopUsingOldTacMapComponent()
        {
            m_TacMapComponent?.Dispose();
            m_TacMapComponent = null;
        }
    }
}