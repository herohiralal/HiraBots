using UnityEngine;
using UnityEngine.Profiling;

namespace HiraBots
{
    /// <summary>
    /// This class is responsible for all runtime initialization and cleanup.
    /// </summary>
    internal static class Initializer
    {
        // compile all HiraBots components in their required order
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            Profiler.BeginSample("HiraBots Initialization");

            Application.quitting -= Quit;
            Application.quitting += Quit;

            BlackboardUnsafeHelpers.ClearObjectCache();

            Profiler.BeginSample("Blackboard Template Compilation");

            var blackboardTemplateCompiler = new BlackboardTemplateCompiler();

            var btc = BlackboardTemplateCollection.instance;
            var btcCount = btc.count;
            for (var i = 0; i < btcCount; i++)
            {
                // the assumption here is that the blackboard templates are arranged
                // according to their hierarchy indices, to make sure a parent template
                // does not get compiled before its child
                blackboardTemplateCompiler.Compile(btc[i]);
            }

            Profiler.EndSample();

            Profiler.EndSample();
        }

        // Free all the compiled HiraBots components in a reverse order
        private static void Quit()
        {
            Profiler.BeginSample("HiraBots Cleanup");

            Application.quitting -= Quit;

            var btc = BlackboardTemplateCollection.instance;
            var btcCount = btc.count;
            for (var i = btcCount - 1; i >= 0; i--)
            {
                // free all the compiled blackboards in reverse, to
                // free them backwards from the order they were compiled in
                btc[i].Free();
            }

            BlackboardUnsafeHelpers.ClearObjectCache();

            Profiler.EndSample();
        }
    }
}