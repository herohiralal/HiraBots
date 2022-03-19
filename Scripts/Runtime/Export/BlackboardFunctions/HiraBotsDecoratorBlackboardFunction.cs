namespace UnityEngine.AI
{
    public abstract class HiraBotsDecoratorBlackboardFunction : HiraBotsBlackboardFunction
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public unsafe delegate bool Delegate(in BlackboardComponent.LowLevel blackboard, byte* memory);

        [Tooltip("Whether to automatically test the decorator on the blackboard component prior to execution.")]
        [SerializeField] private bool m_CheckBeforeExecution = true;

        public bool checkBeforeExecution
        {
            get => m_CheckBeforeExecution;
            set => m_CheckBeforeExecution = value;
        }

        #region Execution

        public bool Execute(BlackboardComponent blackboard, bool expected)
        {
            if (!m_CheckBeforeExecution)
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