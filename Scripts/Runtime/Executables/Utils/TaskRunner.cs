using System.Runtime.CompilerServices;
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

        internal static IInterface instance { private get; set; }

        /// <summary>
        /// Register an executor.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Add(ExecutorComponent executor, IHiraBotsTask task, float tickInterval, float tickIntervalMultiplier)
        {
            instance?.Add(executor, task, tickInterval, tickIntervalMultiplier);
        }

        /// <summary>
        /// Unregister an executor.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Remove(ExecutorComponent service)
        {
            instance?.Remove(service);
        }

        /// <summary>
        /// Change the tick interval multiplier of an executor.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ChangeTickIntervalMultiplier(ExecutorComponent executor, float tickIntervalMultiplier)
        {
            instance?.ChangeTickIntervalMultiplier(executor, tickIntervalMultiplier);
        }

        /// <summary>
        /// Pause/unpause an executor.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ChangeTickPaused(ExecutorComponent executor, bool value)
        {
            instance?.ChangeTickPaused(executor, value);
        }
    }
}