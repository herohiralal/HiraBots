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
            void Add(IUpdatableBehaviour behaviour, float tickInterval, float tickIntervalMultiplier);
            void Remove(IUpdatableBehaviour behaviour);
            void ChangeTickInterval(IUpdatableBehaviour behaviour, float tickInterval);
            void ChangeTickPaused(IUpdatableBehaviour behaviour, bool value);
        }

        internal static IInterface instance { get; set; }
    }
}