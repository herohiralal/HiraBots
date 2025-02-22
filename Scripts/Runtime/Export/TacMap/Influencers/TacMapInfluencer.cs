using HiraBots;

namespace UnityEngine.AI
{
    public abstract class TacMapInfluencer : MonoBehaviour, IUpdatableBehaviour
    {
        [Tooltip("The map to apply the influence on.")]
        [SerializeField] private TacMap m_Map = null;

        [Tooltip("The ticking interval to check whether the influence needs to be re-evaluated. Negative value means no auto-update.")]
        [SerializeField] private float m_TickInterval = 0f;

        [Tooltip("Whether to update the tac map synchronously.")]
        [SerializeField] private bool m_UpdateSynchronously = false;

        [System.NonSerialized] private TacMap m_CurrentlyUsedMap = null;
        [System.NonSerialized] private bool m_Dirty = false;

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                map = m_Map;
                tickInterval = m_TickInterval;
            }

            OnValidateCallback();
        }

        protected void OnEnable()
        {
            StartUsingNewMap();
            OnEnableCallback();
        }

        protected void OnDisable()
        {
            OnDisableCallback();
            StopUsingOldMap();
        }

        public TacMap map
        {
            get => m_Map;
            set
            {
                m_Map = value;

                if (!isActiveAndEnabled)
                {
                    return;
                }

                if (!ReferenceEquals(m_Map, m_CurrentlyUsedMap))
                {
                    StopUsingOldMap();
                    StartUsingNewMap();
                }
            }
        }

        public float tickInterval
        {
            get => m_TickInterval;
            set
            {
                m_TickInterval = value;

                if (!isActiveAndEnabled || string.IsNullOrWhiteSpace(gameObject.scene.path))
                {
                    return;
                }

                if (value < 0f)
                {
                    BehaviourUpdater.Remove(this);
                }
                else
                {
                    BehaviourUpdater.Add(this, m_TickInterval);
                }
            }
        }

        public bool updateSynchronously
        {
            get => m_UpdateSynchronously;
            set => m_UpdateSynchronously = value;
        }

        public void Tick(float deltaTime)
        {
            if (ReferenceEquals(m_Map, null))
            {
                return;
            }

            try
            {
                TickCallback(deltaTime);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

            if (m_Dirty)
            {
                RemoveInfluenceFromMap();
                AddInfluenceToMap();

                m_Dirty = false;
            }
        }

        // repaint influence on the newly created map
        internal void OnNewMapCreated()
        {
            try
            {
                AddInfluenceToMap();
            }
            catch (System.Exception e)
            {
                Debug.LogError(e, this);
            }

            m_Dirty = false;
        }

        // to clear up the references, for GC
        internal void OnMapDestroy()
        {
            m_CurrentlyUsedMap = null;
            m_Map = null;
            BehaviourUpdater.Remove(this);
        }

        private void StartUsingNewMap()
        {
            if (m_CurrentlyUsedMap != null)
            {
                return;
            }

            if (m_Map == null)
            {
                return;
            }

            m_CurrentlyUsedMap = m_Map;
            m_CurrentlyUsedMap.influencers.Add(this);
            BehaviourUpdater.Add(this, m_TickInterval);

            try
            {
                AddInfluenceToMap();
            }
            catch (System.Exception e)
            {
                Debug.LogError(e, this);
            }

            m_Dirty = false;
        }

        private void StopUsingOldMap()
        {
            if (m_CurrentlyUsedMap == null)
            {
                return;
            }

            try
            {
                RemoveInfluenceFromMap();
            }
            catch (System.Exception e)
            {
                Debug.LogError(e, this);
            }

            BehaviourUpdater.Remove(this);
            m_CurrentlyUsedMap.influencers.Remove(this);
            m_CurrentlyUsedMap = null;
        }

        protected virtual void OnValidateCallback()
        {
        }

        protected virtual void OnEnableCallback()
        {
        }

        protected virtual void OnDisableCallback()
        {
        }

        protected virtual void TickCallback(float deltaTime)
        {
        }

        protected abstract void AddInfluenceToMap();
        protected abstract void RemoveInfluenceFromMap();

        protected void MarkDirty()
        {
            m_Dirty = true;
        }
    }
}