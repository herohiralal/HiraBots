#if UNITY_EDITOR || HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
using System.Collections.Generic;
using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// The context to validate a blackboard function.
    /// </summary>
    internal struct BlackboardFunctionValidatorContext
    {
        /// <summary>
        /// The info regarding a bad key.
        /// </summary>
        internal struct BadKeyInfo
        {
            /// <summary>
            /// The name of the blackboard function.
            /// </summary>
            internal string functionName { get; set; }

            /// <summary>
            /// The name of the badly assigned variable.
            /// </summary>
            internal string variableName { get; set; }

            /// <summary>
            /// The currently selected key.
            /// </summary>
            internal BlackboardKey selectedKey { get; set; }
        }

        /// <summary>
        /// Whether the validation succeeded.
        /// </summary>
        internal bool succeeded { get; set; }

        /// <summary>
        /// List of badly selected keys.
        /// </summary>
        internal List<BadKeyInfo> badlySelectedKeys { get; set; }

        /// <summary>
        /// The pool of allowed keys.
        /// </summary>
        internal ReadOnlyHashSetAccessor<BlackboardKey> allowedKeyPool { get; set; }
    }

    internal abstract partial class BlackboardFunction
    {
        /// <summary>
        /// Validate this blackboard function.
        /// </summary>
        internal virtual void Validate(ref BlackboardFunctionValidatorContext context)
        {
        }

        /// <summary>
        /// Validate a blackboard key selector.
        /// </summary>
        /// <param name="selector">The key selector in quesiton.</param>
        /// <param name="typesFilter">The valid types for the key.</param>
        /// <param name="context">function validator context.</param>
        /// <returns>Whether the validation was successful.</returns>
        protected static bool ValidateKeySelector(ref BlackboardTemplate.KeySelector selector,
            BlackboardKeyType typesFilter, ref BlackboardFunctionValidatorContext context)
        {
            var keySelectorValidator = new BlackboardTemplateKeySelectorValidatorContext
            {
                succeeded = true,
                allowedKeyPool = context.allowedKeyPool,
                allowedKeyTypes = typesFilter
            };

            selector.Validate(ref keySelectorValidator);

            return keySelectorValidator.succeeded;
        }
    }
}
#endif