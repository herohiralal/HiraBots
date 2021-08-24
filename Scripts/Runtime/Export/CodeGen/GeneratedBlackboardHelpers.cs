using HiraBots;

namespace UnityEngine
{
    public static class GeneratedBlackboardHelpers
    {
        public static Object InstanceIDToObject(int input)
        {
            return BlackboardUnsafeHelpers.InstanceIDToObject(input);
        }

        public static int ObjectToInstanceID(Object input)
        {
            return BlackboardUnsafeHelpers.ObjectToInstanceID(input);
        }

        public static void DisposeResourcesRelatedToExistingValue<T>(T input)
        {
            // intentionally no-op
        }

        public static void DisposeResourcesRelatedToExistingValueActual(int instanceID)
        {
            BlackboardUnsafeHelpers.RemoveInstanceFromObjectCache(instanceID);
        }
    }
}