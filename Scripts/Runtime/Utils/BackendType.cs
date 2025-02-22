namespace HiraBots
{
    /// <summary>
    /// The type of backend to use for the functioning of HiraBot components.
    /// </summary>
    [System.Serializable]
    internal enum BackendType : byte
    {
        [UnityEngine.Tooltip("No backend. Will be ignored by both code generator and runtime compiler.")]
        None = 0,

        [UnityEngine.Tooltip("Will generate bytecode at runtime.")]
        RuntimeInterpreter = 1 << 0,

        [UnityEngine.Tooltip("Will generate C# code.")]
        CodeGenerator = 1 << 1,

        [UnityEngine.Tooltip("Visible to both code generator and runtime compiler.")]
        Both = RuntimeInterpreter | CodeGenerator
    }
}