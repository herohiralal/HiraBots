namespace HiraBots
{
    internal partial class BlackboardKey
    {
#if UNITY_EDITOR
        internal string unmanagedFieldGeneratedCode =>
            CodeGenHelpers.ReadTemplate("Blackboard/BlackboardKeyField",
                ("<BLACKBOARD-KEY-TYPE>", unmanagedTypeName),
                ("<BLACKBOARD-KEY-NAME>", name));

        internal string accessorGeneratedCode =>
            CodeGenHelpers.ReadTemplate("Blackboard/BlackboardKeyAccessor",
                ("<BLACKBOARD-KEY-NAME>", name),
                ("<BLACKBOARD-KEY-TYPE>", unmanagedTypeName),
                ("<BLACKBOARD-DEFAULT-VALUE>", defaultValueGeneratedCode),
                ("<BLACKBOARD-ACTUAL-KEY-TYPE>", actualTypeName),
                ("<BLACKBOARD-KEY-UNMANAGED-TO-ACTUAL>", unmanagedToActualGeneratedCode),
                ("<BLACKBOARD-KEY-ACTUAL-TO-UNMANAGED>", actualToUnmanagedGeneratedCode));

        internal string initializerGeneratedCode =>
            CodeGenHelpers.ReadTemplate("Blackboard/BlackboardInitializer",
                ("<BLACKBOARD-KEY-NAME>", name));

        protected abstract string unmanagedTypeName { get; }
        protected abstract string defaultValueGeneratedCode { get; }
        protected abstract string actualTypeName { get; }
        protected virtual string unmanagedToActualGeneratedCode => "";
        protected virtual string actualToUnmanagedGeneratedCode => "";
#endif
    }

#if UNITY_EDITOR

    internal partial class BooleanBlackboardKey
    {
        protected override string unmanagedTypeName => "bool";
        protected override string defaultValueGeneratedCode => m_DefaultValue ? "true" : "false";
        protected override string actualTypeName => "bool";
    }

    internal partial class EnumBlackboardKey
    {
        protected override string unmanagedTypeName => "byte";
        protected override string defaultValueGeneratedCode => m_DefaultValue.m_Value.ToString();
        protected override string actualTypeName => DynamicEnum.Helpers.identifierToType[m_DefaultValue.m_TypeIdentifier].FullName;
        protected override string unmanagedToActualGeneratedCode => $"({actualTypeName}) ";
        protected override string actualToUnmanagedGeneratedCode => "(byte) ";
    }

    internal partial class FloatBlackboardKey
    {
        protected override string unmanagedTypeName => "float";
        protected override string defaultValueGeneratedCode => $"{m_DefaultValue}f";
        protected override string actualTypeName => "float";
    }

    internal partial class IntegerBlackboardKey
    {
        protected override string unmanagedTypeName => "int";
        protected override string defaultValueGeneratedCode => m_DefaultValue.ToString();
        protected override string actualTypeName => "int";
    }

    internal partial class ObjectBlackboardKey
    {
        protected override string unmanagedTypeName => "int";
        protected override string defaultValueGeneratedCode => "0";
        protected override string actualTypeName => "Object";
        protected override string unmanagedToActualGeneratedCode => "GeneratedBlackboardHelpers.InstanceIDToObject";
        protected override string actualToUnmanagedGeneratedCode => "GeneratedBlackboardHelpers.ObjectToInstanceID";
    }

    internal partial class QuaternionBlackboardKey
    {
        protected override string unmanagedTypeName => "quaternion";

        protected override string defaultValueGeneratedCode =>
            $"quaternion.Euler(new float3({m_DefaultValue.x}f, {m_DefaultValue.y}f, {m_DefaultValue.z}f))";

        protected override string actualTypeName => "Quaternion";
    }

    internal partial class VectorBlackboardKey
    {
        protected override string unmanagedTypeName => "float3";

        protected override string defaultValueGeneratedCode =>
            $"new float3({m_DefaultValue.x}f, {m_DefaultValue.y}f, {m_DefaultValue.z}f)";

        protected override string actualTypeName => "Vector3";
    }

#endif
}