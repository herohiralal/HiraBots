namespace UnityEngine
{
    public abstract class TacMapInfluencer : MonoBehaviour
    {
        [SerializeField] private TacMap m_Map = null;

        [System.NonSerialized] private TacMap m_CurrentlyUsedMap = null;
        [System.NonSerialized] private bool m_Dirty = false;

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

                StopUsingOldMap();
                StartUsingNewMap();
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
        }

        private void StartUsingNewMap()
        {
            if (m_CurrentlyUsedMap != null)
            {
                return;
            }

            m_CurrentlyUsedMap = m_Map;
            m_CurrentlyUsedMap.m_Influencers.Add(this);

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

            m_CurrentlyUsedMap.m_Influencers.Remove(this);
            m_CurrentlyUsedMap = null;
        }

        protected virtual void OnEnableCallback()
        {
        }

        protected virtual void OnDisableCallback()
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