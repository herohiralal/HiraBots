#if UNITY_EDITOR // ideally manual creation must only be done for testing purposes as it skips validation checks
namespace HiraBots
{
    internal partial class QuaternionBlackboardKey
    {
        protected internal QuaternionBlackboardKey(BlackboardKeyTraits traits, byte sizeInBytes, BlackboardKeyType keyType, UnityEngine.Vector3 defaultValue)
            : base(traits, sizeInBytes, keyType) =>
            this.defaultValue = defaultValue;
    }
}
#endif