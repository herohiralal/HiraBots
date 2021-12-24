using UnityEngine;

namespace HiraBots
{
    internal sealed class ErrorTaskProvider : HiraBotsTaskProvider
    {
        [TextArea]
        [SerializeField] private string m_Text = "Insert error here that will help you recognize this node from a debug message.";

        public override IHiraBotsTask GetTask(UnityEngine.BlackboardComponent blackboard)
        {
            return ErrorExecutable.Get(m_Text);
        }
    }
}