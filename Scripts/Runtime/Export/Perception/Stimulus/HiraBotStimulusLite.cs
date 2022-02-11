using System;
using System.Collections;
using HiraBots;

namespace UnityEngine.AI
{
    public struct HiraBotStimulusLite : IDisposable
    {
        public HiraBotStimulusLite(StimulusType type, Vector3 position, Object associatedObject)
        {
            m_Id = PerceptionSystem.AddStimulus(type.ToTypeIndex(), position, associatedObject.GetInstanceID());
        }

        private int m_Id;

        public StimulusType type
        {
            set => PerceptionSystem.ChangeStimulusType(m_Id, value.ToTypeIndex());
        }

        public Vector3 position
        {
            set => PerceptionSystem.ChangeStimulusPosition(m_Id, value);
        }

        public Object associatedObject
        {
            set => PerceptionSystem.ChangeStimulusAssociatedObject(m_Id, value.GetInstanceID());
        }

        public void Dispose()
        {
            PerceptionSystem.RemoveStimulus(m_Id);
            m_Id = 0;
        }

        public void Dispose(float timer)
        {
            CoroutineRunner.Start(DisposeCoroutine(m_Id, timer));
        }

        private static IEnumerator DisposeCoroutine(int id, float timer)
        {
            yield return new WaitForSeconds(timer);
            PerceptionSystem.RemoveStimulus(id);
        }
    }
}