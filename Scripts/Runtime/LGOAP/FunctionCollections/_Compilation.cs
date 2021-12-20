namespace HiraBots
{
    internal unsafe partial struct LGOAPInsistence : ILowLevelObjectProvider
    {
        [System.NonSerialized] internal UnityEngine.Object m_Owner;
        [System.NonSerialized] private DefaultLowLevelObjectProviderCollection<DecoratorBlackboardFunction> m_CompilationHelper;

        internal void PrepareForCompilation()
        {
            foreach (var scoreCalculator in m_Insistence)
            {
                scoreCalculator.PrepareForCompilation();
            }

            m_CompilationHelper = m_Insistence.GetLowLevelObjectProviderCollection();
        }

        public int GetMemorySizeRequiredForCompilation()
        {
            return m_CompilationHelper.GetMemorySizeRequiredForCompilation();
        }

        public void Compile(ref byte* stream)
        {
            CompilationRegistry.IncreaseDepth();
            var start = stream;

            m_CompilationHelper.Compile(ref stream);

            CompilationRegistry.AddEntry(m_Owner.name, start, stream);
            CompilationRegistry.DecreaseDepth();
        }
    }

    internal unsafe partial struct LGOAPTarget : ILowLevelObjectProvider
    {
        [System.NonSerialized] internal UnityEngine.Object m_Owner;
        [System.NonSerialized] private DefaultLowLevelObjectProviderCollection<DecoratorBlackboardFunction> m_CompilationHelper;
        [System.NonSerialized] private int m_ActualMemorySize;
        [System.NonSerialized] private bool m_IsFake;

        internal void PrepareForCompilation(bool isFake)
        {
            foreach (var decorator in m_Target)
            {
                decorator.PrepareForCompilation();
            }

            m_IsFake = isFake;

            m_CompilationHelper = m_Target.GetLowLevelObjectProviderCollection();

            var requiredSize = ByteStreamHelpers.CombinedSizes<int, bool>() // size & fake header
                               + m_CompilationHelper.GetMemorySizeRequiredForCompilation();

            m_ActualMemorySize = requiredSize;
        }

        public int GetMemorySizeRequiredForCompilation()
        {
            return m_ActualMemorySize;
        }

        public void Compile(ref byte* stream)
        {
            CompilationRegistry.IncreaseDepth();
            var start = stream;

            ByteStreamHelpers.Write<int>(ref stream, m_ActualMemorySize);

            ByteStreamHelpers.Write<bool>(ref stream, m_IsFake);

            m_CompilationHelper.Compile(ref stream);

            CompilationRegistry.AddEntry(m_Owner.name, start, stream);

            CompilationRegistry.DecreaseDepth();
        }
    }

    internal unsafe partial struct LGOAPAction : ILowLevelObjectProvider
    {
        [System.NonSerialized] internal UnityEngine.Object m_Owner;
        [System.NonSerialized] private DefaultLowLevelObjectProviderCollection<DecoratorBlackboardFunction> m_PreconditionCompiler;
        [System.NonSerialized] private DefaultLowLevelObjectProviderCollection<DecoratorBlackboardFunction> m_CostCompiler;
        [System.NonSerialized] private DefaultLowLevelObjectProviderCollection<EffectorBlackboardFunction> m_EffectCompiler;
        [System.NonSerialized] private int m_Size;

        internal void PrepareForCompilation()
        {
            foreach (var decorator in m_Precondition)
            {
                decorator.PrepareForCompilation();
            }

            foreach (var scoreCalculator in m_Cost)
            {
                scoreCalculator.PrepareForCompilation();
            }

            foreach (var effector in m_Effect)
            {
                effector.PrepareForCompilation();
            }

            m_PreconditionCompiler = m_Precondition.GetLowLevelObjectProviderCollection();
            m_CostCompiler = m_Cost.GetLowLevelObjectProviderCollection();
            m_EffectCompiler = m_Effect.GetLowLevelObjectProviderCollection();

            var size = ByteStreamHelpers.CombinedSizes<int>(); // size header

            size += m_PreconditionCompiler.GetMemorySizeRequiredForCompilation();
            size += m_CostCompiler.GetMemorySizeRequiredForCompilation();
            size += m_EffectCompiler.GetMemorySizeRequiredForCompilation();

            m_Size = size;
        }

        public int GetMemorySizeRequiredForCompilation()
        {
            return m_Size;
        }

        public void Compile(ref byte* stream)
        {
            CompilationRegistry.IncreaseDepth();
            var start = stream;

            ByteStreamHelpers.Write<int>(ref stream, m_Size); // size header

            m_PreconditionCompiler.Compile(ref stream);

            m_CostCompiler.Compile(ref stream);

            m_EffectCompiler.Compile(ref stream);

            CompilationRegistry.AddEntry(m_Owner.name, start, stream);

            CompilationRegistry.DecreaseDepth();
        }
    }
}