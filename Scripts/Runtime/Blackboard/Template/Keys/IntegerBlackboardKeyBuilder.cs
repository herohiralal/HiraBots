#if UNITY_EDITOR // ideally manual creation must only be done for testing purposes as it skips validation checks
namespace HiraBots
{
    internal partial class IntegerBlackboardKey
    {
        protected internal IntegerBlackboardKey(BlackboardKeyTraits traits, byte sizeInBytes, BlackboardKeyType keyType, int defaultValue)
            : base(traits, sizeInBytes, keyType) =>
            this.defaultValue = defaultValue;
    }
}
#endif