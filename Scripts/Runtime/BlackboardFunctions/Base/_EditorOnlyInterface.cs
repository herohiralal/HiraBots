#if UNITY_EDITOR
using UnityEngine;

namespace HiraBots
{
    internal abstract partial class BlackboardFunction
    {
        [SerializeField, HideInInspector] private string m_Subtitle = "";
        internal ref string subtitle => ref m_Subtitle;

        [SerializeField, HideInInspector] protected string m_Description = "";
        internal string description => m_Description;

        internal virtual void OnValidate()
        {
        }

        /// <summary>
        /// Callback to allow updating template in key selectors.
        /// </summary>
        /// <param name="newTemplate">The new template.</param>
        /// <param name="keySet">The allowed key-set (including parent keys).</param>
        internal virtual void OnTargetBlackboardTemplateChanged(BlackboardTemplate newTemplate, ReadOnlyHashSetAccessor<BlackboardKey> keySet)
        {
        }
    }

    internal abstract partial class BlackboardFunction<TFunction>
        where TFunction : System.Delegate
    {
    }

    internal abstract partial class DecoratorBlackboardFunction
    {
        protected string scoreString => $"{(m_Header.m_Score >= 0 ? '+' : '-')}{m_Header.m_Score}.";
    }

    internal abstract partial class EffectorBlackboardFunction
    {
    }
}
#endif