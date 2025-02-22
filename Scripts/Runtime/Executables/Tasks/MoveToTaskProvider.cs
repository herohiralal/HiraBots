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

            if (m_TrackMovingTarget)
            {
                System.Func<UnityEngine.AI.BlackboardComponent, NavMeshAgent, string, float, float, IHiraBotsTask> func;
                switch (key.keyType)
                {
                    case UnityEngine.AI.BlackboardKeyType.Vector:
                        func = MoveToTrackedVectorTask.Get;
                        break;
                    case UnityEngine.AI.BlackboardKeyType.Object:
                        var obj = blackboard.GetObjectValue(key.name);
                        switch (obj)
                        {
                            case Transform _:
                                func = MoveToTrackedTransformTask.Get;
                                break;
                            case GameObject _:
                                func = MoveToTrackedGameObjectTask.Get;
                                break;
                            case Component _:
                                func = MoveToTrackedComponentTask.Get;
                                break;
                            default:
                                return ErrorExecutable.Get($"Cannot get a position from object of type {obj.GetType()}.");
                        }

                        break;
                    default:
                        return ErrorExecutable.Get($"Can not execute {name}.");
                }

                return func(
                    blackboard,
                    navigatingAgent.component,
                    m_Target.selectedKey.name,
                    m_Tolerance,
                    m_Speed);
            }
            else
            {
                Vector3 location;
                switch (key.keyType)
                {
                    case UnityEngine.AI.BlackboardKeyType.Vector:
                        location = blackboard.GetVectorValue(key.name);
                        break;
                    case UnityEngine.AI.BlackboardKeyType.Object:
                        var obj = blackboard.GetObjectValue(key.name);
                        switch (obj)
                        {
                            case Transform t:
                                location = t.position;
                                break;
                            case GameObject g:
                                location = g.transform.position;
                                break;
                            case Component c:
                                location = c.transform.position;
                                break;
                            default:
                                return ErrorExecutable.Get($"Cannot get a position from object of type {obj.GetType()}.");
                        }

                        break;
                    default:
                        return ErrorExecutable.Get($"Can not execute {name}.");
                }

                return MoveToLocationTask.Get(
                    navigatingAgent.component,
                    location,
                    m_Tolerance,
                    m_Speed);
            }
        }
    }
}