using UnityEngine;
using UnityEngine.AI;

namespace HiraBots
{
    internal sealed partial class MoveToTaskProvider : HiraBotsTaskProvider
    {
        [Tooltip("Target location.")]
        [SerializeField] private UnityEngine.AI.BlackboardTemplate.KeySelector m_Target;

        [Tooltip("Track moving target.")]
        [SerializeField] private bool m_TrackMovingTarget = false;

        [Tooltip("Tolerance for reaching the destination.")]
        [SerializeField] private float m_Tolerance = 0.25f;

        [Tooltip("The speed at which to move the agent.")]
        [SerializeField] private float m_Speed = 3f;

        protected override IHiraBotsTask GetTask(UnityEngine.AI.BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            var key = m_Target.selectedKey;

            if (!(archetype is IHiraBotArchetype<NavMeshAgent> navigatingAgent))
            {
                return ErrorExecutable.Get($"{archetype.gameObject.name} is not a navigating agent.");
            }

            switch (key.keyType)
            {
                case UnityEngine.AI.BlackboardKeyType.Vector when !m_TrackMovingTarget:
                    var location = blackboard.GetVectorValue(key.name);
                    return MoveToLocationTask.Get(navigatingAgent.component, location, m_Tolerance, m_Speed);
                case UnityEngine.AI.BlackboardKeyType.Vector when m_TrackMovingTarget:
                    return MoveToTrackedLocationTask.Get(blackboard, navigatingAgent.component, m_Target.selectedKey.name, m_Tolerance, m_Speed);
                default:
                    return ErrorExecutable.Get($"Can not execute {name}.");
            }
        }
    }
}