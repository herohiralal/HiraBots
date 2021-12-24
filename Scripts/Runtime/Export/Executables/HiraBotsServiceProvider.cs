namespace UnityEngine
{
    public interface IHiraBotsService
    {
        void Start();
        void Tick();
        void Stop();
    }

    public abstract class HiraBotsServiceProvider : ScriptableObject
    {
        public abstract IHiraBotsService GetService(BlackboardComponent blackboard);
    }
}