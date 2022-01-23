namespace UnityEngine
{
    public abstract class HiraBotsDecoratorBlackboardFunction : HiraBotsBlackboardFunction
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public unsafe delegate bool Delegate(in BlackboardComponent.LowLevel blackboard, byte* memory);

        #region Execution

        public bool Execute(BlackboardComponent blackboard, bool expected)
        {
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