namespace UnityEngine.AI
{
    public abstract class HiraBotsEffectorBlackboardFunction : HiraBotsBlackboardFunction
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public unsafe delegate void Delegate(in BlackboardComponent.LowLevel blackboard, byte* memory);

        [Tooltip("Whether to automatically apply the effector on the blackboard component upon successful execution.")]
        [SerializeField] private bool m_AutoApplyOnSuccessfulExecution = true;

        public bool autoApplyOnSuccessfulExecution
        {
            get => m_AutoApplyOnSuccessfulExecution;
            set => m_AutoApplyOnSuccessfulExecution = value;
        }

        #region Execution

        public void Execute(BlackboardComponent blackboard, bool expected)
        {
            try
            {
                if (m_AutoApplyOnSuccessfulExecution)
                {
                    ExecuteFunction(blackboard, expected);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        protected virtual void ExecuteFunction(BlackboardComponent blackboard, bool expected)
        {
            Debug.LogError($"{GetType()} does not implement ExecuteFunction(). This is not supposed to happen.");
        }

        #endregion
    }
}