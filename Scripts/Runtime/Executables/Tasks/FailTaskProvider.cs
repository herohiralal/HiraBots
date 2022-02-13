using UnityEngine.AI;

namespace HiraBots
{
    internal partial class FailTaskProvider : HiraBotsTaskProvider
    {
        protected override IHiraBotsTask GetTask(UnityEngine.AI.BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            return FailTask.Get();
        }
    }
}