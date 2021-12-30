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
    public unsafe partial class EnumHasFlagsDecorator : HiraBotsDecoratorBlackboardFunction
    {
        private struct Memory
        {
            internal System.Boolean _invert;
            internal BlackboardKey.LowLevel _key;
            internal byte _value;
        }

        [SerializeField] internal System.Boolean invert;
        [SerializeField] internal BlackboardTemplate.KeySelector key;
        [SerializeField] internal DynamicEnum value;

        // pack memory
        private Memory memory => new Memory
        { _invert = invert, _key = new BlackboardKey.LowLevel(key.selectedKey), _value = value };

        #region Execution

        // actual function
        [Unity.Burst.BurstCompile(DisableDirectCall = true), AOT.MonoPInvokeCallback(typeof(Delegate))]
        private static bool ActualFunction(in BlackboardComponent.LowLevel blackboard, byte* rawMemory)
        {
            var memory = (Memory*) rawMemory;
            return HiraBots.SampleDecoratorBlackboardFunctions.EnumHasFlagsDecorator(memory->_invert, ref blackboard.Access<byte>(memory->_key.offset), memory->_value);
        }

        // non-VM execution
        protected override bool ExecuteFunction(BlackboardComponent blackboard, bool expected)
        {
            var _key = blackboard.GetEnumValue(key.selectedKey.name); var output = HiraBots.SampleDecoratorBlackboardFunctions.EnumHasFlagsDecorator(invert, ref _key, value); return output;
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
            key.keyTypesFilter = UnityEngine.BlackboardKeyType.Invalid | UnityEngine.BlackboardKeyType.Enum;
            value.typeIdentifier = key.selectedKey.enumTypeIdentifier;
            // no external validator
        }

        protected override void UpdateDescription(out string staticDescription)
        {
            HiraBots.SampleDecoratorBlackboardFunctions.EnumHasFlagsDecoratorUpdateDescription(invert, key, value, out staticDescription);
        }

        #endregion
#endif

#if UNITY_EDITOR || HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
        #region Validation

        protected override void Validate(ref ValidatorContext context)
        {
            base.Validate(ref context);
            ValidateKeySelector(ref key, UnityEngine.BlackboardKeyType.Invalid | UnityEngine.BlackboardKeyType.Enum, ref context, nameof(key));
        }

        #endregion
#endif
    }
}