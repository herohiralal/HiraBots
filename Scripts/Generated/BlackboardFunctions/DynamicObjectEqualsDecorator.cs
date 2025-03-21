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
    public unsafe partial class DynamicObjectEqualsDecorator : HiraBotsDecoratorBlackboardFunction
    {
        private struct Memory
        {
            internal System.Boolean _invert;
            internal ushort _a;
            internal ushort _b;
        }

        [SerializeField] internal System.Boolean invert;
        [SerializeField] internal BlackboardTemplate.KeySelector a;
        [SerializeField] internal BlackboardTemplate.KeySelector b;

        // pack memory
        private Memory memory => new Memory
        { _invert = invert, _a = a.selectedKey.offset, _b = b.selectedKey.offset };

        #region Execution

        // actual function
        [Unity.Burst.BurstCompile(DisableDirectCall = true), AOT.MonoPInvokeCallback(typeof(Delegate))]
        private static bool ActualFunction(in BlackboardComponent.LowLevel blackboard, byte* rawMemory)
        {
            var memory = (Memory*) rawMemory;
            return HiraBots.SampleDecoratorBlackboardFunctions.DynamicObjectEqualsDecorator(memory->_invert, ref blackboard.Access<int>(memory->_a), ref blackboard.Access<int>(memory->_b));
        }

        // non-VM execution
        protected override bool ExecuteFunction(BlackboardComponent blackboard, bool expected)
        {
            var _a = GeneratedBlackboardHelpers.ObjectToInstanceID(blackboard.GetObjectValue(a.selectedKey.name)); var _b = GeneratedBlackboardHelpers.ObjectToInstanceID(blackboard.GetObjectValue(b.selectedKey.name)); var output = HiraBots.SampleDecoratorBlackboardFunctions.DynamicObjectEqualsDecorator(invert, ref _a, ref _b); return output;
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
            a.OnTargetBlackboardTemplateChanged(template, in keySet); 
            b.OnTargetBlackboardTemplateChanged(template, in keySet);
        }

        protected override void OnValidateCallback()
        {
            a.keyTypesFilter = UnityEngine.AI.BlackboardKeyType.Invalid | UnityEngine.AI.BlackboardKeyType.Object;
            b.keyTypesFilter = UnityEngine.AI.BlackboardKeyType.Invalid | UnityEngine.AI.BlackboardKeyType.Object;
            // no external validator
        }

        protected override void UpdateDescription(out string staticDescription)
        {
            HiraBots.SampleDecoratorBlackboardFunctions.DynamicObjectEqualsDecoratorUpdateDescription(invert, a, b, out staticDescription);
        }

        #endregion
#endif

#if UNITY_EDITOR || HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
        #region Validation

        public override void Validate(ref ValidatorContext context)
        {
            base.Validate(ref context);
            ValidateKeySelector(ref a, UnityEngine.AI.BlackboardKeyType.Invalid | UnityEngine.AI.BlackboardKeyType.Object, ref context, nameof(a));
            ValidateKeySelector(ref b, UnityEngine.AI.BlackboardKeyType.Invalid | UnityEngine.AI.BlackboardKeyType.Object, ref context, nameof(b));
        }

        #endregion
#endif
    }
}