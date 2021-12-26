using UnityEngine;

namespace HiraBots
{
    internal sealed partial class ErrorServiceProvider : HiraBotsServiceProvider
    {
        [SerializeField] private bool m_DisablePlayModeEntryOrBuildingPlayer;

        [TextArea]
        [SerializeField] private string m_Text = "Insert error here that will help you recognize this node from a debug message.";

        public override IHiraBotsService GetService(UnityEngine.BlackboardComponent blackboard)
        {
            return ErrorExecutable.Get(m_Text);
        }
    }
}