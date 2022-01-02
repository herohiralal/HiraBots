using UnityEngine;

namespace HiraBots
{
    internal static class TaskRunner
    {
        internal interface IInterface
        {
            void Add(ExecutorComponent executor, IHiraBotsTask task, float tickInterval, float timeDilation);
            void Remove(ExecutorComponent executor);
            void ChangeTimeDilation(ExecutorComponent executor, float timeDilation);
        }

        internal static IInterface instance { get; set; }
    }
}