﻿using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

namespace HiraBots
{
    /// <summary>
    /// This class is responsible for all runtime initialization and cleanup.
    /// </summary>
    [AddComponentMenu("")]
    internal class HiraBotsInitializer : MonoBehaviour
    {
        // compile all HiraBots components in their required order
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Load()
        {
            Profiler.BeginSample("HiraBots Initialization");

            {
                var go = new GameObject("[HiraBots]")
                {
                    hideFlags = HideFlags.HideAndDontSave
                };

                DontDestroyOnLoad(go);

                try
                {
                    go.AddComponent<HiraBotsInitializer>();
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }

                try
                {
                    go.AddComponent<HiraBotsModule>();
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }

            BlackboardComponent.ResetStaticIDAssigner();
            LGOAPPlannerComponent.ResetStaticIDAssigner();
            TacMapComponent.ResetStaticIDAssigner();
            ExecutorComponent.ResetStaticIDAssigner();

            CompilationRegistry.Initialize();

            Profiler.BeginSample("Blackboard Template Compilation");

            var btc = BlackboardTemplateCollection.instance;
            var btcCount = btc.count;
            for (var i = 0; i < btcCount; i++)
            {
                try
                {
                    // the assumption here is that the blackboard templates are arranged
                    // according to their hierarchy indices, to make sure a parent template
                    // does not get compiled before its child
                    btc[i].Compile();
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }

            Profiler.EndSample();

            ErrorTaskProvider.CreateNoneTaskInstance();

            Profiler.BeginSample("LGOAP Domain Compilation");

            var ldc = LGOAPDomainCollection.instance;
            var ldcCount = ldc.count;
            for (var i = 0; i < ldcCount; i++)
            {
                try
                {
                    // the assumption here is that the blackboard templates have already compiled
                    ldc[i].Compile();
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }

            Profiler.EndSample();

            CompilationRegistry.Build();

            TacMap.s_ActiveMaps.Clear();
            HiraLGOAPRealtimeBot.s_ActiveBots.Clear();

            Profiler.EndSample();
        }

        private void OnApplicationQuit()
        {
            Profiler.BeginSample("HiraBots Cleanup");

            try
            {
                Unload(gameObject);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

            Profiler.EndSample();
        }

        // Free all the compiled HiraBots components in a reverse order
        private static void Unload(GameObject gameObject)
        {
            for (var i = HiraLGOAPRealtimeBot.s_ActiveBots.Count - 1; i >= 0; i--)
            {
                try
                {
                    HiraLGOAPRealtimeBot.s_ActiveBots[i].Dispose();
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }

            HiraLGOAPRealtimeBot.s_ActiveBots.Clear();

            for (var i = TacMap.s_ActiveMaps.Count - 1; i >= 0; i--)
            {
                try
                {
                    TacMap.s_ActiveMaps[i].Dispose();
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }

            TacMap.s_ActiveMaps.Clear();

            try
            {
                gameObject.GetComponent<HiraBotsModule>().Dispose();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

            var ldc = LGOAPDomainCollection.instance;
            var ldcCount = ldc.count;
            for (var i = ldcCount - 1; i >= 0; i--)
            {
                try
                {
                    // need to free them before the blackboards they depend on
                    ldc[i].Free();
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }

            ErrorTaskProvider.ClearNoneTaskInstance();

            var btc = BlackboardTemplateCollection.instance;
            var btcCount = btc.count;
            for (var i = btcCount - 1; i >= 0; i--)
            {
                try
                {
                    // free all the compiled blackboards in reverse, to
                    // free them backwards from the order they were compiled in
                    btc[i].Free();
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }

            // get rid of all collections
            // needed to work correctly when domain reload on play mode state change is disabled
            LGOAPDomainCollection.ClearInstance();
            BlackboardTemplateCollection.ClearInstance();

            CompilationRegistry.Clear();

            Destroy(gameObject);
        }
    }
}