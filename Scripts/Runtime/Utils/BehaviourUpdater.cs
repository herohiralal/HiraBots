using System.Runtime.CompilerServices;
using UnityEngine;

namespace HiraBots
{
    internal interface IUpdatableBehaviour
    {
        void Tick(float deltaTime);
    }

    internal static class BehaviourUpdater
    {
        internal interface IInterface
        {
            void Add(IUpdatableBehaviour behaviour, float tickInterval, float tickIntervalMultiplier = 1f);
            void Remove(IUpdatableBehaviour behaviour);
            void ChangeTimeElapsedSinceLastTick(IUpdatableBehaviour behaviour, float timeElapsed);
            void ChangeTickInterval(IUpdatableBehaviour behaviour, float tickInterval);
            void ChangeTickPaused(IUpdatableBehaviour behaviour, bool value);
        }

        internal static IInterface instance { private get; set; }

        /// <summary>
        /// Add a behaviour to the ticking list.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Add(IUpdatableBehaviour behaviour, float tickInterval, float tickIntervalMultiplier = 1f)
        {
            instance?.Add(behaviour, tickInterval, tickIntervalMultiplier);
        }

        /// <summary>
        /// Remove a behaviour from the ticking list.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Remove(IUpdatableBehaviour behaviour)
        {
            instance?.Remove(behaviour);
        }

        /// <summary>
        /// Change the ticking interval.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ChangeTickInterval(IUpdatableBehaviour behaviour, float tickInterval)
        {
            instance?.ChangeTickInterval(behaviour, tickInterval);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void WrappedTick(this IUpdatableBehaviour behaviour, float deltaTime)
        {
            try
            {
                behaviour.Tick(deltaTime);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}