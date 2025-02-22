using UnityEngine;
using UnityEngine.AI;

namespace HiraBots
{
    internal sealed partial class WaitBlackboardTimeTaskProvider : HiraBotsTaskProvider
    {
        [Tooltip("The amount of time to wait.")]
        [SerializeField] private UnityEngine.AI.BlackboardTemplate.KeySelector m_Timer;

        protected override IHiraBotsTask GetTask(UnityEngine.AI.BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            switch (m_Timer.selectedKey.keyType)
            {
                case UnityEngine.AI.BlackboardKeyType.Float:
                    return WaitTask.Get(blackboard.GetFloatValue(m_Timer.selectedKey.name));
                case UnityEngine.AI.BlackboardKeyType.Integer:
                    return WaitTask.Get(blackboard.GetIntegerValue(m_Timer.selectedKey.name));
                default:
                    return ErrorExecutable.Get($"Can not execute {name} because a correct key is not selected.");
            }
        }
    }
}