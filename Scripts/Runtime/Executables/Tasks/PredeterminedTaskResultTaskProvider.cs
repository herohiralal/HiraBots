using UnityEngine;
using UnityEngine.AI;

namespace HiraBots
{
    internal partial class PredeterminedTaskResultTaskProvider : HiraBotsTaskProvider
    {
        [SerializeField] private HiraBotsTaskResult m_Result = HiraBotsTaskResult.InProgress;

        protected override IHiraBotsTask GetTask(UnityEngine.AI.BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            return PredeterminedTaskResultTask.Get(m_Result);
        }
    }
}