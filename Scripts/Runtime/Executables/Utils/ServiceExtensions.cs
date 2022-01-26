using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

namespace HiraBots
{
    internal static class ServiceExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void WrappedStart(this IHiraBotsService service)
        {
            try
            {
                service.Start();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void WrappedTick(this IHiraBotsService service, float deltaTime)
        {
            try
            {
                service.Tick(deltaTime);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void WrappedStop(this IHiraBotsService service)
        {
            try
            {
                service.Stop();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}