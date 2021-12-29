namespace UnityEngine
{
    public abstract class HiraBotsDecoratorBlackboardFunction : HiraBotsBlackboardFunction
    {
        protected unsafe delegate bool Delegate(in BlackboardComponent.LowLevel blackboard, byte* memory);

        [SerializeField] private bool m_Invert = false;

        public ref bool invert => ref m_Invert;

        #region Compilation

        public override void PrepareForCompilation()
        {
            base.PrepareForCompilation();
            m_MemorySize += sizeof(bool);
        }

        public override unsafe void Compile(ref byte* stream)
        {
            base.Compile(ref stream);

            *(bool*) stream = m_Invert;
            stream += sizeof(bool);
        }

        #endregion

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

        protected abstract bool ExecuteFunction(BlackboardComponent blackboard, bool expected);

        #endregion
    }
}