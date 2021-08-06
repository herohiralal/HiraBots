#if UNITY_EDITOR || HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
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
}
#endif