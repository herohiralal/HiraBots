namespace UnityEngine.AI
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
        [Tooltip("The interval between two ticks (to the most precise frame).")]
        [SerializeField] private float m_TickInterval = 0f;

        public float tickInterval
        {
            get => m_TickInterval;
            set => m_TickInterval = Mathf.Max(0, value);
        }

        internal IHiraBotsTask WrappedGetTask(HiraBots.BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            try
            {
                return GetTask(blackboard, archetype);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        protected abstract IHiraBotsTask GetTask(BlackboardComponent blackboard, IHiraBotArchetype archetype);
    }
}