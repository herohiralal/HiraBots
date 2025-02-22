using System.Linq;

namespace HiraBots
{
    internal partial struct LGOAPGoalLayer
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

            m_Size = 0 // no header
                     + m_InsistenceCompilationHelper.GetMemorySizeRequiredForCompilation()
                     + m_TargetCompilationHelper.GetMemorySizeRequiredForCompilation();
        }

        internal int GetMemorySizeRequiredForCompilation()
        {
            return m_Size;
        }

        internal unsafe void Compile(ref byte* stream)
        {
            CompilationRegistry.IncreaseDepth();

            var insistenceStartAddress = stream;
            m_InsistenceCompilationHelper.Compile(ref stream);
            CompilationRegistry.AddEntry("Insistence Collection", insistenceStartAddress, stream);

            var targetStartAddress = stream;
            m_TargetCompilationHelper.Compile(ref stream);
            CompilationRegistry.AddEntry("Target Collection", targetStartAddress, stream);

            CompilationRegistry.DecreaseDepth();
        }
    }

    internal partial struct LGOAPTaskLayer
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

            m_Size = 0 // no header
                     + m_ActionCompilationHelper.GetMemorySizeRequiredForCompilation()
                     + m_TargetCompilationHelper.GetMemorySizeRequiredForCompilation();
        }

        internal int GetMemorySizeRequiredForCompilation()
        {
            return m_Size;
        }

        internal unsafe void Compile(ref byte* stream)
        {
            CompilationRegistry.IncreaseDepth();

            var actionStartAddress = stream;
            m_ActionCompilationHelper.Compile(ref stream);
            CompilationRegistry.AddEntry("Action Collection", actionStartAddress, stream);

            var targetStartAddress = stream;
            m_TargetCompilationHelper.Compile(ref stream);
            CompilationRegistry.AddEntry("Target Collection", targetStartAddress, stream);

            CompilationRegistry.DecreaseDepth();
        }
    }
}