using System.Collections.Generic;
using HiraBots;

namespace UnityEngine
{
    [AddComponentMenu("AI/TacMap")]
    public sealed partial class TacMap : MonoBehaviour
    {
        [Tooltip("The size of a hexagonal cell.")]
        [SerializeField] private float m_CellSize = 1f;

        [System.NonSerialized] private TacMapComponent m_TacMapComponent = null;

        [System.NonSerialized] private Vector3 m_CurrentPosition;
        [System.NonSerialized] private Quaternion m_CurrentRotation;
        [System.NonSerialized] private Vector3 m_CurrentScale;
        [System.NonSerialized] private float m_CurrentCellSize;

        internal HashSet<TacMapInfluencer> influencers { get; } = new HashSet<TacMapInfluencer>();

        private void OnEnable()
        {
            StartUsingNewTacMapComponent();
        }

        private void OnDisable()
        {
            StopUsingOldTacMapComponent();
        }

        private void OnDestroy()
        {
            foreach (var influencer in influencers)
            {
                influencer.OnMapDestroy();
            }
        }

        internal TacMapComponent component => m_TacMapComponent;

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

            if (!TacMapComponent.TryCreate(t, m_CellSize, out var createdComponent))
            {
                return;
            }

            m_TacMapComponent = createdComponent;
            m_CurrentPosition = t.position;
            m_CurrentRotation = t.rotation;
            m_CurrentScale = t.localScale;
            m_CurrentCellSize = m_CellSize;

            foreach (var influencer in influencers)
            {
                influencer.OnNewMapCreated();
            }
        }

        private void StopUsingOldTacMapComponent()
        {
            if (m_TacMapComponent == null)
            {
                return;
            }

            // As much as I wish to mirror the OnNewMapCreated() callback and have a
            // OnOldMapDestroyed() callback, I must face the unfortunate truth that
            // the performance cost is just not worth it. My soul weeps in pain and
            // my heart yearns for closure that I must accept - will never be mine.

            m_TacMapComponent.Dispose();
            m_TacMapComponent = null;
        }
    }
}