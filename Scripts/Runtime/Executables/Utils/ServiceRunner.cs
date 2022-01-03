using UnityEngine;

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

        internal static IInterface instance { get; set; }
    }
}