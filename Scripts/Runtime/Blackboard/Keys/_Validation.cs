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
            if (m_KeyType == BlackboardKeyType.Invalid)
            {
                context.succeeded = false;
            }

            ValidateInternal(ref context);
        }

        protected virtual void ValidateInternal(ref BlackboardKeyValidatorContext context)
        {
        }
    }
}