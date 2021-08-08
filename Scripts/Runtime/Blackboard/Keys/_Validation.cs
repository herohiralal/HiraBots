#if UNITY_EDITOR || HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
using System.Collections.Generic;

namespace HiraBots
{
    /// <summary>
    /// The context required to validate a blackboard key.
    /// </summary>
    internal struct BlackboardKeyValidatorContext
    {
        /// <summary>
        /// Whether the validation succeeded.
        /// </summary>
        internal bool succeeded { get; set; }
    }

    internal abstract partial class BlackboardKey
    {
        /// <summary>
        /// Validate this blackboard key.
        /// </summary>
        internal void Validate(ref BlackboardKeyValidatorContext context)
        {
            switch (m_KeyType)
            {
                case BlackboardKeyType.Boolean:
                case BlackboardKeyType.Enum:
                case BlackboardKeyType.Float:
                case BlackboardKeyType.Integer:
                case BlackboardKeyType.Object:
                case BlackboardKeyType.Quaternion:
                case BlackboardKeyType.Vector:
                    break;
                default:
                    context.succeeded = false;
                    break;
            }

            ValidateInternal(ref context);
        }

        protected virtual void ValidateInternal(ref BlackboardKeyValidatorContext context)
        {
        }
    }

    internal struct BlackboardTemplateKeySelectorValidatorContext
    {
        /// <summary>
        /// Whether the validation succeeded.
        /// </summary>
        internal bool succeeded { get; set; }
        
        /// <summary>
        /// The pool of allowed keys.
        /// </summary>
        internal HashSet<BlackboardKey> allowedKeyPool { get; set; }

        /// <summary>
        /// The allowed key types.
        /// </summary>
        internal BlackboardKeyType allowedKeyTypes { get; set; }
    }

    internal partial class BlackboardKey
    {
        internal partial struct Selector
        {
            /// <summary>
            /// Validate whether the selected key is compatible with the filters.
            /// </summary>
            internal void Validate(ref BlackboardTemplateKeySelectorValidatorContext context)
            {
                // validate key itself
                if (m_Key == null)
                {
                    context.succeeded = false;
                    return;
                }

                // validate template filter
                if (!context.allowedKeyPool.Contains(m_Key))
                {
                    context.succeeded = false;
                }

                // validate key types filter
                if (!context.allowedKeyTypes.HasFlag(m_Key.keyType))
                {
                    context.succeeded = false;
                }
            }
        }
    }
}
#endif