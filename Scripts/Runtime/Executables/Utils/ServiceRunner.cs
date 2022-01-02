using UnityEngine;

namespace HiraBots
{
    internal static class ServiceRunner
    {
        internal interface IInterface
        {
            void Add(IHiraBotsService service, float tickInterval);
            void Remove(IHiraBotsService service);
        }

        internal static IInterface instance { get; set; }
    }
}