#if HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
namespace HiraBots
{
    internal partial class BlackboardTemplate
    {
        /// <summary>
        /// Build a BlackboardTemplate.
        /// </summary>
        internal void BuildBlackboardTemplate(BlackboardTemplate parent, BlackboardKey[] keys)
        {
            m_Parent = parent;
            m_Keys = keys;
        }
    }
}
#endif