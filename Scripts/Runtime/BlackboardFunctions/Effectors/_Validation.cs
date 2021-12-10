#if UNITY_EDITOR || HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
namespace HiraBots
{
    internal partial class EnumOperatorEffectorBlackboardFunction
    {
        internal override void Validate(ref BlackboardFunctionValidatorContext context)
        {
            base.Validate(ref context);

            if (!ValidateKeySelector(ref m_Key, BlackboardKeyType.Enum, ref context))
            {
                context.badObjects.Add($"{context.identifier}({name})::{nameof(m_Key)}({(m_Key.selectedKey == null ? "null" : m_Key.selectedKey.name)}) (badly selected)");
            }
        }
    }

    internal partial class FloatOperatorEffectorBlackboardFunction
    {
        internal override void Validate(ref BlackboardFunctionValidatorContext context)
        {
            base.Validate(ref context);

            if (!ValidateKeySelector(ref m_Key, BlackboardKeyType.Float, ref context))
            {
                context.badObjects.Add($"{context.identifier}({name})::{nameof(m_Key)}({(m_Key.selectedKey == null ? "null" : m_Key.selectedKey.name)}) (badly selected)");
            }
        }
    }

    internal partial class IntegerOperatorEffectorBlackboardFunction
    {
        internal override void Validate(ref BlackboardFunctionValidatorContext context)
        {
            base.Validate(ref context);

            if (!ValidateKeySelector(ref m_Key, BlackboardKeyType.Integer, ref context))
            {
                context.badObjects.Add($"{context.identifier}{name}:{nameof(m_Key)}({(m_Key.selectedKey == null ? "null" : m_Key.selectedKey.name)}) (badly selected)");
            }
        }
    }

    internal partial class IsSetEffectorBlackboardFunction
    {
        internal override void Validate(ref BlackboardFunctionValidatorContext context)
        {
            base.Validate(ref context);

            if (!ValidateKeySelector(ref m_Key, BlackboardKeyType.UnmanagedSettable, ref context))
            {
                context.badObjects.Add($"{context.identifier}({name})::{nameof(m_Key)}({(m_Key.selectedKey == null ? "null" : m_Key.selectedKey.name)}) (badly selected)");
            }
        }
    }

    internal partial class ObjectEqualsEffectorBlackboardFunction
    {
        internal override void Validate(ref BlackboardFunctionValidatorContext context)
        {
            base.Validate(ref context);

            if (!ValidateKeySelector(ref m_Key, BlackboardKeyType.Object, ref context))
            {
                context.badObjects.Add($"{context.identifier}({name})::{nameof(m_Key)}({(m_Key.selectedKey == null ? "null" : m_Key.selectedKey.name)}) (badly selected)");
            }
        }
    }
}
#endif