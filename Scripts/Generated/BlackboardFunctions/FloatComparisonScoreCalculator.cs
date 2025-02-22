// ---------------------------------------------------------------------
// <auto-generated>
// 
// This file has been automatically generated.
// 
// Any changes to it will be lost upon regeneration.
// 
// Consider extending partial classes in ManualExtensions folder.
// 
// </auto-generated>
// ---------------------------------------------------------------------

namespace UnityEngine.AI
{
    [Unity.Burst.BurstCompile]
    public unsafe partial class FloatComparisonScoreCalculator : HiraBotsScoreCalculatorBlackboardFunction
    {
        private struct Memory
        {
            internal System.Boolean _invert;
            internal ushort _key;
            internal System.Single _secondValue;
            internal System.Single _equalityTolerance;
            internal HiraBots.SampleBlackboardFunctionsFloatComparisonType _comparisonType;
            internal System.Single _score;
        }

        [SerializeField] internal System.Boolean invert;
        [SerializeField] internal BlackboardTemplate.KeySelector key;
        [SerializeField] internal System.Single secondValue;
        [SerializeField] internal System.Single equalityTolerance;
        [SerializeField] internal HiraBots.SampleBlackboardFunctionsFloatComparisonType comparisonType;
        [SerializeField] internal System.Single score;

        // pack memory
        private Memory memory => new Memory
        { _invert = invert, _key = key.selectedKey.offset, _secondValue = secondValue, _equalityTolerance = equalityTolerance, _comparisonType = comparisonType, _score = score };

        #region Execution

        // actual function
        [Unity.Burst.BurstCompile(DisableDirectCall = true), AOT.MonoPInvokeCallback(typeof(Delegate))]
        private static float ActualFunction(in BlackboardComponent.LowLevel blackboard, byte* rawMemory, float currentScore)
        {
            var memory = (Memory*) rawMemory;
            return HiraBots.SampleScoreCalculatorBlackboardFunctions.FloatComparisonScoreCalculator(currentScore, memory->_invert, ref blackboard.Access<float>(memory->_key), memory->_secondValue, memory->_equalityTolerance, memory->_comparisonType, memory->_score);
        }

        // non-VM execution
        protected override float ExecuteFunction(BlackboardComponent blackboard, bool expected, float currentScore)
        {
            var _key = blackboard.GetFloatValue(key.selectedKey.name); var output = HiraBots.SampleScoreCalculatorBlackboardFunctions.FloatComparisonScoreCalculator(currentScore, invert, ref _key, secondValue, equalityTolerance, comparisonType, score); return output;
        }

        #endregion

        #region Compilation

        private static bool s_ActualFunctionCompiled = false;
        private static Unity.Burst.FunctionPointer<Delegate> s_ActualFunction;

        public override void PrepareForCompilation()
        {
            base.PrepareForCompilation();
            m_MemorySize += sizeof(Memory); // this will also help make sure the packed memory stays unmanaged

            if (!s_ActualFunctionCompiled)
            {
                s_ActualFunction = Unity.Burst.BurstCompiler.CompileFunctionPointer<Delegate>(ActualFunction);
                s_ActualFunctionCompiled = true;
            }

            functionPtr = s_ActualFunction.Value;
        }

        // compile override
        public override void Compile(ref byte* stream)
        {
            CompilationRegistry.IncreaseDepth();

            var start = stream;

            base.Compile(ref stream);

            *(Memory*) stream = memory;
            stream += sizeof(Memory);

            CompilationRegistry.AddEntry(name, start, stream);

            CompilationRegistry.DecreaseDepth();
        }

        #endregion

#if UNITY_EDITOR
        #region EditorOnlyInterface

        protected override void OnTargetBlackboardTemplateChanged(BlackboardTemplate template, in BlackboardTemplate.KeySet keySet)
        {
            key.OnTargetBlackboardTemplateChanged(template, in keySet);
        }

        protected override void OnValidateCallback()
        {
            key.keyTypesFilter = UnityEngine.AI.BlackboardKeyType.Invalid | UnityEngine.AI.BlackboardKeyType.Float;
            // no external validator
        }

        protected override void UpdateDescription(out string staticDescription)
        {
            HiraBots.SampleScoreCalculatorBlackboardFunctions.FloatComparisonScoreCalculatorUpdateDescription(invert, key, secondValue, equalityTolerance, comparisonType, score, out staticDescription);
        }

        #endregion
#endif

#if UNITY_EDITOR || HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
        #region Validation

        public override void Validate(ref ValidatorContext context)
        {
            base.Validate(ref context);
            ValidateKeySelector(ref key, UnityEngine.AI.BlackboardKeyType.Invalid | UnityEngine.AI.BlackboardKeyType.Float, ref context, nameof(key));
        }

        #endregion
#endif
    }
}