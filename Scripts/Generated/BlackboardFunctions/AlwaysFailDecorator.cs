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
    public unsafe partial class AlwaysFailDecorator : HiraBotsDecoratorBlackboardFunction
    {
        private struct Memory
        {
        }


        // pack memory
        private Memory memory => new Memory
        {  };

        #region Execution

        // actual function
        [Unity.Burst.BurstCompile(DisableDirectCall = true), AOT.MonoPInvokeCallback(typeof(Delegate))]
        private static bool ActualFunction(in BlackboardComponent.LowLevel blackboard, byte* rawMemory)
        {
            var memory = (Memory*) rawMemory;
            return HiraBots.SampleDecoratorBlackboardFunctions.AlwaysFailDecorator();
        }

        // non-VM execution
        protected override bool ExecuteFunction(BlackboardComponent blackboard, bool expected)
        {
            var output = HiraBots.SampleDecoratorBlackboardFunctions.AlwaysFailDecorator(); return output;
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
        }

        protected override void OnValidateCallback()
        {
            // no external validator
        }

        protected override void UpdateDescription(out string staticDescription)
        {
            HiraBots.SampleDecoratorBlackboardFunctions.AlwaysFailDecoratorUpdateDescription(out staticDescription);
        }

        #endregion
#endif

#if UNITY_EDITOR || HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
        #region Validation

        public override void Validate(ref ValidatorContext context)
        {
            base.Validate(ref context);
        }

        #endregion
#endif
    }
}