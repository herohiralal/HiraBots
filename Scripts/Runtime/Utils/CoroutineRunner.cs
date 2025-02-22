using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace HiraBots
{
    internal static class CoroutineRunner
    {
        internal static MonoBehaviour instance { private get; set; }

        /// <summary>
        /// Start a coroutine.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Coroutine Start(IEnumerator enumerator)
        {
            if (ReferenceEquals(instance, null))
            {
                return null;
            }

            return instance.StartCoroutine(enumerator);
        }

        /// <summary>
        /// Stop an already running coroutine.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Stop(Coroutine coroutine)
        {
            if (!ReferenceEquals(instance, null) && !ReferenceEquals(coroutine, null))
            {
                instance.StopCoroutine(coroutine);
            }
        }
    }
}