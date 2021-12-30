#if UNITY_EDITOR
using UnityEngine;

namespace HiraBots
{
    internal partial class AlwaysSucceedDecoratorBlackboardFunction
    {
        internal override void OnValidate()
        {
            base.OnValidate();

            UpdateDescription();
        }

        internal override void OnTargetBlackboardTemplateChanged(BlackboardTemplate newTemplate, ReadOnlyHashSetAccessor<BlackboardKey> keySet)
        {
            base.OnTargetBlackboardTemplateChanged(newTemplate, keySet);

            UpdateDescription();
        }

        private void UpdateDescription()
        {
            m_Description = m_Header.m_Invert
                ? m_Header.m_IsScoreCalculator
                    ? "[USELESS]."
                    : "Always fail. Used to disable a goal/task for debugging purposes."
                : m_Header.m_IsScoreCalculator
                    ? scoreString
                    : "[USELESS].";
        }
    }

    internal partial class EnumHasFlagsDecoratorBlackboardFunction
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

            m_Description = m_Header.m_IsScoreCalculator
                ? $"{scoreString} if {m_Key.selectedKey.name} {(m_Header.m_Invert ? "doesn't have" : "has")} these flags."
                : $"{m_Key.selectedKey.name} {(m_Header.m_Invert ? "must not" : "must")} have these flags.";
        }
    }

    internal partial class IsSetDecoratorBlackboardFunction
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

            var verb = m_Header.m_IsScoreCalculator
                ? m_Header.m_Invert
                    ? "is not"
                    : "is"
                : m_Header.m_Invert
                    ? "must not be"
                    : "must be";

            m_Description = m_Header.m_IsScoreCalculator
                ? $"{scoreString} if "
                : "";

            m_Description += $"{m_Key.selectedKey.name} {verb} set.";
        }
    }

    internal partial class NumericalComparisonDecoratorBlackboardFunction
    {
        internal override void OnValidate()
        {
            base.OnValidate();

            m_Key.keyTypesFilter = BlackboardKeyType.Numeric;

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

            var value = $"{m_Value}";
            string comparison;

            switch (m_ComparisonType)
            {
                case ComparisonType.AlmostEqualTo:
                    if (m_EqualityTolerance == 0f)
                    {
                        comparison = "equal to";
                    }
                    else
                    {
                        comparison = "almost equal to";
                        value = $"{value} ± {m_EqualityTolerance}";
                    }
                    break;
                case ComparisonType.GreaterThan:
                    comparison = "greater than";
                    break;
                case ComparisonType.GreaterThanEqualTo:
                    comparison = "greater than or equal to";
                    break;
                case ComparisonType.LesserThan:
                    comparison = "lesser than";
                    break;
                case ComparisonType.LesserThanEqualTo:
                    comparison = "lesser than or equal to";
                    break;
                default:
                    m_Description = "";
                    return;
            }

            var verb = m_Header.m_IsScoreCalculator
                ? m_Header.m_Invert
                    ? "is not"
                    : "is"
                : m_Header.m_Invert
                    ? "must not be"
                    : "must be";

            m_Description = m_Header.m_IsScoreCalculator
                ? $"{scoreString} if "
                : "";

            m_Description += $"{m_Key.selectedKey.name} {verb} {comparison} {value}.";
        }
    }

    internal partial class ObjectEqualsDecoratorBlackboardFunction
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

            var value = m_Value == null
                ? "null"
                : m_Value.name;

            var verb = m_Header.m_IsScoreCalculator
                ? m_Header.m_Invert
                    ? "is not"
                    : "is"
                : m_Header.m_Invert
                    ? "must not be"
                    : "must be";

            m_Description = m_Header.m_IsScoreCalculator
                ? $"{scoreString} if "
                : "";

            m_Description += $"{m_Key.selectedKey} {verb} set to {value}.";
        }
    }
}
#endif