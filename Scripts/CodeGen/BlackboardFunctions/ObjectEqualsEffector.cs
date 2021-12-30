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
    public unsafe partial class ObjectEqualsEffector : HiraBotsEffectorBlackboardFunction
    {
        private struct Memory
        {
            internal BlackboardKey.LowLevel _key;
            internal int _value;
        }

        [SerializeField] internal BlackboardTemplate.KeySelector key;
        [SerializeField] internal UnityEngine.Object value;

        // pack memory
        private Memory memory => new Memory
        { _key = new BlackboardKey.LowLevel(key.selectedKey), _value = GeneratedBlackboardHelpers.ObjectToInstanceID(value) };

        #region Execution

        // actual function
        [Unity.Burst.BurstCompile(DisableDirectCall = true), AOT.MonoPInvokeCallback(typeof(Delegate))]
        private static void ActualFunction(in BlackboardComponent.LowLevel blackboard, byte* rawMemory)
        {
            var memory = (Memory*) rawMemory;
            HiraBots.SampleEffectorBlackboardFunctions.ObjectEqualsEffector(ref blackboard.Access<int>(memory->_key.offset), memory->_value);
        }

        // non-VM execution
        protected override void ExecuteFunction(BlackboardComponent blackboard, bool expected)
        {
            var _key = blackboard.GetObjectValue(key.selectedKey.name).GetInstanceID(); HiraBots.SampleEffectorBlackboardFunctions.ObjectEqualsEffector(ref _key, value.GetInstanceID()); blackboard.SetObjectValue(key.selectedKey.name, GeneratedBlackboardHelpers.InstanceIDToObject(_key), expected);
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
            key.keyTypesFilter = UnityEngine.BlackboardKeyType.Invalid | UnityEngine.BlackboardKeyType.Object;
        }

        #endregion
#endif

#if UNITY_EDITOR || HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
        #region Validation

        protected override void Validate(ref ValidatorContext context)
        {
            base.Validate(ref context);
            ValidateKeySelector(ref key, UnityEngine.BlackboardKeyType.Invalid | UnityEngine.BlackboardKeyType.Object, ref context, nameof(key));
        }

        #endregion
#endif
    }
}