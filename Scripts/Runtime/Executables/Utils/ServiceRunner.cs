using UnityEngine;

namespace HiraBots
{
    internal static class ServiceRunner
    {
        internal interface IInterface
        {
            void Add(IHiraBotsService service, float tickInterval, float timeDilation);
            void Remove(IHiraBotsService service);
            void ChangeServiceTimeDilation(IHiraBotsService service, float timeDilation);
        }

        internal static IInterface instance { get; set; }
    }
}