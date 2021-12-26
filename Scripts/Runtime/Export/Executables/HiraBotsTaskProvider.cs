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

    public abstract partial class HiraBotsTaskProvider : ScriptableObject
    {
        [SerializeField] private float m_TickInterval = 0f;

        public float tickInterval
        {
            get => m_TickInterval;
            set => m_TickInterval = Mathf.Max(0, value);
        }

        public abstract IHiraBotsTask GetTask(BlackboardComponent blackboard);
    }
}