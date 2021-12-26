#if UNITY_EDITOR
namespace HiraBots
{
    internal partial class ErrorServiceProvider
    {
        protected override void UpdateDescription(out string staticDescription)
        {
            staticDescription = m_DisablePlayModeEntryOrBuildingPlayer
                ? "Disable play mode entry or building a player and log the error."
                : "Log the error upon service start.";
        }
    }
}
#endif