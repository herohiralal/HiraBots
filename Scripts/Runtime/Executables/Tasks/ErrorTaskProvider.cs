using UnityEngine;

namespace HiraBots
{
    internal sealed partial class ErrorTaskProvider : HiraBotsTaskProvider
    {
        [Tooltip("Whether to disable entry into play mode or building a player.")]
        [SerializeField] private bool m_DisablePlayModeEntryOrBuildingPlayer;

        [TextArea, Tooltip("The error to display.")]
        [SerializeField] private string m_Text = "Insert error here that will help you recognize this node from a debug message.";

        public override IHiraBotsTask GetTask(UnityEngine.BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            return ErrorExecutable.Get(m_Text);
        }

        internal static ErrorTaskProvider noneTaskProvider { get; private set; }
        internal static ReadOnlyArrayAccessor<HiraBotsTaskProvider> emptyTaskProviders { get; private set; }

        internal static void CreateNoneTaskInstance()
        {
            noneTaskProvider = CreateInstance<ErrorTaskProvider>();
            noneTaskProvider.m_DisablePlayModeEntryOrBuildingPlayer = false;
            noneTaskProvider.m_Text = "Missing task provider.";

            emptyTaskProviders = new HiraBotsTaskProvider[] { noneTaskProvider }.ReadOnly();
        }

        internal static void ClearNoneTaskInstance()
        {
            emptyTaskProviders = new HiraBotsTaskProvider[0].ReadOnly();

            Destroy(noneTaskProvider);
            noneTaskProvider = null;
        }
    }
}