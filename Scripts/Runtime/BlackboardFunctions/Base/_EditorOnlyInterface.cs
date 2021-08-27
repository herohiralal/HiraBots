#if UNITY_EDITOR
using UnityEngine;

namespace HiraBots
{
    internal abstract partial class BlackboardFunction<TFunction>
        where TFunction : System.Delegate
    {
        protected virtual void OnValidate()
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
}
#endif