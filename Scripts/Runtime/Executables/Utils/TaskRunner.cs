using UnityEngine;

namespace HiraBots
{
    internal static class TaskRunner
    {
        internal interface IInterface
        {
            void Add(ExecutorComponent executor, IHiraBotsTask task, float tickInterval, float tickIntervalMultiplier);
            void Remove(ExecutorComponent executor);
            void ChangeTickIntervalMultiplier(ExecutorComponent executor, float tickIntervalMultiplier);
        }

        internal static IInterface instance { get; set; }
    }
}