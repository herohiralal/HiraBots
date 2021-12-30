using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HiraBots.Editor
{
    /// <summary>
    /// The main entry point for generating blackboard function code
    /// </summary>
    internal static class BlackboardFunctionGenerator
    {
        private const string k_CodeGenSubFolderName = "BlackboardFunctions";

        internal readonly struct BlackboardFunctionParameterInfo
        {
            internal enum Type : byte
            {
                UnmanagedValue = 0,
                BlackboardKey,
                Object,
                DynamicEnum
            }

            internal BlackboardFunctionParameterInfo(string name, UnityEngine.BlackboardKeyType keyType)
            {
                m_Name = name;
                m_Type = Type.BlackboardKey;
                m_KeyType = keyType;
                m_DynamicEnumKeyDependency = null;
                m_ObjectType = null;
            }

            internal BlackboardFunctionParameterInfo(string name, string dynamicEnumKeyDependency)
            {
                m_Name = name;
                m_Type = Type.DynamicEnum;
                m_KeyType = UnityEngine.BlackboardKeyType.Invalid;
                m_DynamicEnumKeyDependency = dynamicEnumKeyDependency;
                m_ObjectType = null;
            }

            internal BlackboardFunctionParameterInfo(string name, System.Type objectType)
            {
                m_Name = name;
                m_Type = UnsafeUtility.IsUnmanaged(objectType) ? Type.UnmanagedValue : Type.Object;
                m_KeyType = UnityEngine.BlackboardKeyType.Invalid;
                m_DynamicEnumKeyDependency = null;
                m_ObjectType = objectType;
            }

            internal readonly string m_Name;
            internal readonly Type m_Type;
            internal readonly UnityEngine.BlackboardKeyType m_KeyType;
            internal readonly string m_DynamicEnumKeyDependency;
            internal readonly System.Type m_ObjectType;
        }

        internal readonly struct BlackboardFunctionInfo
        {
            internal enum Type : byte
            {
                Decorator,
                ScoreCalculator,
                Effector
            }

            internal BlackboardFunctionInfo(Type type, string typeName, string name, string guid, BlackboardFunctionParameterInfo[] parameters = null)
            {
                m_Type = type;
                m_TypeName = typeName;
                m_Name = name;
                m_Guid = guid;
                m_Parameters = parameters ?? new BlackboardFunctionParameterInfo[0];
            }

            internal readonly Type m_Type;
            internal readonly string m_TypeName;
            internal readonly string m_Name;
            internal readonly string m_Guid;
            internal readonly BlackboardFunctionParameterInfo[] m_Parameters;
        }

        internal readonly struct InvalidatedBlackboardFunctionInfo
        {
            public InvalidatedBlackboardFunctionInfo(string typeName, string methodName, string guid, string baseClass)
            {
                m_TypeName = typeName;
                m_MethodName = methodName;
                m_Guid = guid;
                m_BaseClass = baseClass;
            }

            internal readonly string m_TypeName;
            internal readonly string m_MethodName;
            internal readonly string m_Guid;
            internal readonly string m_BaseClass;
        }

        /// <summary>
        /// Only allow code generation if in Edit mode.
        /// </summary>
        [MenuItem("Assets/Generate HiraBots Functions", true, priority = 800)]
        private static bool AllowCodeGeneration()
        {
            return !EditorApplication.isPlayingOrWillChangePlaymode;
        }

        /// <summary>
        /// Generate code for blackboard functions.
        /// </summary>
        [MenuItem("Assets/Generate HiraBots Functions", false, priority = 800)]
        private static void GenerateReflectionCode()
        {
            // create folder/asmdef
            EditorSerializationUtility.ConfirmCodeGenFolder();
            EditorSerializationUtility.CreateCodeGenAssemblyDefinition();

            var generatedCode = new List<(string path, string contents, string guid)>();

            var invalidatedMethods = new List<InvalidatedBlackboardFunctionInfo>();

            var functionInfos = new List<BlackboardFunctionInfo>();

            foreach (var decoratorWannabeFunction in TypeCache.GetMethodsWithAttribute<GenerateHiraBotsDecoratorAttribute>())
            {
                if (!ValidateMethodInfo(decoratorWannabeFunction,
                        typeof(bool),
                        out var paramInfos,
                        false))
                {
                    invalidatedMethods.Add(new InvalidatedBlackboardFunctionInfo(
                        $"{decoratorWannabeFunction.DeclaringType}",
                        decoratorWannabeFunction.Name,
                        decoratorWannabeFunction.GetCustomAttribute<GenerateHiraBotsDecoratorAttribute>().guid,
                        nameof(HiraBotsDecoratorBlackboardFunction)));
                    continue;
                }

                var functionInfo = new BlackboardFunctionInfo(
                    BlackboardFunctionInfo.Type.Decorator,
                    $"{decoratorWannabeFunction.DeclaringType}",
                    decoratorWannabeFunction.Name,
                    decoratorWannabeFunction.GetCustomAttribute<GenerateHiraBotsDecoratorAttribute>().guid,
                    paramInfos
                );

                functionInfos.Add(functionInfo);
            }

            foreach (var scoreCalculatorWannabeFunction in TypeCache.GetMethodsWithAttribute<GenerateHiraBotsScoreCalculatorAttribute>())
            {
                if (!ValidateMethodInfo(scoreCalculatorWannabeFunction,
                        typeof(float),
                        out var paramInfos, 
                        false,
                        typeof(float)))
                {
                    invalidatedMethods.Add(new InvalidatedBlackboardFunctionInfo(
                        $"{scoreCalculatorWannabeFunction.DeclaringType}",
                        scoreCalculatorWannabeFunction.Name,
                        scoreCalculatorWannabeFunction.GetCustomAttribute<GenerateHiraBotsScoreCalculatorAttribute>().guid,
                        nameof(HiraBotsScoreCalculatorBlackboardFunction)));
                    continue;
                }

                var functionInfo = new BlackboardFunctionInfo(
                    BlackboardFunctionInfo.Type.ScoreCalculator,
                    $"{scoreCalculatorWannabeFunction.DeclaringType}",
                    scoreCalculatorWannabeFunction.Name,
                    scoreCalculatorWannabeFunction.GetCustomAttribute<GenerateHiraBotsScoreCalculatorAttribute>().guid,
                    paramInfos
                );

                functionInfos.Add(functionInfo);
            }

            foreach (var effectorWannabeFunction in TypeCache.GetMethodsWithAttribute<GenerateHiraBotsEffectorAttribute>())
            {
                if (!ValidateMethodInfo(effectorWannabeFunction,
                        typeof(void),
                        out var paramInfos,
                        false))
                {
                    invalidatedMethods.Add(new InvalidatedBlackboardFunctionInfo(
                        $"{effectorWannabeFunction.DeclaringType}",
                        effectorWannabeFunction.Name,
                        effectorWannabeFunction.GetCustomAttribute<GenerateHiraBotsEffectorAttribute>().guid,
                        nameof(HiraBotsEffectorBlackboardFunction)));
                    continue;
                }

                var functionInfo = new BlackboardFunctionInfo(
                    BlackboardFunctionInfo.Type.Effector,
                    $"{effectorWannabeFunction.DeclaringType}",
                    effectorWannabeFunction.Name,
                    effectorWannabeFunction.GetCustomAttribute<GenerateHiraBotsEffectorAttribute>().guid,
                    paramInfos
                );

                functionInfos.Add(functionInfo);
            }

            foreach (var invalidatedMethod in invalidatedMethods)
            {
                var invalidFunctionTemplate = CodeGenHelpers.ReadTemplate("BlackboardFunctions/InvalidFunction",
                    ("<BLACKBOARD-FUNCTION-METHOD-NAME>", invalidatedMethod.m_MethodName),
                    ("<BLACKBOARD-FUNCTION-BASE-CLASS>", invalidatedMethod.m_BaseClass));

                generatedCode.Add(($"{k_CodeGenSubFolderName}/{invalidatedMethod.m_TypeName}/{invalidatedMethod.m_MethodName}.cs",
                    invalidFunctionTemplate, invalidatedMethod.m_Guid));
            }

            foreach (var functionInfo in functionInfos)
            {
                var code = GenerateCode(in functionInfo);

                generatedCode.Add(($"{k_CodeGenSubFolderName}/{functionInfo.m_TypeName}/{functionInfo.m_Name}.cs",
                    code, functionInfo.m_Guid));
            }

            // write all c# code
            var generatedFiles = new string[generatedCode.Count];
            for (var i = 0; i < generatedCode.Count; i++)
            {
                var (path, contents, guid) = generatedCode[i];
                generatedFiles[i] = path;

                EditorSerializationUtility.GenerateCode(path, contents, guid);
            }

            // generate manifest
            EditorSerializationUtility.CleanupAndGenerateManifest("hirabots_blackboard_functions", generatedFiles);

            // import new files
            AssetDatabase.Refresh();
        }

        internal static bool ValidateMethodInfo(MethodInfo wannabeFunction, Type expectedReturnType,
            out BlackboardFunctionParameterInfo[] paramInfos, bool skipPublicStaticCheck, params Type[] extraExpectedArgs)
        {
            paramInfos = null;

            var numberOfExtraArguments = extraExpectedArgs.Length;

            // public/static check
            if (!skipPublicStaticCheck && (!wannabeFunction.IsPublic || !wannabeFunction.IsStatic))
            {
                Debug.LogError(
                    $"{wannabeFunction.DeclaringType}.{wannabeFunction.Name} is not public/static.");
                return false;
            }

            // non-generic check
            if (wannabeFunction.IsGenericMethod)
            {
                Debug.LogError(
                    $"{wannabeFunction.DeclaringType}.{wannabeFunction.Name} is generic.");
                return false;
            }

            // return type check
            if (wannabeFunction.ReturnType != expectedReturnType)
            {
                Debug.LogError(
                    $"{wannabeFunction.DeclaringType}.{wannabeFunction.Name} does not have a valid return type: {expectedReturnType}.");
                return false;
            }

            var parameters = wannabeFunction.GetParameters();

            // check if it even has enough params
            if (parameters.Length < 2 + numberOfExtraArguments)
            {
                Debug.LogError(
                    $"{wannabeFunction.DeclaringType}.{wannabeFunction.Name} does not have enough arguments.");
                return false;
            }

            // first two args check
            if (parameters[0].ParameterType != typeof(UnityEngine.BlackboardComponent) || parameters[1].ParameterType != typeof(bool))
            {
                Debug.LogError(
                    $"{wannabeFunction.DeclaringType}.{wannabeFunction.Name} does not" +
                    $" have (BlackboardComponent blackboard) and (bool expected) as first two arguments.");
                return false;
            }

            // extra args check
            for (var i = 0; i < numberOfExtraArguments; i++)
            {
                if (extraExpectedArgs[i] != parameters[2 + i].ParameterType)
                {
                    Debug.LogError(
                        $"Argument {parameters[2 + i].Name} in {wannabeFunction.DeclaringType}.{wannabeFunction.Name} must" +
                        $" be of type {extraExpectedArgs[i]}.");
                    return false;
                }
            }

            paramInfos = new BlackboardFunctionParameterInfo[parameters.Length - 2 - numberOfExtraArguments];

            // individual args checks
            for (var i = 0; i < parameters.Length - 2 - numberOfExtraArguments; i++)
            {
                // start with the third arg
                var param = parameters[i + 2 + numberOfExtraArguments];
                var paramType = param.ParameterType;

                // dynamic enum
                var matchTypeToEnumKey = param.GetCustomAttribute<MatchTypeToEnumKeyAttribute>();
                if (matchTypeToEnumKey != null)
                {
                    // data type must be byte
                    if (paramType != typeof(byte))
                    {
                        Debug.LogError(
                            $"Argument {param.Name} in {wannabeFunction.DeclaringType}.{wannabeFunction.Name} is not a valid" +
                            $" dynamic enum argument because it is not a byte.");
                        return false;
                    }

                    // check if there's a valid argument before this index
                    var correctArgumentFound = false;
                    for (var j = 0; j < i; j++)
                    {
                        if (paramInfos[j].m_Name != matchTypeToEnumKey.argumentName || !paramInfos[j].m_KeyType.HasFlag(UnityEngine.BlackboardKeyType.Enum))
                        {
                            continue;
                        }

                        correctArgumentFound = true;
                        break;
                    }

                    if (!correctArgumentFound)
                    {
                        Debug.LogError(
                            $"Argument {param.Name} in {wannabeFunction.DeclaringType}.{wannabeFunction.Name} is not a valid" +
                            $" dynamic enum argument because argument {matchTypeToEnumKey.argumentName} could not be found or is" +
                            $"not a valid enum key argument.");
                        return false;
                    }

                    paramInfos[i] = new BlackboardFunctionParameterInfo(param.Name, matchTypeToEnumKey.argumentName);
                }
                else if (paramType == typeof(UnityEngine.BlackboardKey))
                {
                    var keyTypeAttribute = param.GetCustomAttribute<HiraBotsBlackboardKeyAttribute>();

                    if (keyTypeAttribute == null)
                    {
                        Debug.LogError(
                            $"Argument {param.Name} in {wannabeFunction.DeclaringType}.{wannabeFunction.Name} is not a valid" +
                            $" blackboard key argument because it does not provide [HiraBotsBlackboardKey] attribute.");
                        return false;
                    }

                    paramInfos[i] = new BlackboardFunctionParameterInfo(param.Name, keyTypeAttribute.keyType);
                }
                // object
                else if (typeof(Object).IsAssignableFrom(paramType))
                {
                    paramInfos[i] = new BlackboardFunctionParameterInfo(param.Name, paramType);
                }
                // unmanaged value
                else if (UnsafeUtility.IsUnmanaged(paramType))
                {
                    paramInfos[i] = new BlackboardFunctionParameterInfo(param.Name, paramType);
                }
                // unsupported stuff
                else
                {
                    Debug.LogError(
                        $"Argument {param.Name} in {wannabeFunction.DeclaringType}.{wannabeFunction.Name} is unsupported.");
                    return false;
                }
            }

            // unmanaged function finding
            {
                var declaringType = wannabeFunction.DeclaringType;

                // bs so rider doesn't throw warnings
                if (declaringType == null)
                {
                    Debug.LogError(
                        $"For some reason, the declaring type is null on {wannabeFunction.Name}.");
                    return false;
                }

                // find paired method
                var unmanagedMethod = declaringType.GetMethod($"{wannabeFunction.Name}Unmanaged",
                    (skipPublicStaticCheck
                        ? BindingFlags.Public | BindingFlags.NonPublic
                        : BindingFlags.Public)
                    | BindingFlags.Static);
                if (unmanagedMethod == null)
                {
                    Debug.LogError(
                        $"{declaringType}.{wannabeFunction.Name} does not have an unmanaged" +
                        $" paired function {wannabeFunction.Name}Unmanaged.");
                    return false;
                }

                var unmanagedParams = unmanagedMethod.GetParameters();

                // param min-length check
                if (unmanagedParams.Length < 1 + numberOfExtraArguments)
                {
                    Debug.LogError(
                        $"{declaringType}.{unmanagedMethod.Name} does not have enough parameters.");
                    return false;
                }

                // param length check
                if (paramInfos.Length + 1 + numberOfExtraArguments != unmanagedParams.Length)
                {
                    Debug.LogError(
                        $"{declaringType}.{unmanagedMethod.Name} does not the correct number of parameters.");
                    return false;
                }

                // first arg check
                if (unmanagedParams[0].ParameterType != typeof(UnityEngine.BlackboardComponent.LowLevel))
                {
                    Debug.LogError(
                        $"{declaringType}.{unmanagedMethod.Name} does not have the first argument as (BlackboardComponent.LowLevel).");
                    return false;
                }

                // extra args check
                for (var i = 0; i < numberOfExtraArguments; i++)
                {
                    if (extraExpectedArgs[i] != unmanagedParams[1 + i].ParameterType)
                    {
                        Debug.LogError(
                            $"Argument {unmanagedParams[1 + i].Name} in {declaringType}.{unmanagedMethod.Name} must" +
                            $" be of type {extraExpectedArgs[i]}.");
                        return false;
                    }
                }

                // individual arg matches
                for (var i = 0; i < unmanagedParams.Length - 1 - numberOfExtraArguments; i++)
                {
                    // which type it should match
                    var paramInfo = paramInfos[i];
                    var unmanagedParam = unmanagedParams[i + 1 + numberOfExtraArguments];
                    var supposedType = paramInfo.m_Type;
                    var actualType = unmanagedParam.ParameterType;

                    bool success;

                    switch (supposedType)
                    {
                        case BlackboardFunctionParameterInfo.Type.UnmanagedValue:
                            success = (actualType == paramInfo.m_ObjectType);
                            break;
                        case BlackboardFunctionParameterInfo.Type.BlackboardKey:
                            success = (actualType == typeof(UnityEngine.BlackboardKey.LowLevel));
                            break;
                        case BlackboardFunctionParameterInfo.Type.Object:
                            success = (actualType == typeof(int));
                            break;
                        case BlackboardFunctionParameterInfo.Type.DynamicEnum:
                            success = (actualType == typeof(byte));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (!success)
                    {
                        Debug.LogError(
                            $"Argument {unmanagedParam.Name} in {declaringType}.{unmanagedMethod.Name} could not be" +
                            $" paired with {paramInfo.m_Name} in {declaringType}.{wannabeFunction.Name}.");
                        return false;
                    }
                }
            }

            return true;
        }

        internal static string GenerateCode(in BlackboardFunctionInfo function)
        {
            string baseClass, returnType, extraParams, extraPassingParams;
            switch (function.m_Type)
            {
                case BlackboardFunctionInfo.Type.Decorator:
                    baseClass = nameof(HiraBotsDecoratorBlackboardFunction);
                    returnType = "bool";
                    extraParams = "";
                    extraPassingParams = "";
                    break;
                case BlackboardFunctionInfo.Type.ScoreCalculator:
                    baseClass = nameof(HiraBotsScoreCalculatorBlackboardFunction);
                    returnType = "float";
                    extraParams = ", float currentScore";
                    extraPassingParams = "currentScore";
                    break;
                case BlackboardFunctionInfo.Type.Effector:
                    baseClass = nameof(HiraBotsEffectorBlackboardFunction);
                    returnType = "void";
                    extraParams = "";
                    extraPassingParams = "";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var unmanagedFields = string.Join("", function
                .m_Parameters
                .Select(i =>
                    CodeGenHelpers.ReadTemplate("BlackboardFunctions/BlackboardFunctionUnmanagedField",
                        ("<BLACKBOARD-FUNCTION-PARAM-UNMANAGED-TYPE>", GetUnmanagedTypeName(in i)),
                        ("<BLACKBOARD-FUNCTION-PARAM-NAME>", $"_{i.m_Name}"))));

            var managedFields = string.Join("", function
                .m_Parameters
                .Select(i =>
                    CodeGenHelpers.ReadTemplate("BlackboardFunctions/BlackboardFunctionManagedField",
                        ("<BLACKBOARD-FUNCTION-PARAM-MANAGED-TYPE>", GetManagedTypeName(in i)),
                        ("<BLACKBOARD-FUNCTION-PARAM-NAME>", i.m_Name))));

            var unmanagedToManagedWithComma = string.Join(", ", function
                .m_Parameters
                .Select(i =>
                {
                    switch (i.m_Type)
                    {
                        case BlackboardFunctionParameterInfo.Type.UnmanagedValue:
                        case BlackboardFunctionParameterInfo.Type.DynamicEnum:
                            return $"_{i.m_Name} = {i.m_Name}";
                        case BlackboardFunctionParameterInfo.Type.BlackboardKey:
                            return $"_{i.m_Name} = new BlackboardKey.LowLevel({i.m_Name}.selectedKey)";
                        case BlackboardFunctionParameterInfo.Type.Object:
                            return $"_{i.m_Name} = {i.m_Name}.GetInstanceID()";
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }));

            var allUnmanagedFunctionParams = Enumerable
                .Empty<string>()
                .Append("blackboard");

            var allManagedFunctionParams = Enumerable
                .Empty<string>()
                .Append("blackboard, expected");

            if (!string.IsNullOrWhiteSpace(extraPassingParams))
            {
                allUnmanagedFunctionParams = allUnmanagedFunctionParams.Append(extraPassingParams);
                allManagedFunctionParams = allManagedFunctionParams.Append(extraPassingParams);
            }

            allUnmanagedFunctionParams = allUnmanagedFunctionParams.Concat(function
                .m_Parameters
                .Select(i => $"memory->_{i.m_Name}"));

            allManagedFunctionParams = allManagedFunctionParams.Concat(function
                .m_Parameters
                .Select(i =>
                {
                    switch (i.m_Type)
                    {
                        case BlackboardFunctionParameterInfo.Type.UnmanagedValue:
                        case BlackboardFunctionParameterInfo.Type.Object:
                        case BlackboardFunctionParameterInfo.Type.DynamicEnum:
                            return i.m_Name;
                        case BlackboardFunctionParameterInfo.Type.BlackboardKey:
                            return $"{i.m_Name}.selectedKey";
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }));

            var unmanagedFunctionCall = (returnType == "void" ? "" : "return ") +
                                        $"{function.m_TypeName}.{function.m_Name}Unmanaged({string.Join(", ", allUnmanagedFunctionParams)});";

            var managedFunctionCall = (returnType == "void" ? "" : "return ") +
                                      $"{function.m_TypeName}.{function.m_Name}({string.Join(", ", allManagedFunctionParams)});";

            var keyParams = function
                .m_Parameters
                .Where(i => i.m_Type == BlackboardFunctionParameterInfo.Type.BlackboardKey)
                .ToArray();

            var templateChangedCallback = string.Join(" ", keyParams
                .Select(i => $"{i.m_Name}.OnTargetBlackboardTemplateChanged(template, in keySet);"));

            return CodeGenHelpers.ReadTemplate("BlackboardFunctions/BlackboardFunction",
                ("<BLACKBOARD-FUNCTION-METHOD-NAME>", function.m_Name),
                ("<BLACKBOARD-FUNCTION-BASE-CLASS>", baseClass),
                ("<BLACKBOARD-FUNCTION-UNMANAGED-FIELDS>", unmanagedFields),
                ("<BLACKBOARD-FUNCTION-MANAGED-FIELDS>", managedFields),
                ("<BLACKBOARD-FUNCTION-MANAGED-TO-UNMANAGED-WITH-COMMA>", unmanagedToManagedWithComma),
                ("<BLACKBOARD-FUNCTION-RETURN-TYPE>", returnType),
                ("<BLACKBOARD-FUNCTION-EXTRA-PARAMS>", extraParams),
                ("<BLACKBOARD-FUNCTION-UNMANAGED-FUNCTION-CALL>", unmanagedFunctionCall),
                ("<BLACKBOARD-FUNCTION-MANAGED-FUNCTION-CALL>", managedFunctionCall),
                ("<BLACKBOARD-FUNCTION-TEMPLATE-CHANGED-CALLBACK>", templateChangedCallback)
            );
        }

        private static string GetUnmanagedTypeName(in BlackboardFunctionParameterInfo info)
        {
            switch (info.m_Type)
            {
                case BlackboardFunctionParameterInfo.Type.UnmanagedValue:
                    return info.m_ObjectType.FullName;
                case BlackboardFunctionParameterInfo.Type.BlackboardKey:
                    return $"{nameof(UnityEngine.BlackboardKey)}.{nameof(UnityEngine.BlackboardKey.LowLevel)}";
                case BlackboardFunctionParameterInfo.Type.Object:
                    return "int";
                case BlackboardFunctionParameterInfo.Type.DynamicEnum:
                    return "byte";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string GetManagedTypeName(in BlackboardFunctionParameterInfo info)
        {
            switch (info.m_Type)
            {
                case BlackboardFunctionParameterInfo.Type.UnmanagedValue:
                case BlackboardFunctionParameterInfo.Type.Object:
                    return info.m_ObjectType.FullName;
                case BlackboardFunctionParameterInfo.Type.BlackboardKey:
                    return $"{nameof(UnityEngine.BlackboardTemplate)}.{nameof(UnityEngine.BlackboardTemplate.KeySelector)}";
                case BlackboardFunctionParameterInfo.Type.DynamicEnum:
                    return nameof(UnityEngine.DynamicEnum);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}