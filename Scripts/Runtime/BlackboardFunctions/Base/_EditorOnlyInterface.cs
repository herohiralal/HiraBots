#if UNITY_EDITOR
using UnityEngine;

namespace HiraBots
{
    internal abstract partial class BlackboardFunction
    {
        [SerializeField, HideInInspector] private string m_Subtitle = "";
        internal ref string subtitle => ref m_Subtitle;

        protected virtual void OnValidate()
        {
        }

        internal abstract class Serialized : CustomSerializedObject<BlackboardFunction>
        {
            protected Serialized(BlackboardFunction obj) : base(obj)
            {
            }

            internal void Validate()
            {
                target.OnValidate();
            }
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
        internal new abstract class Serialized : BlackboardFunction.Serialized
        {
            protected Serialized(BlackboardFunction<TFunction> obj) : base(obj)
            {
            }
        }
    }

    internal abstract partial class DecoratorBlackboardFunction
    {
        /// <summary>
        /// Whether this decorator is used as a score calculator.
        /// </summary>
        internal ref bool isScoreCalculator => ref m_Header.m_IsScoreCalculator;

        internal new class Serialized : BlackboardFunction<DecoratorDelegate>.Serialized
        {
            internal Serialized(DecoratorBlackboardFunction obj) : base(obj)
            {
            }
        }
    }

    internal abstract partial class EffectorBlackboardFunction
    {
        internal new class Serialized : BlackboardFunction<EffectorDelegate>.Serialized
        {
            internal Serialized(EffectorBlackboardFunction obj) : base(obj)
            {
            }
        }
    }
}
#endif