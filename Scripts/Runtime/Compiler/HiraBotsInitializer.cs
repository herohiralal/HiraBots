using UnityEngine;
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
                go.AddComponent<HiraBotsInitializer>();
                go.AddComponent<HiraBotsModule>();
                go.AddComponent<HiraBotsTaskRunner>();
                go.AddComponent<HiraBotsServiceRunner>();
            }

            BlackboardComponent.ResetStaticIDAssigner();
            LGOAPPlannerComponent.ResetStaticIDAssigner();
            ExecutorComponent.ResetStaticIDAssigner();

            CompilationRegistry.Initialize();
            BlackboardUnsafeHelpers.ClearObjectCache();

            Profiler.BeginSample("Blackboard Template Compilation");

            var btc = BlackboardTemplateCollection.instance;
            var btcCount = btc.count;
            for (var i = 0; i < btcCount; i++)
            {
                // the assumption here is that the blackboard templates are arranged
                // according to their hierarchy indices, to make sure a parent template
                // does not get compiled before its child
                btc[i].Compile();
            }

            Profiler.EndSample();

            Profiler.BeginSample("LGOAP Domain Compilation");

            var ldc = LGOAPDomainCollection.instance;
            var ldcCount = ldc.count;
            for (var i = 0; i < ldcCount; i++)
            {
                // the assumption here is that the blackboard templates have already compiled
                ldc[i].Compile();
            }

            Profiler.EndSample();

            CompilationRegistry.Build();

            Profiler.EndSample();
        }

        private void OnApplicationQuit()
        {
            Destroy(GetComponent<HiraBotsServiceRunner>());
            Destroy(GetComponent<HiraBotsTaskRunner>());
            Destroy(GetComponent<HiraBotsModule>());
            Unload();
            Destroy(this);
            Destroy(gameObject);
        }

        // Free all the compiled HiraBots components in a reverse order
        private static void Unload()
        {
            Profiler.BeginSample("HiraBots Cleanup");

            var ldc = LGOAPDomainCollection.instance;
            var ldcCount = ldc.count;
            for (var i = ldcCount - 1; i >= 0; i--)
            {
                // need to free them before the blackboards they depend on
                ldc[i].Free();
            }

            var btc = BlackboardTemplateCollection.instance;
            var btcCount = btc.count;
            for (var i = btcCount - 1; i >= 0; i--)
            {
                // free all the compiled blackboards in reverse, to
                // free them backwards from the order they were compiled in
                btc[i].Free();
            }

            // get rid of all collections
            // needed to work correctly when domain reload on play mode state change is disabled
            LGOAPDomainCollection.ClearInstance();
            BlackboardTemplateCollection.ClearInstance();

            BlackboardUnsafeHelpers.ClearObjectCache();

            CompilationRegistry.Clear();

            Profiler.EndSample();
        }
    }
}