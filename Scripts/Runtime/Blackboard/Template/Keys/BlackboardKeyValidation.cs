#if UNITY_EDITOR // ideally validation is only needed within the editor (either when building, or when exiting play mode)
namespace HiraBots
{
    internal abstract partial class BlackboardKey
    {
        internal void Validate(IBlackboardKeyValidatorContext context)
        {
            if (KeyType == BlackboardKeyType.Invalid)
                context.MarkUnsuccessful();
        }
    }
}
#endif