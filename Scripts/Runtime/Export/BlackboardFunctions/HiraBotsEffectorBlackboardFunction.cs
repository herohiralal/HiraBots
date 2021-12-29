namespace UnityEngine
{
    public abstract class HiraBotsEffectorBlackboardFunction : HiraBotsBlackboardFunction
    {
        protected unsafe delegate void Delegate(in BlackboardComponent.LowLevel blackboard, byte* memory);

        #region Execution

        public void Execute(BlackboardComponent blackboard, bool expected)
        {
            try
            {
                ExecuteFunction(blackboard, expected);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        protected abstract void ExecuteFunction(BlackboardComponent blackboard, bool expected);

        #endregion
    }
}