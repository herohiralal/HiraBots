namespace HiraBots
{
    internal partial class BlackboardKey
    {
#if UNITY_EDITOR
        internal string unmanagedFieldGeneratedCode =>
            CodeGenHelpers.ReadTemplate("Blackboard/BlackboardKeyField",
                ("<BLACKBOARD-KEY-TYPE>", unmanagedTypeName),
                ("<BLACKBOARD-KEY-NAME>", name));

        internal string defaultInitializerGeneratedCode =>
            CodeGenHelpers.ReadTemplate("Blackboard/BlackboardDefaultInitializer",
                ("<BLACKBOARD-KEY-NAME>", name),
                ("<BLACKBOARD-DEFAULT-VALUE>", defaultValueGeneratedCode));

        protected abstract string unmanagedTypeName { get; }
        protected abstract string defaultValueGeneratedCode { get; }
        internal abstract string actualTypeName { get; }
#endif
    }

#if UNITY_EDITOR

    internal partial class BooleanBlackboardKey
    {
        protected override string unmanagedTypeName => "bool";
        protected override string defaultValueGeneratedCode => m_DefaultValue ? "true" : "false";
        internal override string actualTypeName => "bool";
    }

    internal partial class EnumBlackboardKey
    {
        protected override string unmanagedTypeName => "byte";
        protected override string defaultValueGeneratedCode => m_DefaultValue.m_Value.ToString();
        internal override string actualTypeName => DynamicEnum.Helpers.identifierToType[m_DefaultValue.m_TypeIdentifier].FullName;
    }

    internal partial class FloatBlackboardKey
    {
        protected override string unmanagedTypeName => "float";
        protected override string defaultValueGeneratedCode => $"{m_DefaultValue}f";
        internal override string actualTypeName => "float";
    }

    internal partial class IntegerBlackboardKey
    {
        protected override string unmanagedTypeName => "int";
        protected override string defaultValueGeneratedCode => m_DefaultValue.ToString();
        internal override string actualTypeName => "int";
    }

    internal partial class ObjectBlackboardKey
    {
        protected override string unmanagedTypeName => "int";
        protected override string defaultValueGeneratedCode => "0";
        internal override string actualTypeName => "Object";
    }

    internal partial class QuaternionBlackboardKey
    {
        protected override string unmanagedTypeName => "quaternion";

        protected override string defaultValueGeneratedCode =>
            $"quaternion.Euler(new float3({m_DefaultValue.x}f, {m_DefaultValue.y}f, {m_DefaultValue.z}f))";

        internal override string actualTypeName => "Quaternion";
    }

    internal partial class VectorBlackboardKey
    {
        protected override string unmanagedTypeName => "float3";

        protected override string defaultValueGeneratedCode =>
            $"new float3({m_DefaultValue.x}f, {m_DefaultValue.y}f, {m_DefaultValue.z}f)";

        internal override string actualTypeName => "Vector3";
    }

#endif
}