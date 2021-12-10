using System.Linq;

namespace HiraBots
{
    internal partial struct LGOAPGoalLayer : ILowLevelObjectProvider
    {
        [System.NonSerialized] private int m_Size;
        [System.NonSerialized] private DefaultLowLevelObjectProviderCollection<LGOAPInsistence> m_InsistenceCompilationHelper;
        [System.NonSerialized] private DefaultLowLevelObjectProviderCollection<LGOAPTarget> m_TargetCompilationHelper;

        internal void PrepareForCompilation()
        {
            foreach (var goal in m_Goals)
            {
                goal.PrepareForCompilation();
            }

            m_InsistenceCompilationHelper = m_Goals.Select(g => g.insistence).ToArray().GetLowLevelObjectProviderCollection();
            m_TargetCompilationHelper = m_Goals.Select(g => g.target).ToArray().GetLowLevelObjectProviderCollection();

            m_Size = ByteStreamHelpers.CombinedSizes<int>() // total size header
                     + m_InsistenceCompilationHelper.GetMemorySizeRequiredForCompilation()
                     + m_TargetCompilationHelper.GetMemorySizeRequiredForCompilation();
        }

        public int GetMemorySizeRequiredForCompilation()
        {
            return m_Size;
        }

        public unsafe void Compile(ref byte* stream)
        {
            ByteStreamHelpers.Write<int>(ref stream, m_Size);

            m_InsistenceCompilationHelper.Compile(ref stream);

            m_TargetCompilationHelper.Compile(ref stream);
        }
    }

    internal partial struct LGOAPTaskLayer : ILowLevelObjectProvider
    {
        [System.NonSerialized] private int m_Size;
        [System.NonSerialized] private DefaultLowLevelObjectProviderCollection<LGOAPAction> m_ActionCompilationHelper;
        [System.NonSerialized] private DefaultLowLevelObjectProviderCollection<LGOAPTarget> m_TargetCompilationHelper;

        internal void PrepareForCompilation()
        {
            foreach (var task in m_Tasks)
            {
                task.PrepareForCompilation();
            }

            m_ActionCompilationHelper = m_Tasks.Select(t => t.action).ToArray().GetLowLevelObjectProviderCollection();
            m_TargetCompilationHelper = m_Tasks.Select(t => t.target).ToArray().GetLowLevelObjectProviderCollection();

            m_Size = ByteStreamHelpers.CombinedSizes<int>() // total size header
                     + m_ActionCompilationHelper.GetMemorySizeRequiredForCompilation()
                     + m_TargetCompilationHelper.GetMemorySizeRequiredForCompilation();
        }

        public int GetMemorySizeRequiredForCompilation()
        {
            return m_Size;
        }

        public unsafe void Compile(ref byte* stream)
        {
            ByteStreamHelpers.Write<int>(ref stream, m_Size);

            m_ActionCompilationHelper.Compile(ref stream);

            m_TargetCompilationHelper.Compile(ref stream);
        }
    }
}