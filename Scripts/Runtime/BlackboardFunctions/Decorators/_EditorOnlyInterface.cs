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

            if (m_Key.selectedKey is EnumBlackboardKey enumKey)
            {
                m_Value.m_TypeIdentifier = enumKey.typeIdentifier;
            }
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

            m_Key.keyTypesFilter = BlackboardKeyType.Boolean | BlackboardKeyType.Object | BlackboardKeyType.Quaternion | BlackboardKeyType.Vector;
        }

        internal override void OnTargetBlackboardTemplateChanged(BlackboardTemplate newTemplate, ReadOnlyHashSetAccessor<BlackboardKey> keySet)
        {
            base.OnTargetBlackboardTemplateChanged(newTemplate, keySet);

            m_Key.OnTargetBlackboardTemplateChanged(newTemplate, keySet);
        }
    }
}
#endif