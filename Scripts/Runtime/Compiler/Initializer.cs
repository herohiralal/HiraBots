using UnityEngine;

namespace HiraBots
{
    internal static class Initializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            Application.quitting -= Quit;
            Application.quitting += Quit;

            BlackboardUnsafeHelpers.ClearObjectCache();
        }

        private static void Quit()
        {
            Application.quitting -= Quit;
            BlackboardUnsafeHelpers.ClearObjectCache();
        }
    }
}