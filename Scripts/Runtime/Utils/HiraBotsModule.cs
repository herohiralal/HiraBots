using System.Collections;
using UnityEngine;

namespace HiraBots
{
    [AddComponentMenu("")]
    internal class HiraBotsModule : MonoBehaviour
    {
        #region Singleton

        private static HiraBotsModule s_Instance;

        private void Awake()
        {
            if (s_Instance != null)
            {
                Destroy(this);
                return;
            }

            s_Instance = this;
        }

        private void OnDestroy()
        {
            if (s_Instance != this)
            {
                return;
            }

            s_Instance = null;
        }

        #endregion

        /// <summary>
        /// Start a coroutine.
        /// </summary>
        internal new static Coroutine StartCoroutine(IEnumerator enumerator)
        {
            return ((MonoBehaviour) s_Instance)?.StartCoroutine(enumerator);
        }

        /// <summary>
        /// Stop an already running coroutine.
        /// </summary>
        internal new static void StopCoroutine(Coroutine coroutine)
        {
            ((MonoBehaviour) s_Instance)?.StopCoroutine(coroutine);
        }
    }
}