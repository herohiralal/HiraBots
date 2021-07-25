#if UNITY_EDITOR // ideally manual creation must only be done for testing purposes as it skips validation checks
namespace HiraBots
{
    internal abstract partial class BlackboardKey
    {
        protected internal BlackboardKey(BlackboardKeyTraits traits, byte sizeInBytes, BlackboardKeyType keyType)
        {
            instanceSynced = (traits & BlackboardKeyTraits.InstanceSynced) != 0;
            essentialToDecisionMaking = (traits & BlackboardKeyTraits.BroadcastEventOnUnexpectedChange) != 0;
            SizeInBytes = sizeInBytes;
            KeyType = keyType;
        }
    }
}
#endif