using HiraBots;

namespace UnityEngine.AI
{
    public static class GeneratedBlackboardHelpers
    {
        /// <summary>
        /// Convert instance id to object.
        /// </summary>
        public static Object InstanceIDToObject(int input)
        {
            return ObjectUtils.InstanceIDToObject(input);
        }

        /// <summary>
        /// Convert object to instance id.
        /// </summary>
        public static int ObjectToInstanceID(Object input)
        {
            return input.GetInstanceID();
        }
    }
}