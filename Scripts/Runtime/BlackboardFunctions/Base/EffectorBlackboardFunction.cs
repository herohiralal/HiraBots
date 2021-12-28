namespace HiraBots
{
    internal unsafe delegate void EffectorDelegate(in LowLevelBlackboard blackboard, byte* memory);

    /// <summary>
    /// A effector that can be executed on a <see cref="LowLevelBlackboard"/>.
    /// =====================================================================================================
    /// Any changes to this class MUST be synchronized with <see cref="LowLevelEffectorBlackboardFunction"/>.
    /// =====================================================================================================
    /// </summary>
    internal abstract partial class EffectorBlackboardFunction : BlackboardFunction<EffectorDelegate>
    {
        // non-VM execution
        internal void Execute(BlackboardComponent blackboard, bool expected)
        {
            try
            {
                ExecuteFunction(blackboard, expected);
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogException(e, this);
            }
        }

        protected abstract void ExecuteFunction(BlackboardComponent blackboard, bool expected);
    }
}