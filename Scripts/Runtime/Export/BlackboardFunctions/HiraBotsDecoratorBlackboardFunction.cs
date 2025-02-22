namespace UnityEngine.AI
{
    public abstract class HiraBotsDecoratorBlackboardFunction : HiraBotsBlackboardFunction
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public unsafe delegate bool Delegate(in BlackboardComponent.LowLevel blackboard, byte* memory);

        [Tooltip("Whether to automatically test the decorator on the blackboard component during execution.")]
        [SerializeField] private bool m_CheckDuringExecution = true;

        public bool checkDuringExecution
        {
            get => m_CheckDuringExecution;
            set => m_CheckDuringExecution = value;
        }

        #region Execution

        public bool Execute(BlackboardComponent blackboard, bool expected)
        {
            if (!m_CheckDuringExecution)
            {
                return true;
            }

            try
            {
                return ExecuteFunction(blackboard, expected);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        protected virtual bool ExecuteFunction(BlackboardComponent blackboard, bool expected)
        {
            Debug.LogError($"{GetType()} does not implement ExecuteFunction(). This is not supposed to happen.");
            return false;
        }

        #endregion
    }
}