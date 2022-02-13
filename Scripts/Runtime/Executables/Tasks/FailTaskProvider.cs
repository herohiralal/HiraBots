using UnityEngine.AI;

namespace HiraBots
{
    internal class FailTaskProvider : HiraBotsTaskProvider
    {
        protected override IHiraBotsTask GetTask(UnityEngine.AI.BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            return FailTask.Get();
        }
    }
}