using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    public struct BaseBlackboard
    {
        static BaseBlackboard()
        {
            defaultBlackboard = new BaseBlackboard
            {
                m_X = 3
            };
        }
        
        public static BaseBlackboard defaultBlackboard { get; }
        public byte m_X;
    }
    
    public unsafe struct SomeBlackboard
    {
        static SomeBlackboard()
        {
            defaultBlackboard = new SomeBlackboard
            {
                m_ParentGeneratedBaseBlackboard = BaseBlackboard.defaultBlackboard,
                m_Something = 3,
            };
        }

        public static SomeBlackboard defaultBlackboard { get; }

        public BaseBlackboard m_ParentGeneratedBaseBlackboard;
        public int m_Something;
    }
    
    public partial class SmeBlackboardTemplate
    {
        private static readonly SmeBlackboardTemplate s_Instance = new SmeBlackboardTemplate();

        protected SmeBlackboardTemplate()
        {
        }
    }

    public partial class SmeBlackboardDerivedTemplate : SmeBlackboardTemplate
    {
        private static readonly SmeBlackboardDerivedTemplate s_Instance = new SmeBlackboardDerivedTemplate();

        protected SmeBlackboardDerivedTemplate()
        {
        }
    }

    public abstract partial class BaseBlackboardWrapper
    {
        protected abstract ref BaseBlackboard internalBaseBlackboard { get; }
    }

    public partial class BaseBlackboardWrapperActual : BaseBlackboardWrapper
    {
        private BaseBlackboard m_Internal = BaseBlackboard.defaultBlackboard;

        protected override ref BaseBlackboard internalBaseBlackboard => ref m_Internal;
    }

    public abstract partial class SomeBlackboardWrapper : BaseBlackboardWrapper
    {
        protected abstract ref SomeBlackboard internalSomeBlackboard { get; }

        public int Something
        {
            get => internalSomeBlackboard.m_Something;
            set => internalSomeBlackboard.m_Something = value;
        }
    }

    public partial class SomeBlackboardWrapperActual : SomeBlackboardWrapper
    {
        private SomeBlackboard m_Internal = SomeBlackboard.defaultBlackboard;
        
        protected override ref BaseBlackboard internalBaseBlackboard => ref m_Internal.m_ParentGeneratedBaseBlackboard;

        protected override ref SomeBlackboard internalSomeBlackboard => ref m_Internal;
    }
    
    /// <summary>
    /// The main entry point for code generation
    /// </summary>
    internal static class CodeGeneratorEntryPoint
    {
        [MenuItem("Assets/Generate HiraBots Code", false, priority = 800)]
        private static void GenerateHiraBotsCode()
        {
            var templateGenerationSuccessful = CookingHelpers.TryGetBlackboardTemplatesToGenerateCodeFor(out var blackboardTemplates);

            if (!templateGenerationSuccessful)
            {
                Debug.LogError("Failed to generate code.");
                return;
            }

            EditorSerializationUtility.ConfirmCodeGenFolder();
            EditorSerializationUtility.CreateCodeGenAssemblyDefinition();

            var generatedCode = new List<(string path, string contents)>();

            foreach (var (path, template) in blackboardTemplates)
            {
                generatedCode.Add((path, template.allGeneratedCode));
            }

            var generatedFiles = new string[generatedCode.Count];
            for (var i = 0; i < generatedCode.Count; i++)
            {
                var (path, contents) = generatedCode[i];
                generatedFiles[i] = path;

                EditorSerializationUtility.GenerateCode(path, contents);
            }

            EditorSerializationUtility.CleanupAndGenerateManifest(string.Join("\n", generatedFiles));

            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Generate HiraBots Code", true, priority = 800)]
        private static bool AllowCodeGeneration()
        {
            return !EditorApplication.isPlayingOrWillChangePlaymode;
        }
    }
}