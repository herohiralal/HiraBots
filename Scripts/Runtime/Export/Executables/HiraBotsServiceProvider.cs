namespace UnityEngine
{
    public interface IHiraBotsService
    {
        void Start();
        void Tick(float deltaTime);
        void Stop();
    }

    public abstract class HiraBotsServiceProvider : ScriptableObject
    {
        [SerializeField] private float m_TickRate = 0f;

        public float tickRate
        {
            get => m_TickRate;
            set => m_TickRate = Mathf.Max(0, value);
        }

        public abstract IHiraBotsService GetService(BlackboardComponent blackboard);
    }
}