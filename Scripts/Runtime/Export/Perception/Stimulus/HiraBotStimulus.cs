using System;
using HiraBots;

namespace UnityEngine.AI
{
    public struct HiraBotStimulus : IDisposable
    {
        public HiraBotStimulus(StimulusType type, Vector3 position, Object associatedObject)
        {
            m_TypeIndex = type.ToTypeIndex();
            m_Position = position;
            m_AssociatedObjectInstanceID = associatedObject.GetInstanceID();
            m_Id = PerceptionSystem.AddStimulus(m_TypeIndex, m_Position, m_AssociatedObjectInstanceID);
        }

        private int m_Id;
        private byte m_TypeIndex;
        private Vector3 m_Position;
        private int m_AssociatedObjectInstanceID;

        public StimulusType type
        {
            get => new StimulusType(m_TypeIndex);
            set
            {
                var typeIndex = value.ToTypeIndex();

                if (m_Id != 0 && m_TypeIndex != typeIndex)
                {
                    PerceptionSystem.ChangeStimulusType(m_Id, typeIndex);
                }

                m_TypeIndex = typeIndex;
            }
        }

        public Vector3 position
        {
            get => m_Position;
            set
            {
                if (m_Id != 0 && m_Position != value)
                {
                    PerceptionSystem.ChangeStimulusPosition(m_Id, value);
                }

                m_Position = value;
            }
        }

        public Object associatedObject
        {
            get => ObjectUtils.InstanceIDToObject(m_AssociatedObjectInstanceID);
            set
            {
                var instanceID = value.GetInstanceID();

                if (m_Id != 0 && m_AssociatedObjectInstanceID != instanceID)
                {
                    PerceptionSystem.ChangeStimulusAssociatedObject(m_Id, instanceID);
                }

                m_AssociatedObjectInstanceID = instanceID;
            }
        }

        public void Dispose()
        {
            PerceptionSystem.RemoveStimulus(m_Id);
            m_Id = 0;
        }
    }
}