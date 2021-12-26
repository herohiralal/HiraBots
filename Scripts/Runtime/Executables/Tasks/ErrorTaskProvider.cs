using UnityEngine;

namespace HiraBots
{
    internal sealed partial class ErrorTaskProvider : HiraBotsTaskProvider
    {
        [Tooltip("Whether to disable entry into play mode or building a player.")]
        [SerializeField] private bool m_DisablePlayModeEntryOrBuildingPlayer;

        [TextArea, Tooltip("The error to display.")]
        [SerializeField] private string m_Text = "Insert error here that will help you recognize this node from a debug message.";

        public override IHiraBotsTask GetTask(UnityEngine.BlackboardComponent blackboard)
        {
            return ErrorExecutable.Get(m_Text);
        }
    }
}