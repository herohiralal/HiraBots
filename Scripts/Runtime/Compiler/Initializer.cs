using UnityEngine;
using UnityEngine.Profiling;

namespace HiraBots
{
    internal static class Initializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            Profiler.BeginSample("HiraBots Initialization");
            Application.quitting -= Quit;
            Application.quitting += Quit;

            BlackboardUnsafeHelpers.ClearObjectCache();

            Profiler.BeginSample("Blackboard Template Compilation");
            var blackboardTemplateCompilerContext = new BlackboardTemplateCompilerContext();
            
            var btc = BlackboardTemplateCollection.Instance;
            var btcCount = btc.Count;
            for (var i = 0; i < btcCount; i++)
            {
                btc[i].Compile(blackboardTemplateCompilerContext);
                blackboardTemplateCompilerContext.Update();
            }
            Profiler.EndSample();

            Profiler.EndSample();
        }

        private static void Quit()
        {
            Application.quitting -= Quit;

            var btc = BlackboardTemplateCollection.Instance;
            var btcCount = btc.Count;
            for (var i = btcCount - 1; i >= 0; i--) btc[i].Free();

            BlackboardUnsafeHelpers.ClearObjectCache();
        }
    }
}