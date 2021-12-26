namespace UnityEngine
{
    public interface IHiraBotsService
    {
        void Start();
        void Tick(float deltaTime);
        void Stop();
    }

    public abstract partial class HiraBotsServiceProvider : ScriptableObject
    {
        [SerializeField] private float m_TickInterval = 0f;

        public float tickInterval
        {
            get => m_TickInterval;
            set => m_TickInterval = Mathf.Max(0, value);
        }

        public abstract IHiraBotsService GetService(BlackboardComponent blackboard);
    }
}