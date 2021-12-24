namespace UnityEngine
{
    public enum HiraBotsTaskResult : byte
    {
        InProgress,
        Succeeded,
        Failed
    }

    public interface IHiraBotsTask
    {
        void Begin();
        HiraBotsTaskResult Execute(float deltaTime);
        void Abort();
        void End(bool success);
    }

    public abstract class HiraBotsTaskProvider : ScriptableObject
    {
        public abstract IHiraBotsTask GetTask(BlackboardComponent blackboard);
    }
}