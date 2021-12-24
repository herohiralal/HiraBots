using UnityEngine;

namespace HiraBots
{
    internal sealed class WaitTaskProvider : HiraBotsTaskProvider
    {
        [SerializeField] private float m_Timer;

        public override IHiraBotsTask GetTask(UnityEngine.BlackboardComponent blackboard)
        {
            return WaitTask.Get(m_Timer);
        }
    }
}