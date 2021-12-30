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
    public unsafe partial class FloatOperatorEffector : HiraBotsEffectorBlackboardFunction
    {
        private struct Memory
        {
            internal ushort _key;
            internal HiraBots.FloatOperationType _operationType;
            internal System.Single _value;
        }

        [SerializeField] internal BlackboardTemplate.KeySelector key;
        [SerializeField] internal HiraBots.FloatOperationType operationType;
        [SerializeField] internal System.Single value;

        // pack memory
        private Memory memory => new Memory
        { _key = key.selectedKey.offset, _operationType = operationType, _value = value };

        #region Execution

        // actual function
        [Unity.Burst.BurstCompile(DisableDirectCall = true), AOT.MonoPInvokeCallback(typeof(Delegate))]
        private static void ActualFunction(in BlackboardComponent.LowLevel blackboard, byte* rawMemory)
        {
            var memory = (Memory*) rawMemory;
            HiraBots.SampleEffectorBlackboardFunctions.FloatOperatorEffector(ref blackboard.Access<float>(memory->_key), memory->_operationType, memory->_value);
        }

        // non-VM execution
        protected override void ExecuteFunction(BlackboardComponent blackboard, bool expected)
        {
            var _key = blackboard.GetFloatValue(key.selectedKey.name); HiraBots.SampleEffectorBlackboardFunctions.FloatOperatorEffector(ref _key, operationType, value); blackboard.SetFloatValue(key.selectedKey.name, _key, expected);
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
            key.keyTypesFilter = UnityEngine.BlackboardKeyType.Invalid | UnityEngine.BlackboardKeyType.Float;
            HiraBots.SampleEffectorBlackboardFunctions.FloatOperatorEffectorOnValidate(ref key, ref operationType, ref value);
        }

        protected override void UpdateDescription(out string staticDescription)
        {
            HiraBots.SampleEffectorBlackboardFunctions.FloatOperatorEffectorUpdateDescription(key, operationType, value, out staticDescription);
        }

        #endregion
#endif

#if UNITY_EDITOR || HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
        #region Validation

        public override void Validate(ref ValidatorContext context)
        {
            base.Validate(ref context);
            ValidateKeySelector(ref key, UnityEngine.BlackboardKeyType.Invalid | UnityEngine.BlackboardKeyType.Float, ref context, nameof(key));
        }

        #endregion
#endif
    }
}