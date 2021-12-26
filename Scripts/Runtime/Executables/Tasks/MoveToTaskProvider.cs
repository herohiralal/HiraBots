using UnityEngine;
using UnityEngine.AI;

namespace HiraBots
{
    internal sealed partial class MoveToTaskProvider : HiraBotsTaskProvider
    {
        [Tooltip("Target location.")]
        [SerializeField] private UnityEngine.BlackboardTemplate.KeySelector m_Target;

        [Tooltip("Tolerance for reaching the destination.")]
        [SerializeField] private float m_Tolerance = 0.25f;

        [Tooltip("The speed at which to move the agent.")]
        [SerializeField] private float m_Speed = 3f;

        public override IHiraBotsTask GetTask(UnityEngine.BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            var key = m_Target.selectedKey;

            switch (key.keyType)
            {
                case UnityEngine.BlackboardKeyType.Vector when archetype is IHiraBotArchetype<NavMeshAgent> navigatingAgent:
                    var location = blackboard.GetVectorValue(key.name);
                    return MoveToLocationTask.Get(navigatingAgent.component, location, m_Tolerance, m_Speed);
                default:
                    return ErrorExecutable.Get($"Can not execute {name}.");
            }
        }
    }
}