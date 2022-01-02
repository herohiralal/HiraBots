namespace HiraBots
{
    internal partial class ExecutorComponent
    {
        private static ulong s_Id = 0;

        private readonly ulong m_Id;

        /// <summary>
        /// Reset the static id assigner.
        /// </summary>
        internal static void ResetStaticIDAssigner()
        {
            s_Id = 0;
        }

        internal static bool TryCreate(out ExecutorComponent executor)
        {
            executor = new ExecutorComponent();
            return true;
        }

        private ExecutorComponent()
        {
            m_Id = ++s_Id;
        }

        internal void Dispose()
        {
        }
    }
}