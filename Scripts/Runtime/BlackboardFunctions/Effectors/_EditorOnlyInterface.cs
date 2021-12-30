#if UNITY_EDITOR
using UnityEngine;

namespace HiraBots
{
    internal partial class EnumOperatorEffectorBlackboardFunction
    {
        internal override void OnValidate()
        {
            base.OnValidate();

            m_Key.keyTypesFilter = BlackboardKeyType.Enum;

            m_Value.m_TypeIdentifier = m_Key.selectedKey is EnumBlackboardKey enumKey
                ? enumKey.typeIdentifier
                : "";

            UpdateDescription();
        }

        internal override void OnTargetBlackboardTemplateChanged(BlackboardTemplate newTemplate, ReadOnlyHashSetAccessor<BlackboardKey> keySet)
        {
            base.OnTargetBlackboardTemplateChanged(newTemplate, keySet);

            m_Key.OnTargetBlackboardTemplateChanged(newTemplate, keySet);

            UpdateDescription();
        }

        private void UpdateDescription()
        {
            if (m_Key.selectedKey == null)
            {
                m_Description = "";
                return;
            }

            switch (m_OperationType)
            {
                case OperationType.Set:
                    m_Description = $"Set {m_Key.selectedKey.name} to this value.";
                    break;
                case OperationType.AddFlags:
                    m_Description = $"Remove these flags to {m_Key.selectedKey.name}.";
                    break;
                case OperationType.RemoveFlags:
                    m_Description = $"Remove these flags from {m_Key.selectedKey.name}.";
                    break;
                default:
                    m_Description = "";
                    return;
            }
        }
    }

    internal partial class FloatOperatorEffectorBlackboardFunction
    {
        internal override void OnValidate()
        {
            base.OnValidate();

            if (m_OperationType == OperationType.Divide && m_Value == 0f)
            {
                m_Value = 0.1f;
            }

            m_Key.keyTypesFilter = BlackboardKeyType.Float;

            UpdateDescription();
        }

        internal override void OnTargetBlackboardTemplateChanged(BlackboardTemplate newTemplate, ReadOnlyHashSetAccessor<BlackboardKey> keySet)
        {
            base.OnTargetBlackboardTemplateChanged(newTemplate, keySet);

            m_Key.OnTargetBlackboardTemplateChanged(newTemplate, keySet);

            UpdateDescription();
        }

        private void UpdateDescription()
        {
            if (m_Key.selectedKey == null)
            {
                m_Description = "";
                return;
            }

            switch (m_OperationType)
            {
                case OperationType.Set:
                    m_Description = $"Set {m_Key.selectedKey.name} to {m_Value}.";
                    break;
                case OperationType.Add:
                    m_Description = $"Add {m_Value} to {m_Key.selectedKey.name}.";
                    break;
                case OperationType.Subtract:
                    m_Description = $"Subtract {m_Value} from {m_Key.selectedKey.name}.";
                    break;
                case OperationType.Multiply:
                    m_Description = $"Multiply {m_Key.selectedKey.name} by {m_Value}.";
                    break;
                case OperationType.Divide:
                    m_Description = $"Divide {m_Key.selectedKey.name} by {m_Value}.";
                    break;
                default:
                    m_Description = "";
                    return;
            }
        }
    }

    internal partial class IntegerOperatorEffectorBlackboardFunction
    {
        internal override void OnValidate()
        {
            base.OnValidate();

            if (m_OperationType == OperationType.Divide && m_Value == 0)
            {
                m_Value = 1;
            }

            m_Key.keyTypesFilter = BlackboardKeyType.Integer;

            UpdateDescription();
        }

        internal override void OnTargetBlackboardTemplateChanged(BlackboardTemplate newTemplate, ReadOnlyHashSetAccessor<BlackboardKey> keySet)
        {
            base.OnTargetBlackboardTemplateChanged(newTemplate, keySet);

            m_Key.OnTargetBlackboardTemplateChanged(newTemplate, keySet);

            UpdateDescription();
        }

        private void UpdateDescription()
        {
            if (m_Key.selectedKey == null)
            {
                m_Description = "";
                return;
            }

            switch (m_OperationType)
            {
                case OperationType.Set:
                    m_Description = $"Set {m_Key.selectedKey.name} to {m_Value}.";
                    break;
                case OperationType.Add:
                    m_Description = $"Add {m_Value} to {m_Key.selectedKey.name}.";
                    break;
                case OperationType.Subtract:
                    m_Description = $"Subtract {m_Value} from {m_Key.selectedKey.name}.";
                    break;
                case OperationType.Multiply:
                    m_Description = $"Multiply {m_Key.selectedKey.name} by {m_Value}.";
                    break;
                case OperationType.Divide:
                    m_Description = $"Divide {m_Key.selectedKey.name} by {m_Value}.";
                    break;
                case OperationType.BitwiseAnd:
                    m_Description = $"{m_Key.selectedKey.name} &= {m_Value}.";
                    break;
                case OperationType.BitwiseOr:
                    m_Description = $"{m_Key.selectedKey.name} |= {m_Value}.";
                    break;
                case OperationType.BitwiseXor:
                    m_Description = $"{m_Key.selectedKey.name} ^= {m_Value}.";
                    break;
                default:
                    m_Description = "";
                    return;
            }
        }
    }

    internal partial class IsSetEffectorBlackboardFunction
    {
        internal override void OnValidate()
        {
            base.OnValidate();

            m_Key.keyTypesFilter = BlackboardKeyType.UnmanagedSettable;

            UpdateDescription();
        }

        internal override void OnTargetBlackboardTemplateChanged(BlackboardTemplate newTemplate, ReadOnlyHashSetAccessor<BlackboardKey> keySet)
        {
            base.OnTargetBlackboardTemplateChanged(newTemplate, keySet);

            m_Key.OnTargetBlackboardTemplateChanged(newTemplate, keySet);

            UpdateDescription();
        }

        private void UpdateDescription()
        {
            if (m_Key.selectedKey == null)
            {
                m_Description = "";
                return;
            }

            switch (m_OperationType)
            {
                case OperationType.Set:
                    m_Description = $"Set {m_Key.selectedKey.name}.";
                    break;
                case OperationType.Unset:
                    m_Description = $"Unset {m_Key.selectedKey.name}.";
                    break;
                default:
                    m_Description = "";
                    return;
            }
        }
    }

    internal partial class ObjectEqualsEffectorBlackboardFunction
    {
        internal override void OnValidate()
        {
            base.OnValidate();

            m_Key.keyTypesFilter = BlackboardKeyType.Object;

            UpdateDescription();
        }

        internal override void OnTargetBlackboardTemplateChanged(BlackboardTemplate newTemplate, ReadOnlyHashSetAccessor<BlackboardKey> keySet)
        {
            base.OnTargetBlackboardTemplateChanged(newTemplate, keySet);

            m_Key.OnTargetBlackboardTemplateChanged(newTemplate, keySet);

            UpdateDescription();
        }

        private void UpdateDescription()
        {
            if (m_Key.selectedKey == null)
            {
                m_Description = "";
                return;
            }

            var value = m_Value == null ? "null" : m_Value.name;

            m_Description = $"Set {m_Key.selectedKey} to {value}.";
        }
    }
}
#endif