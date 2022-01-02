using System.Collections;
using UnityEngine;

namespace HiraBots
{
    internal static class CoroutineRunner
    {
        internal static MonoBehaviour instance { get; set; }

        /// <summary>
        /// Start a coroutine.
        /// </summary>
        internal static Coroutine Start(IEnumerator enumerator)
        {
            return instance.StartCoroutine(enumerator);
        }

        /// <summary>
        /// Stop an already running coroutine.
        /// </summary>
        internal static void Stop(Coroutine coroutine)
        {
            instance.StopCoroutine(coroutine);
        }
    }
}