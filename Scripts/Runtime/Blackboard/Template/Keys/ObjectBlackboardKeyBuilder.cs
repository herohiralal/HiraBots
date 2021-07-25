#if UNITY_EDITOR // ideally manual creation must only be done for testing purposes as it skips validation checks
namespace HiraBots
{
    internal partial class ObjectBlackboardKey
    {
        protected internal ObjectBlackboardKey(BlackboardKeyTraits traits, byte sizeInBytes, BlackboardKeyType keyType, UnityEngine.Object defaultValue)
            : base(traits, sizeInBytes, keyType) =>
            this.defaultValue = defaultValue;
    }
}
#endif