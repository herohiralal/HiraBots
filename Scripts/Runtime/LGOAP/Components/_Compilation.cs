namespace HiraBots
{
    internal sealed partial class LGOAPGoal
    {
        internal void PrepareForCompilation()
        {
            m_Insistence.PrepareForCompilation();
            m_Target.PrepareForCompilation(false);
        }
    }

    internal sealed partial class LGOAPTask
    {
        internal void PrepareForCompilation()
        {
            m_Action.PrepareForCompilation();
            m_Target.PrepareForCompilation(m_IsAbstract);
        }
    }
}