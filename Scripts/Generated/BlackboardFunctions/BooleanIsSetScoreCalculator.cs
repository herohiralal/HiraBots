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

namespace UnityEngine
{
    public unsafe partial class BooleanIsSetScoreCalculator : HiraBotsScoreCalculatorBlackboardFunction
    {
        private struct Memory
        {
            internal System.Boolean _invert;
            internal ushort _key;
            internal System.Single _score;
        }

        [SerializeField] internal System.Boolean invert;
        [SerializeField] internal BlackboardTemplate.KeySelector key;
        [SerializeField] internal System.Single score;

        // pack memory
        private Memory memory => new Memory
        { _invert = invert, _key = key.selectedKey.offset, _score = score };

        #region Execution

        // actual function
        [Unity.Burst.BurstCompile(DisableDirectCall = true), AOT.MonoPInvokeCallback(typeof(Delegate))]
        private static float ActualFunction(in BlackboardComponent.LowLevel blackboard, byte* rawMemory, float currentScore)
        {
            var memory = (Memory*) rawMemory;
            return HiraBots.SampleScoreCalculatorBlackboardFunctions.BooleanIsSetScoreCalculator(currentScore, memory->_invert, ref blackboard.Access<bool>(memory->_key), memory->_score);
        }

        // non-VM execution
        protected override float ExecuteFunction(BlackboardComponent blackboard, bool expected, float currentScore)
        {
            var _key = blackboard.GetBooleanValue(key.selectedKey.name); var output = HiraBots.SampleScoreCalculatorBlackboardFunctions.BooleanIsSetScoreCalculator(currentScore, invert, ref _key, score); return output;
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

        protected override System.IntPtr functionPtr => s_ActualFunction.Value;

        #endregion

#if UNITY_EDITOR
        #region EditorOnlyInterface

        protected override void OnTargetBlackboardTemplateChanged(BlackboardTemplate template, in BlackboardTemplate.KeySet keySet)
        {
            key.OnTargetBlackboardTemplateChanged(template, in keySet);
        }

        protected override void OnValidateCallback()
        {
            key.keyTypesFilter = UnityEngine.BlackboardKeyType.Invalid | UnityEngine.BlackboardKeyType.Boolean;
            // no external validator
        }

        protected override void UpdateDescription(out string staticDescription)
        {
            HiraBots.SampleScoreCalculatorBlackboardFunctions.BooleanIsSetScoreCalculatorUpdateDescription(invert, key, score, out staticDescription);
        }

        #endregion
#endif

#if UNITY_EDITOR || HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
        #region Validation

        protected override void Validate(ref ValidatorContext context)
        {
            base.Validate(ref context);
            ValidateKeySelector(ref key, UnityEngine.BlackboardKeyType.Invalid | UnityEngine.BlackboardKeyType.Boolean, ref context, nameof(key));
        }

        #endregion
#endif
    }
}