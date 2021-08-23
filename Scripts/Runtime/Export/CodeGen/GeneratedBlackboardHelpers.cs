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
            return input.GetInstanceID();
        }
    }
}