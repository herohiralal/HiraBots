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
        // contains no extra header so no overridden memory size of compilation
    }
}