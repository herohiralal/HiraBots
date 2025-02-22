using System.Runtime.CompilerServices;
using UnityEngine.AI;

namespace HiraBots
{
    internal static class ServiceRunner
    {
        internal interface IInterface
        {
            void Add(IHiraBotsService service, float tickInterval, float tickIntervalMultiplier);
            void Remove(IHiraBotsService service);
            void ChangeTickIntervalMultiplier(IHiraBotsService service, float tickIntervalMultiplier);
            void ChangeTickPaused(IHiraBotsService service, bool value);
        }

        internal static IInterface instance { private get; set; }

        /// <summary>
        /// Register a service.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Add(IHiraBotsService service, float tickInterval, float tickIntervalMultiplier)
        {
            instance?.Add(service, tickInterval, tickIntervalMultiplier);
        }

        /// <summary>
        /// Unregister a service.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Remove(IHiraBotsService service)
        {
            instance?.Remove(service);
        }

        /// <summary>
        /// Change the tick interval multiplier of a service.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ChangeTickIntervalMultiplier(IHiraBotsService service, float tickIntervalMultiplier)
        {
            instance?.ChangeTickIntervalMultiplier(service, tickIntervalMultiplier);
        }

        /// <summary>
        /// Pause/unpause a service.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ChangeTickPaused(IHiraBotsService service, bool value)
        {
            instance?.ChangeTickPaused(service, value);
        }
    }
}