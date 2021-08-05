namespace HiraBots
{
    /// <summary>
    /// This class is responsible for compiling blackboard templates.
    /// </summary>
    internal class BlackboardTemplateCompiler : IBlackboardTemplateCompilerContext
    {
        private void Reset()
        {
        }

        /// <summary>
        /// Compile a blackbaord template.
        /// </summary>
        internal void Compile(BlackboardTemplate template)
        {
            Reset();

            template.Compile(this);
        }
    }
}