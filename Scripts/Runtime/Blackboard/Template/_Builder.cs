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

    internal partial class BooleanBlackboardKey
    {
        protected internal BooleanBlackboardKey(BlackboardKeyTraits traits, byte sizeInBytes, BlackboardKeyType keyType, bool defaultValue)
            : base(traits, sizeInBytes, keyType) =>
            this.defaultValue = defaultValue;
    }

    internal partial class FloatBlackboardKey
    {
        protected internal FloatBlackboardKey(BlackboardKeyTraits traits, byte sizeInBytes, BlackboardKeyType keyType, float defaultValue)
            : base(traits, sizeInBytes, keyType) =>
            this.defaultValue = defaultValue;
    }

    internal partial class IntegerBlackboardKey
    {
        protected internal IntegerBlackboardKey(BlackboardKeyTraits traits, byte sizeInBytes, BlackboardKeyType keyType, int defaultValue)
            : base(traits, sizeInBytes, keyType) =>
            this.defaultValue = defaultValue;
    }

    internal partial class ObjectBlackboardKey
    {
        protected internal ObjectBlackboardKey(BlackboardKeyTraits traits, byte sizeInBytes, BlackboardKeyType keyType, UnityEngine.Object defaultValue)
            : base(traits, sizeInBytes, keyType) =>
            this.defaultValue = defaultValue;
    }

    internal partial class QuaternionBlackboardKey
    {
        protected internal QuaternionBlackboardKey(BlackboardKeyTraits traits, byte sizeInBytes, BlackboardKeyType keyType, UnityEngine.Vector3 defaultValue)
            : base(traits, sizeInBytes, keyType) =>
            this.defaultValue = defaultValue;
    }

    internal partial class VectorBlackboardKey
    {
        protected internal VectorBlackboardKey(BlackboardKeyTraits traits, byte sizeInBytes, BlackboardKeyType keyType, UnityEngine.Vector3 defaultValue)
            : base(traits, sizeInBytes, keyType) =>
            this.defaultValue = defaultValue;
    }

    internal partial class BlackboardTemplate
    {
        internal BlackboardTemplate(BlackboardTemplate parent, BlackboardKey[] keys)
        {
            this.parent = parent;
            this.keys = keys;
        }
    }
}

#endif