namespace HiraBots
{
    internal unsafe partial struct LGOAPInsistence : ILowLevelObjectProvider
    {
        [UnityEngine.SerializeField] internal UnityEngine.Object m_Owner;
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

            CompilationRegistry.IncreaseDepth();
            var calculatorCollectionStart = stream;
            m_CompilationHelper.Compile(ref stream);
            CompilationRegistry.AddEntry("Insistence Score Calculator Collection", calculatorCollectionStart, stream);
            CompilationRegistry.DecreaseDepth();

            CompilationRegistry.AddEntry(m_Owner.name, start, stream);
            CompilationRegistry.DecreaseDepth();
        }
    }

    internal unsafe partial struct LGOAPTarget : ILowLevelObjectProvider
    {
        [UnityEngine.SerializeField] internal UnityEngine.Object m_Owner;
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

            CompilationRegistry.IncreaseDepth();

            var sizeStart = stream;
            ByteStreamHelpers.Write<int>(ref stream, m_ActualMemorySize);
            CompilationRegistry.AddEntry("Size", sizeStart, stream);

            var isFakeStart = stream;
            ByteStreamHelpers.Write<bool>(ref stream, m_IsFake);
            CompilationRegistry.AddEntry("Fake Check", isFakeStart, stream);

            var decoratorStart = stream;
            m_CompilationHelper.Compile(ref stream);
            CompilationRegistry.AddEntry("Target Decorator Collection", decoratorStart, stream);

            CompilationRegistry.DecreaseDepth();

            CompilationRegistry.AddEntry(m_Owner.name, start, stream);

            CompilationRegistry.DecreaseDepth();
        }
    }

    internal unsafe partial struct LGOAPAction : ILowLevelObjectProvider
    {
        [UnityEngine.SerializeField] internal UnityEngine.Object m_Owner;
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

            CompilationRegistry.IncreaseDepth();

            var sizeStart = stream;
            ByteStreamHelpers.Write<int>(ref stream, m_Size); // size header
            CompilationRegistry.AddEntry("Size", sizeStart, stream);

            var preconditionStart = stream;
            m_PreconditionCompiler.Compile(ref stream);
            CompilationRegistry.AddEntry("Precondition Decorator Collection", preconditionStart, stream);

            var costStart = stream;
            m_CostCompiler.Compile(ref stream);
            CompilationRegistry.AddEntry("Cost Score Calculator Collection", costStart, stream);

            var effectStart = stream;
            m_EffectCompiler.Compile(ref stream);
            CompilationRegistry.AddEntry("Effect Effector Collection", effectStart, stream);

            CompilationRegistry.DecreaseDepth();

            CompilationRegistry.AddEntry(m_Owner.name, start, stream);

            CompilationRegistry.DecreaseDepth();
        }
    }
}