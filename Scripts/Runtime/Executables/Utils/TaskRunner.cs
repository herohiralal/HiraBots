using UnityEngine.AI;

namespace HiraBots
{
    internal static class TaskRunner
    {
        internal interface IInterface
        {
            void Add(ExecutorComponent executor, IHiraBotsTask task, float tickInterval, float tickIntervalMultiplier);
            void Remove(ExecutorComponent executor);
            void ChangeTickIntervalMultiplier(ExecutorComponent executor, float tickIntervalMultiplier);
            void ChangeTickPaused(ExecutorComponent executor, bool value);
        }

        internal static IInterface instance { get; set; }
    }
}