using UnityEngine;

namespace HiraBots
{
    internal sealed partial class WaitTaskProvider : HiraBotsTaskProvider
    {
        [SerializeField] private float m_Timer;
        [SerializeField] private float m_RandomDeviation;

        public override IHiraBotsTask GetTask(UnityEngine.BlackboardComponent blackboard)
        {
            return WaitTask.Get(m_Timer);
        }
    }
}