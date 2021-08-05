using System.Runtime.CompilerServices;

namespace HiraBots
{
    /// <summary>
    /// The context required to validate a blackboard key.
    /// </summary>
    internal interface IBlackboardKeyValidatorContext
    {
        /// <summary>
        /// Mark the validation as unsuccessful.
        /// </summary>
        void MarkUnsuccessful();
    }

    internal abstract partial class BlackboardKey
    {
        /// <summary>
        /// Validate this blackboard key.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Validate(IBlackboardKeyValidatorContext context)
        {
            if (m_KeyType == BlackboardKeyType.Invalid)
                context.MarkUnsuccessful();
        }
    }
}