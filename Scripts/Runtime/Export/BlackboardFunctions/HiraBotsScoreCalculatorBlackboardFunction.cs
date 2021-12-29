namespace UnityEngine
{
    public abstract class HiraBotsScoreCalculatorBlackboardFunction : HiraBotsBlackboardFunction
    {
        protected unsafe delegate float Delegate(in BlackboardComponent.LowLevel blackboard, byte* memory, float currentScore);

        #region Execution

        public float Execute(BlackboardComponent blackboard, bool expected, float currentScore)
        {
            try
            {
                return ExecuteFunction(blackboard, expected, currentScore);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return 0f;
            }
        }

        protected abstract float ExecuteFunction(BlackboardComponent blackboard, bool expected, float currentScore);

        #endregion
    }
}