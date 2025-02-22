namespace HiraBots
{
    internal sealed partial class LGOAPGoal
    {
        internal void PrepareForCompilation()
        {
            m_Insistence.m_Owner = this;
            m_Insistence.PrepareForCompilation();

            m_Target.m_Owner = this;
            m_Target.PrepareForCompilation(false);
        }
    }

    internal sealed partial class LGOAPTask
    {
        internal void PrepareForCompilation()
        {
            m_Action.m_Owner = this;
            m_Action.PrepareForCompilation();

            m_Target.m_Owner = this;
            m_Target.PrepareForCompilation(!isAbstract);
        }
    }
}