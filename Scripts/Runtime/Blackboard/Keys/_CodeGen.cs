namespace HiraBots
{
    internal partial class BlackboardKey
    {
#if UNITY_EDITOR
        /// <summary>
        /// Generate code for all the unmanaged fields.
        /// </summary>
        internal string unmanagedFieldGeneratedCode =>
            CodeGenHelpers.ReadTemplate("Blackboard/BlackboardKeyField",
                ("<BLACKBOARD-KEY-TYPE>", GetUnmanagedTypeName(m_KeyType)),
                ("<BLACKBOARD-KEY-NAME>", name));

        /// <summary>
        /// Generate code for all the direct accessors.
        /// </summary>
        internal string accessorGeneratedCode
        {
            get
            {
                var file = m_EssentialToDecisionMaking
                    ? m_InstanceSynced
                        ? "Blackboard/BlackboardStaticEssentialKeyAccessor"
                        : "Blackboard/BlackboardEssentialKeyAccessor"
                    : m_InstanceSynced
                        ? "Blackboard/BlackboardStaticKeyAccessor"
                        : "Blackboard/BlackboardKeyAccessor";

                return CodeGenHelpers.ReadTemplate(file,
                    ("<BLACKBOARD-KEY-NAME>", name),
                    ("<BLACKBOARD-KEY-TYPE>", GetUnmanagedTypeName(m_KeyType)),
                    ("<BLACKBOARD-ACTUAL-KEY-TYPE>", actualTypeNameGeneratedCode),
                    ("<BLACKBOARD-KEY-ACTUAL-TO-UNMANAGED>", actualToUnmanagedGeneratedCode),
                    ("<BLACKBOARD-KEY-UNMANAGED-TO-ACTUAL>", unmanagedToActualGeneratedCode),
                    ("<BLACKBOARD-KEY-EQUALITY-COMPARER>", GetEqualityComparerGeneratedCode(m_KeyType)));
            }
        }

        /// <summary>
        /// Generate code for cleaning up static variables on RuntimeInitializeOnLoad
        /// </summary>
        internal string staticCleanerGeneratedCode =>
            m_InstanceSynced
                ? CodeGenHelpers.ReadTemplate("Blackboard/BlackboardStaticCleaner",
                    ("<BLACKBOARD-KEY-NAME>", name),
                    ("<BLACKBOARD-DEFAULT-VALUE>", defaultValueGeneratedCode))
                : "";

        /// <summary>
        /// Generate code for initializers.
        /// </summary>
        internal string initializerGeneratedCode =>
            CodeGenHelpers.ReadTemplate(m_InstanceSynced ? "Blackboard/BlackboardStaticInitializer" : "Blackboard/BlackboardInitializer",
                ("<BLACKBOARD-KEY-NAME>", name));

        /// <summary>
        /// Generate code for default initializers.
        /// </summary>
        internal string defaultInitializerGeneratedCode =>
            m_InstanceSynced
                ? ""
                : CodeGenHelpers.ReadTemplate("Blackboard/BlackboardDefaultValueInitializer",
                    ("<BLACKBOARD-KEY-NAME>", name),
                    ("<BLACKBOARD-DEFAULT-VALUE>", defaultValueGeneratedCode));

        /// <summary>
        /// Generate code for string accessors.
        /// </summary>
        /// <param name="type">The key type.</param>
        /// <param name="isStatic">Whether the accessor is static.</param>
        /// <param name="accessType">Whether it's a setter or a getter.</param>
        internal string GetStringAccessorGeneratedCode(BlackboardKeyType type, bool isStatic, string accessType)
        {
            string file;

            if (isStatic && !m_InstanceSynced)
            {
                file = "Blackboard/BlackboardStaticIndividualFailedAccessor";
            }
            else if (type == m_KeyType)
            {
                file = m_KeyType == BlackboardKeyType.Enum
                    ? $"Blackboard/BlackboardIndividualEnum{accessType}"
                    : $"Blackboard/BlackboardIndividual{accessType}";
            }
            else
            {
                file = "Blackboard/BlackboardIndividualFailedAccessor";
            }

            return CodeGenHelpers.ReadTemplate(file,
                ("<BLACKBOARD-KEY-NAME>", name),
                ("<BLACKBOARD-STATIC-ACCESSOR>", isStatic ? "Static" : ""),
                ("<BLACKBOARD-KEY-ACTUAL-TO-UNMANAGED>", actualToUnmanagedGeneratedCode),
                ("<BLACKBOARD-KEY-UNMANAGED-TO-ACTUAL>", unmanagedToActualGeneratedCode));
        }

        /// <summary>
        /// Get the unmanaged type name for a blackboard key type..
        /// </summary>
        internal static string GetUnmanagedTypeName(BlackboardKeyType type)
        {
            switch (type)
            {
                case BlackboardKeyType.Boolean:
                    return "bool";
                case BlackboardKeyType.Enum:
                    return "byte";
                case BlackboardKeyType.Float:
                    return "float";
                case BlackboardKeyType.Integer:
                    return "int";
                case BlackboardKeyType.Object:
                    return "int";
                case BlackboardKeyType.Quaternion:
                    return "quaternion";
                case BlackboardKeyType.Vector:
                    return "float3";
                default:
                    return "INTENTIONALLY PUT ERROR HERE BECAUSE UNSUPPORTED.";
            }
        }

        /// <summary>
        /// Generate code for setting the default value.
        /// </summary>
        protected abstract string defaultValueGeneratedCode { get; }

        /// <summary>
        /// Get the actual type name that the user wants the variable as.
        /// </summary>
        internal static string GetActualTypeName(BlackboardKeyType type)
        {
            switch (type)
            {
                case BlackboardKeyType.Boolean:
                    return "bool";
                case BlackboardKeyType.Float:
                    return "float";
                case BlackboardKeyType.Integer:
                    return "int";
                case BlackboardKeyType.Object:
                    return "Object";
                case BlackboardKeyType.Quaternion:
                    return "Quaternion";
                case BlackboardKeyType.Vector:
                    return "Vector3";
                default:
                    return "INTENTIONALLY PUT ERROR HERE BECAUSE UNSUPPORTED.";
            }
        }

        /// <summary>
        /// Accessor to override default function.
        /// </summary>
        protected virtual string actualTypeNameGeneratedCode => GetActualTypeName(m_KeyType);

        /// <summary>
        /// Get the converter from unmanaged to the actual type.
        /// </summary>
        private static string GetUnmanagedToActualGeneratedCode(BlackboardKeyType type)
        {
            switch (type)
            {
                case BlackboardKeyType.Object:
                    return "GeneratedBlackboardHelpers.InstanceIDToObject";
                case BlackboardKeyType.Quaternion:
                    return "(Quaternion) ";
                case BlackboardKeyType.Vector:
                    return "(Vector3) ";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Accessor to override the default function.
        /// </summary>
        protected virtual string unmanagedToActualGeneratedCode => GetUnmanagedToActualGeneratedCode(m_KeyType);

        /// <summary>
        /// Get the converter from actual to unmanaged type.
        /// </summary>
        private static string GetActualToUnmanagedGeneratedCode(BlackboardKeyType type)
        {
            switch (type)
            {
                case BlackboardKeyType.Object:
                    return "GeneratedBlackboardHelpers.ObjectToInstanceID";
                case BlackboardKeyType.Quaternion:
                    return "(quaternion) ";
                case BlackboardKeyType.Vector:
                    return "(float3) ";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Accessor to override the default function.
        /// </summary>
        protected virtual string actualToUnmanagedGeneratedCode => GetActualToUnmanagedGeneratedCode(m_KeyType);

        /// <summary>
        /// Get the equality comparer.
        /// </summary>
        private static string GetEqualityComparerGeneratedCode(BlackboardKeyType type)
        {
            switch (type)
            {
                case BlackboardKeyType.Quaternion:
                case BlackboardKeyType.Vector:
                    return ".Equals";
                default:
                    return " == ";
            }
        }
#endif
    }

#if UNITY_EDITOR

    internal partial class BooleanBlackboardKey
    {
        protected override string defaultValueGeneratedCode => m_DefaultValue ? "true" : "false";
    }

    internal partial class EnumBlackboardKey
    {
        protected override string defaultValueGeneratedCode => m_DefaultValue.m_Value.ToString();
        protected override string actualTypeNameGeneratedCode => DynamicEnum.Helpers.identifierToType[m_DefaultValue.m_TypeIdentifier].FullName;
        protected override string unmanagedToActualGeneratedCode => $"({actualTypeNameGeneratedCode}) ";
        protected override string actualToUnmanagedGeneratedCode => "(byte) ";
    }

    internal partial class FloatBlackboardKey
    {
        protected override string defaultValueGeneratedCode => $"{m_DefaultValue}f";
    }

    internal partial class IntegerBlackboardKey
    {
        protected override string defaultValueGeneratedCode => m_DefaultValue.ToString();
    }

    internal partial class ObjectBlackboardKey
    {
        protected override string defaultValueGeneratedCode => "0";
    }

    internal partial class QuaternionBlackboardKey
    {
        protected override string defaultValueGeneratedCode =>
            $"quaternion.Euler(new float3({m_DefaultValue.x}f, {m_DefaultValue.y}f, {m_DefaultValue.z}f))";
    }

    internal partial class VectorBlackboardKey
    {
        protected override string defaultValueGeneratedCode =>
            $"new float3({m_DefaultValue.x}f, {m_DefaultValue.y}f, {m_DefaultValue.z}f)";
    }

#endif
}