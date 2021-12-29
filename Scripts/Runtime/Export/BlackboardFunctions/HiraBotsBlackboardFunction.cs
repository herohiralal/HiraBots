namespace UnityEngine
{
    public abstract unsafe class HiraBotsBlackboardFunction : ScriptableObject, HiraBots.ILowLevelObjectProvider
    {
        #region Compilation

        /// <summary>
        /// The aligned memory size required by this function.
        /// </summary>
        public int GetMemorySizeRequiredForCompilation() => m_MemorySize;

        // cached memory size
        protected int m_MemorySize = 0;

        /// <summary>
        /// Prepare the object for compilation, such as caching variables.
        /// </summary>
        public virtual void PrepareForCompilation()
        {
            // total size and function ptr
            m_MemorySize = sizeof(int) + sizeof(System.IntPtr);
        }

        /// <summary>
        /// Append the memory to the stream.
        /// </summary>
        public virtual void Compile(ref byte* stream)
        {
            *(int*) stream = GetMemorySizeRequiredForCompilation();
            stream += sizeof(int);

            *(System.IntPtr*) stream = functionPtr;
            stream += sizeof(System.IntPtr);
        }

        /// <summary>
        /// The pointer to the function.
        /// </summary>
        protected abstract System.IntPtr functionPtr { get; }

        protected static class CompilationRegistry
        {
            public static void IncreaseDepth()
            {
                HiraBots.CompilationRegistry.IncreaseDepth();
            }

            public static void DecreaseDepth()
            {
                HiraBots.CompilationRegistry.DecreaseDepth();
            }

            public static void AddEntry(string name, byte* startAddress, byte* endAddress)
            {
                HiraBots.CompilationRegistry.AddEntry(name, startAddress, endAddress);
            }
        }

        #endregion

#if UNITY_EDITOR
        #region Editor-Only Interface

        [SerializeField, HideInInspector] private string m_Subtitle = "";
        internal ref string subtitle => ref m_Subtitle;

        [SerializeField, HideInInspector] private string m_Description = "";
        internal string description => m_Description;

        internal void OnValidate()
        {
            try
            {
                OnValidateCallback();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

            try
            {
                UpdateDescription(out m_Description);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        internal void OnTargetBlackboardTemplateChangedWrapped(HiraBots.BlackboardTemplate template, ReadOnlyHashSetAccessor<HiraBots.BlackboardKey> keySet)
        {
            try
            {
                OnTargetBlackboardTemplateChanged(template, new BlackboardTemplate.KeySet(keySet));
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

            try
            {
                UpdateDescription(out m_Description);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Callback for when the target blackboard template changes.
        /// </summary>
        protected virtual void OnTargetBlackboardTemplateChanged(BlackboardTemplate template, in BlackboardTemplate.KeySet keySet)
        {
        }

        /// <summary>
        /// Callback for Unity's OnValidate message.
        /// </summary>
        protected virtual void OnValidateCallback()
        {
        }

        protected virtual void UpdateDescription(out string staticDescription)
        {
            staticDescription = "Customize this description by overriding UpdateDescription().";
        }

        #endregion
#endif

#if UNITY_EDITOR || HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
        #region Validation

        /// <summary>
        /// The context to validate a blackboard function.
        /// </summary>
        public struct ValidatorContext
        {
            /// <summary>
            /// The info regarding a bad key.
            /// </summary>
            public struct BadKeyInfo
            {
                /// <summary>
                /// The name of the blackboard function.
                /// </summary>
                public string functionName { get; set; }

                /// <summary>
                /// The name of the badly assigned variable.
                /// </summary>
                public string variableName { get; set; }

                /// <summary>
                /// The currently selected key.
                /// </summary>
                public BlackboardKey selectedKey { get; set; }
            }

            /// <summary>
            /// Whether the validation succeeded.
            /// </summary>
            public bool succeeded { get; set; }

            /// <summary>
            /// List of badly selected keys.
            /// </summary>
            public System.Collections.Generic.List<BadKeyInfo> badlySelectedKeys { get; set; }

            /// <summary>
            /// The pool of allowed keys.
            /// </summary>
            public BlackboardTemplate.KeySet allowedKeyPool { get; set; }
        }

        /// <summary>
        /// Validate this blackboard function.
        /// </summary>
        protected virtual void Validate(ref ValidatorContext context)
        {
        }

        protected void ValidateKeySelector(ref BlackboardTemplate.KeySelector selector,
            BlackboardKeyType typesFilter, ref ValidatorContext context,
            string keyName)
        {
            if (!selector.Validate(context.allowedKeyPool, typesFilter))
            {
                context.badlySelectedKeys.Add(new ValidatorContext.BadKeyInfo
                {
                    functionName = name,
                    variableName = keyName,
                    selectedKey = selector.selectedKey
                });

                context.succeeded = false;
            }
        }

        #endregion
#endif
    }
}