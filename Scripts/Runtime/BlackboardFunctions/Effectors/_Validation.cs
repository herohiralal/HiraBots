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
                var badKeyInfo = new BlackboardFunctionValidatorContext.BadKeyInfo
                {
                    functionName = name,
                    variableName = nameof(m_Key),
                    selectedKey = m_Key.selectedKey
                };

                context.succeeded = false;
                context.badlySelectedKeys.Add(badKeyInfo);
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
                var badKeyInfo = new BlackboardFunctionValidatorContext.BadKeyInfo
                {
                    functionName = name,
                    variableName = nameof(m_Key),
                    selectedKey = m_Key.selectedKey
                };

                context.succeeded = false;
                context.badlySelectedKeys.Add(badKeyInfo);
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
                var badKeyInfo = new BlackboardFunctionValidatorContext.BadKeyInfo
                {
                    functionName = name,
                    variableName = nameof(m_Key),
                    selectedKey = m_Key.selectedKey
                };

                context.succeeded = false;
                context.badlySelectedKeys.Add(badKeyInfo);
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
                var badKeyInfo = new BlackboardFunctionValidatorContext.BadKeyInfo
                {
                    functionName = name,
                    variableName = nameof(m_Key),
                    selectedKey = m_Key.selectedKey
                };

                context.succeeded = false;
                context.badlySelectedKeys.Add(badKeyInfo);
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
                var badKeyInfo = new BlackboardFunctionValidatorContext.BadKeyInfo
                {
                    functionName = name,
                    variableName = nameof(m_Key),
                    selectedKey = m_Key.selectedKey
                };

                context.succeeded = false;
                context.badlySelectedKeys.Add(badKeyInfo);
            }
        }
    }
}
#endif