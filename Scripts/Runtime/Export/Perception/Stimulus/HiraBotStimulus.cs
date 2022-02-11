using HiraBots;

namespace UnityEngine.AI
{
    public sealed class HiraBotStimulus : MonoBehaviour, IUpdatableBehaviour
    {
        [Tooltip("The type of the stimulus.")]
        [SerializeField] private StimulusType m_Type;

        [Tooltip("The interval at which to update the position in the stimulus database.")]
        [SerializeField] private float m_TickInterval;

        private int m_Id;

        private void OnEnable()
        {
            m_Id = PerceptionSystem.AddStimulus(m_Type.ToTypeIndex(), transform.position, gameObject.GetInstanceID());
            BehaviourUpdater.Add(this, m_TickInterval);
        }

        private void OnDisable()
        {
            BehaviourUpdater.Remove(this);
            PerceptionSystem.RemoveStimulus(m_Id);
            m_Id = 0;
        }

        private void OnValidate()
        {
            type = m_Type;
            tickInterval = m_TickInterval;
        }

        public StimulusType type
        {
            get => m_Type;
            set
            {
                if (m_Id != 0 && m_Type.ToTypeIndex() != value.ToTypeIndex())
                {
                    PerceptionSystem.ChangeStimulusType(m_Id, value.ToTypeIndex());
                }

                m_Type = value;
            }
        }

        public float tickInterval
        {
            get => m_TickInterval;
            set
            {
                var clampedValue = Mathf.Clamp(value, 0f, float.MaxValue);

                if (m_Id != 0 && isActiveAndEnabled && Mathf.Abs(m_TickInterval - clampedValue) < 0.01f)
                {
                    BehaviourUpdater.ChangeTickInterval(this, clampedValue);
                }

                m_TickInterval = clampedValue;
            }
        }

        public void Tick(float deltaTime)
        {
            PerceptionSystem.ChangeStimulusPosition(m_Id, transform.position);
        }
    }
}