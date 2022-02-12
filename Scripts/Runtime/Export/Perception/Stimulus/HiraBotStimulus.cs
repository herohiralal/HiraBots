using HiraBots;

namespace UnityEngine.AI
{
    [AddComponentMenu("AI/HiraBot Sensor Stimulus")]
    public sealed class HiraBotStimulus : MonoBehaviour, IUpdatableBehaviour
    {
        [Tooltip("The type of the stimulus.")]
        [SerializeField] private StimulusType m_Type;

        [Tooltip("The override for the associated object. Defaults to the GameObject this MonoBehaviour is attached to.")]
        [SerializeField] private Object m_AssociatedObjectOverride;

        [Tooltip("The interval at which to update the position in the stimulus database. Negative value means no auto-update.")]
        [SerializeField] private float m_TickInterval;

        private ulong m_Id;

        private void OnEnable()
        {
            m_Id = PerceptionSystem.AddStimulus(m_Type.ToTypeIndex(), transform.position, associatedObject.GetInstanceID());

            if (m_TickInterval >= 0f)
            {
                BehaviourUpdater.Add(this, m_TickInterval);
            }
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

        public Object associatedObject => ReferenceEquals(m_AssociatedObjectOverride, null) ? gameObject : m_AssociatedObjectOverride;

        public Object associatedObjectOverride
        {
            get => m_AssociatedObjectOverride;
            set
            {
                if (m_Id != 0 && m_AssociatedObjectOverride != value)
                {
                    PerceptionSystem.ChangeStimulusAssociatedObject(m_Id, associatedObject.GetInstanceID());
                }

                m_AssociatedObjectOverride = value;
            }
        }

        public float tickInterval
        {
            get => m_TickInterval;
            set
            {
                m_TickInterval = value;

                if (!isActiveAndEnabled)
                {
                    return;
                }

                if (value >= 0f)
                {
                    BehaviourUpdater.Add(this, value);
                }
                else
                {
                    BehaviourUpdater.Remove(this);
                }
            }
        }

        public void Tick(float deltaTime)
        {
            PerceptionSystem.ChangeStimulusPosition(m_Id, transform.position);
        }
    }
}