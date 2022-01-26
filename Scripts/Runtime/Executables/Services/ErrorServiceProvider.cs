using UnityEngine;
using UnityEngine.AI;

namespace HiraBots
{
    internal sealed partial class ErrorServiceProvider : HiraBotsServiceProvider
    {
        [Tooltip("Whether to disable entry into play mode or building a player.")]
        [SerializeField] private bool m_DisablePlayModeEntryOrBuildingPlayer;

        [TextArea, Tooltip("The error to display.")]
        [SerializeField] private string m_Text = "Insert error here that will help you recognize this node from a debug message.";

        protected override IHiraBotsService GetService(UnityEngine.AI.BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            return ErrorExecutable.Get(m_Text);
        }
    }
}