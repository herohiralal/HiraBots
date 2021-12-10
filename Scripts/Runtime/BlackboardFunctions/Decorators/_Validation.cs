#if UNITY_EDITOR || HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
namespace HiraBots
{
    internal partial class EnumHasFlagsDecoratorBlackboardFunction
    {
        internal override void Validate(ref BlackboardFunctionValidatorContext context)
        {
            base.Validate(ref context);

            if (!ValidateKeySelector(ref m_Key, BlackboardKeyType.Enum, ref context))
            {
                context.badKeys.Add($"{context.identifier}({name})::{nameof(m_Key)}({(m_Key.selectedKey == null ? "null" : m_Key.selectedKey.name)})");
            }
        }
    }

    internal partial class IsSetDecoratorBlackboardFunction
    {
        internal override void Validate(ref BlackboardFunctionValidatorContext context)
        {
            base.Validate(ref context);

            if (!ValidateKeySelector(ref m_Key, BlackboardKeyType.UnmanagedSettable, ref context))
            {
                context.badKeys.Add($"{context.identifier}({name})::{nameof(m_Key)}({(m_Key.selectedKey == null ? "null" : m_Key.selectedKey.name)})");
            }
        }
    }

    internal partial class NumericalComparisonDecoratorBlackboardFunction
    {
        internal override void Validate(ref BlackboardFunctionValidatorContext context)
        {
            base.Validate(ref context);

            if (!ValidateKeySelector(ref m_Key, BlackboardKeyType.Numeric, ref context))
            {
                context.badKeys.Add($"{context.identifier}({name})::{nameof(m_Key)}({(m_Key.selectedKey == null ? "null" : m_Key.selectedKey.name)})");
            }
        }
    }

    internal partial class ObjectEqualsDecoratorBlackboardFunction
    {
        internal override void Validate(ref BlackboardFunctionValidatorContext context)
        {
            base.Validate(ref context);

            if (!ValidateKeySelector(ref m_Key, BlackboardKeyType.Object, ref context))
            {
                context.badKeys.Add($"{context.identifier}({name})::{nameof(m_Key)}({(m_Key.selectedKey == null ? "null" : m_Key.selectedKey.name)})");
            }
        }
    }
}
#endif