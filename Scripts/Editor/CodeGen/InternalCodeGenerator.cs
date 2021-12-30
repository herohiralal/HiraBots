#if HIRA_BOTS_CREATOR_MODE
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace HiraBots.Editor
{
    internal static class InternalCodeGenerator
    {
        [MenuItem("HiraBots/Regenerate Internal Code")]
        private static void Run()
        {
            // create folder/asmdef
            EditorSerializationUtility.ConfirmInternalCodeGenFolder();
            EditorSerializationUtility.CreateInternalCodeGenAssemblyDefinition();

            var generatedCode = new List<(string path, string contents, string guid)>();

            var functionInfos = new List<BlackboardFunctionGenerator.BlackboardFunctionInfo>();

            foreach (var methodInfo in typeof(SampleDecoratorBlackboardFunctions)
                         .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                         .Where(mi => mi.GetCustomAttribute<GenerateInternalBlackboardFunctionAttribute>() != null))
            {
                if (BlackboardFunctionGenerator.ValidateMethodInfo(methodInfo,
                        typeof(bool),
                        out var paramInfos, out var hasDescription, out var hasValidation,
                        true))
                {
                    var functionInfo = new BlackboardFunctionGenerator.BlackboardFunctionInfo(
                        BlackboardFunctionGenerator.BlackboardFunctionInfo.Type.Decorator,
                        $"{methodInfo.DeclaringType}",
                        methodInfo.Name,
                        methodInfo.GetCustomAttribute<GenerateInternalBlackboardFunctionAttribute>().guid,
                        hasDescription,
                        hasValidation,
                        paramInfos
                    );

                    functionInfos.Add(functionInfo);
                }
            }

            foreach (var methodInfo in typeof(SampleScoreCalculatorBlackboardFunctions)
                         .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                         .Where(mi => mi.GetCustomAttribute<GenerateInternalBlackboardFunctionAttribute>() != null))
            {
                if (BlackboardFunctionGenerator.ValidateMethodInfo(methodInfo,
                        typeof(float),
                        out var paramInfos, out var hasDescription, out var hasValidation,
                        true,
                        typeof(float)))
                {
                    var functionInfo = new BlackboardFunctionGenerator.BlackboardFunctionInfo(
                        BlackboardFunctionGenerator.BlackboardFunctionInfo.Type.ScoreCalculator,
                        $"{methodInfo.DeclaringType}",
                        methodInfo.Name,
                        methodInfo.GetCustomAttribute<GenerateInternalBlackboardFunctionAttribute>().guid,
                        hasDescription,
                        hasValidation,
                        paramInfos
                    );

                    functionInfos.Add(functionInfo);
                }
            }

            foreach (var methodInfo in typeof(SampleEffectorBlackboardFunctions)
                         .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                         .Where(mi => mi.GetCustomAttribute<GenerateInternalBlackboardFunctionAttribute>() != null))
            {
                if (BlackboardFunctionGenerator.ValidateMethodInfo(methodInfo,
                        typeof(void),
                        out var paramInfos, out var hasDescription, out var hasValidation,
                        true))
                {
                    var functionInfo = new BlackboardFunctionGenerator.BlackboardFunctionInfo(
                        BlackboardFunctionGenerator.BlackboardFunctionInfo.Type.Effector,
                        $"{methodInfo.DeclaringType}",
                        methodInfo.Name,
                        methodInfo.GetCustomAttribute<GenerateInternalBlackboardFunctionAttribute>().guid,
                        hasDescription,
                        hasValidation,
                        paramInfos
                    );

                    functionInfos.Add(functionInfo);
                }
            }

            foreach (var functionInfo in functionInfos)
            {
                var code = BlackboardFunctionGenerator.GenerateCode(in functionInfo);

                generatedCode.Add(($"BlackboardFunctions/{functionInfo.m_Name}.cs",
                    code, functionInfo.m_Guid));
            }

            // write all c# code
            var generatedFiles = new string[generatedCode.Count];
            for (var i = 0; i < generatedCode.Count; i++)
            {
                var (path, contents, guid) = generatedCode[i];
                generatedFiles[i] = path;

                EditorSerializationUtility.GenerateInternalCode(path, contents, guid);
            }

            // generate manifest
            EditorSerializationUtility.CleanupAndGenerateManifestForInternalCode("hirabots_internal_code", generatedFiles);

            // import new files
            AssetDatabase.Refresh();
        }
    }
}
#endif