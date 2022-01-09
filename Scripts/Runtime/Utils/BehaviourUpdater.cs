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

        internal static IInterface instance { get; set; }
    }
}