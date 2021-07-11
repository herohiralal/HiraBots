#if UNITY_EDITOR // ideally validation is only needed within the editor (either when building, or when exiting play mode
namespace HiraBots
{
    internal interface IBlackboardKeyValidatorContext
    {
        void MarkUnsuccessful();
    }
}
#endif