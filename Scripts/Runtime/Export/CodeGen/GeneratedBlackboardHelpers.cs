using HiraBots;

namespace UnityEngine
{
    public static class GeneratedBlackboardHelpers
    {
        /// <summary>
        /// Convert instance id to object.
        /// </summary>
        public static Object InstanceIDToObject(int input)
        {
            return BlackboardUnsafeHelpers.InstanceIDToObject(input);
        }

        /// <summary>
        /// Convert object to instance id.
        /// </summary>
        public static int ObjectToInstanceID(Object input)
        {
            return BlackboardUnsafeHelpers.ObjectToInstanceID(input);
        }

        /// <summary>
        /// No-op function to correctly compile.
        /// </summary>
        public static void DisposeResourcesRelatedToExistingValue<T>(T input)
        {
            // intentionally no-op
        }

        /// <summary>
        /// Remove object instance from cache.
        /// </summary>
        public static void DisposeResourcesRelatedToExistingValueActual(int instanceID)
        {
            BlackboardUnsafeHelpers.RemoveInstanceFromObjectCache(instanceID);
        }
    }
}