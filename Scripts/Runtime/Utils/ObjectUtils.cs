using System;
using System.Runtime.CompilerServices;
using Object = UnityEngine.Object;

namespace HiraBots
{
    internal static class ObjectUtils
    {
        static ObjectUtils()
        {
#if !UNITY_2020_3_OR_NEWER
            var instanceIDToObjectGetter = typeof(Object)
                .GetMethod("FindObjectFromInstanceID",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static,
                    null,
                    new[] { typeof(int) },
                    null);

            if (instanceIDToObjectGetter == null)
            {
                UnityEngine.Debug.LogError("Could not find instance ID to UnityEngine.Object getter method via reflection.");
            }
            else
            {
                s_InstanceIDToObjectGetter = (Func<int, Object>) Delegate.CreateDelegate(typeof(Func<int, Object>), instanceIDToObjectGetter);
            }
#else
            s_InstanceIDToObjectGetter = UnityEngine.Resources.InstanceIDToObject;
#endif

            if (s_InstanceIDToObjectGetter == null)
            {
                UnityEngine.Debug.LogError("Unity version (or current platform) incompatible with instance ID to Object conversion.");
            }
        }

        private static readonly Func<int, Object> s_InstanceIDToObjectGetter = null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Object InstanceIDToObject(int instanceID)
        {
            return s_InstanceIDToObjectGetter(instanceID);
        }
    }
}