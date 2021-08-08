﻿using UnityEngine;

#if UNITY_EDITOR
namespace HiraBots
{
    internal partial class AlwaysSucceedDecoratorBlackboardFunction
    {
    }
    
    internal partial class BooleanEqualsDecoratorBlackboardFunction
    {
        protected override void OnValidate()
        {
            base.OnValidate();

            m_Key.keyTypesFilter = BlackboardKeyType.Boolean;
        }

        internal override void OnTargetBlackboardTemplateChanged(BlackboardTemplate newTemplate, ReadOnlyHashSetAccessor<BlackboardKey> keySet)
        {
            base.OnTargetBlackboardTemplateChanged(newTemplate, keySet);

            m_Key.OnTargetBlackboardTemplateChanged(newTemplate, keySet);
        }
    }

    internal partial class EnumEqualsBlackboardFunction
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
}
#endif