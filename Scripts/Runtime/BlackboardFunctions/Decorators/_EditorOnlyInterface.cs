#if UNITY_EDITOR
using UnityEngine;

namespace HiraBots
{
    internal partial class AlwaysSucceedDecoratorBlackboardFunction
    {
    }

    internal partial class EnumHasFlagsDecoratorBlackboardFunction
    {
        protected override void OnValidate()
        {
            base.OnValidate();

            m_Key.keyTypesFilter = BlackboardKeyType.Enum;

            m_Value.m_TypeIdentifier = m_Key.selectedKey is EnumBlackboardKey enumKey
                ? enumKey.typeIdentifier
                : "";
        }

        internal override void OnTargetBlackboardTemplateChanged(BlackboardTemplate newTemplate, ReadOnlyHashSetAccessor<BlackboardKey> keySet)
        {
            base.OnTargetBlackboardTemplateChanged(newTemplate, keySet);

            m_Key.OnTargetBlackboardTemplateChanged(newTemplate, keySet);
        }
    }

    internal partial class NumericalComparisonDecoratorBlackboardFunction
    {
        protected override void OnValidate()
        {
            base.OnValidate();

            m_Key.keyTypesFilter = BlackboardKeyType.Numeric;
        }

        internal override void OnTargetBlackboardTemplateChanged(BlackboardTemplate newTemplate, ReadOnlyHashSetAccessor<BlackboardKey> keySet)
        {
            base.OnTargetBlackboardTemplateChanged(newTemplate, keySet);

            m_Key.OnTargetBlackboardTemplateChanged(newTemplate, keySet);
        }
    }

    internal partial class IsSetDecoratorBlackboardFunction
    {
        protected override void OnValidate()
        {
            base.OnValidate();

            m_Key.keyTypesFilter = BlackboardKeyType.UnmanagedSettable;
        }

        internal override void OnTargetBlackboardTemplateChanged(BlackboardTemplate newTemplate, ReadOnlyHashSetAccessor<BlackboardKey> keySet)
        {
            base.OnTargetBlackboardTemplateChanged(newTemplate, keySet);

            m_Key.OnTargetBlackboardTemplateChanged(newTemplate, keySet);
        }
    }

    internal partial class ObjectEqualsDecoratorBlackboardFunction
    {
        protected override void OnValidate()
        {
            base.OnValidate();

            m_Key.keyTypesFilter = BlackboardKeyType.Object;
        }

        internal override void OnTargetBlackboardTemplateChanged(BlackboardTemplate newTemplate, ReadOnlyHashSetAccessor<BlackboardKey> keySet)
        {
            base.OnTargetBlackboardTemplateChanged(newTemplate, keySet);

            m_Key.OnTargetBlackboardTemplateChanged(newTemplate, keySet);
        }
    }
}
#endif