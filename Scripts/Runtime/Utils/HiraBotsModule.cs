using System;
using System.Collections;
using System.Collections.Generic;
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

            lock (s_ExecutionQueue)
            {
                s_ExecutionQueue.Clear();
            }
        }

        private void OnDestroy()
        {
            if (s_Instance != this)
            {
                return;
            }

            s_Instance = null;

            lock (s_ExecutionQueue)
            {
                s_ExecutionQueue.Clear();
            }
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

        private static readonly Queue<Action> s_ExecutionQueue = new Queue<Action>();

        internal static void DispatchOnMainThread(Action action)
        {
            if (action == null)
            {
                return;
            }

            lock (s_ExecutionQueue)
            {
                s_ExecutionQueue.Enqueue(action);
            }
        }

        private void Update()
        {
            lock (s_ExecutionQueue)
            {
                while (s_ExecutionQueue.Count > 0)
                {
                    try
                    {
                        s_ExecutionQueue.Dequeue()();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }
    }
}