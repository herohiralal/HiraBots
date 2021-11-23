#if UNITY_EDITOR
using UnityEngine;

namespace HiraBots
{
    internal partial class FloatOperatorEffectorBlackboardFunction
    {
        protected override void OnValidate()
        {
            base.OnValidate();

            if (m_OperationType == OperationType.Divide && m_Value == 0f)
            {
                m_Value = 0.1f;
            }

            m_Key.keyTypesFilter = BlackboardKeyType.Float;
        }

        internal override void OnTargetBlackboardTemplateChanged(BlackboardTemplate newTemplate, ReadOnlyHashSetAccessor<BlackboardKey> keySet)
        {
            base.OnTargetBlackboardTemplateChanged(newTemplate, keySet);

            m_Key.OnTargetBlackboardTemplateChanged(newTemplate, keySet);
        }
    }

    internal partial class IntegerOperatorEffectorBlackboardFunction
    {
        protected override void OnValidate()
        {
            base.OnValidate();

            if (m_OperationType == OperationType.Divide && m_Value == 0)
            {
                m_Value = 1;
            }

            m_Key.keyTypesFilter = BlackboardKeyType.Integer;
        }

        internal override void OnTargetBlackboardTemplateChanged(BlackboardTemplate newTemplate, ReadOnlyHashSetAccessor<BlackboardKey> keySet)
        {
            base.OnTargetBlackboardTemplateChanged(newTemplate, keySet);

            m_Key.OnTargetBlackboardTemplateChanged(newTemplate, keySet);
        }
    }

    internal partial class EnumOperatorEffectorBlackboardFunction
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

    internal partial class IsSetEffectorBlackboardFunction
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

    internal partial class ObjectEqualsEffectorBlackboardFunction
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