namespace UnityEngine
{
    public abstract class GeneratedBlackboardTemplate : IGeneratedBlackboardInstanceSyncListener
    {
        internal abstract bool GetInstanceSyncedBooleanValue(string key);
        internal abstract T GetInstanceSyncedEnumValue<T>(string key) where T : unmanaged, System.Enum;
        internal abstract float GetInstanceSyncedFloatValue(string key);
        internal abstract int GetInstanceSyncedIntegerValue(string key);
        internal abstract Object GetInstanceSyncedObjectValue(string key);
        internal abstract Unity.Mathematics.float3 GetInstanceSyncedVectorValue(string key);
        internal abstract Unity.Mathematics.quaternion GetInstanceSyncedQuaternionValue(string key);
    }
}