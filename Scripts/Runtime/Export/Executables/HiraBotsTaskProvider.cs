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
        [SerializeField] private float m_TickRate = 0f;

        public float tickRate
        {
            get => m_TickRate;
            set => m_TickRate = Mathf.Max(0, value);
        }

        public abstract IHiraBotsTask GetTask(BlackboardComponent blackboard);
    }
}