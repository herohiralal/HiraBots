using System;
using System.Collections.Generic;
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
        private readonly struct BlackboardFunctionParameterInfo
        {
            internal enum Type : byte
            {
                UnmanagedValue = 0,
                BooleanKey,
                EnumKey,
                FloatKey,
                IntegerKey,
                ObjectKey,
                QuaternionKey,
                VectorKey,
                Object,
                DynamicEnum
            }

            internal BlackboardFunctionParameterInfo(string name, Type type, string dynamicEnumKeyDependency = null)
            {
                m_Name = name;
                m_Type = type;
                m_DynamicEnumKeyDependency = dynamicEnumKeyDependency;
                m_ObjectType = null;
            }

            internal BlackboardFunctionParameterInfo(string name, Type type, System.Type objectType)
            {
                m_Name = name;
                m_Type = type;
                m_DynamicEnumKeyDependency = null;
                m_ObjectType = objectType;
            }

            internal readonly string m_Name;
            internal readonly Type m_Type;
            internal readonly string m_DynamicEnumKeyDependency;
            internal readonly System.Type m_ObjectType;
        }

        private readonly struct BlackboardFunctionInfo
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
            
            var generatedCode = new List<(string path, string contents)>();

            var invalidatedMethods = new List<(string typeName, string methodName)>();

            var functionInfos = new List<BlackboardFunctionInfo>();

            foreach (var decoratorWannabeFunction in TypeCache.GetMethodsWithAttribute<GenerateHiraBotsDecoratorAttribute>())
            {
                if (!ValidateMethodInfo(decoratorWannabeFunction, typeof(bool), out var paramInfos))
                {
                    invalidatedMethods.Add(($"{decoratorWannabeFunction.DeclaringType}", decoratorWannabeFunction.Name));
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
                if (!ValidateMethodInfo(scoreCalculatorWannabeFunction, typeof(float), out var paramInfos, typeof(float)))
                {
                    invalidatedMethods.Add(($"{scoreCalculatorWannabeFunction.DeclaringType}", scoreCalculatorWannabeFunction.Name));
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
                if (!ValidateMethodInfo(effectorWannabeFunction, typeof(void), out var paramInfos))
                {
                    invalidatedMethods.Add(($"{effectorWannabeFunction.DeclaringType}", effectorWannabeFunction.Name));
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

            // write all c# code
            var generatedFiles = new string[generatedCode.Count];
            for (var i = 0; i < generatedCode.Count; i++)
            {
                var (path, contents) = generatedCode[i];
                generatedFiles[i] = path;
            
                EditorSerializationUtility.GenerateCode(path, contents);
            }
            
            // generate manifest
            EditorSerializationUtility.CleanupAndGenerateManifest("hirabots_blackboard_functions", generatedFiles);
            
            // import new files
            AssetDatabase.Refresh();
        }

        private static bool ValidateMethodInfo(MethodInfo wannabeFunction, Type expectedReturnType,
            out BlackboardFunctionParameterInfo[] paramInfos, params Type[] extraExpectedArgs)
        {
            paramInfos = null;

            var numberOfExtraArguments = extraExpectedArgs.Length;

            // public/static check
            if (!wannabeFunction.IsPublic || !wannabeFunction.IsStatic)
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
                        if (paramInfos[j].m_Name != matchTypeToEnumKey.argumentName || paramInfos[j].m_Type != BlackboardFunctionParameterInfo.Type.EnumKey)
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

                    paramInfos[i] = new BlackboardFunctionParameterInfo(param.Name, BlackboardFunctionParameterInfo.Type.DynamicEnum,
                        matchTypeToEnumKey.argumentName);
                }
                // bool key
                else if (paramType == typeof(string) && param.GetCustomAttribute<HiraBotsBooleanKeyAttribute>() != null)
                {
                    paramInfos[i] = new BlackboardFunctionParameterInfo(param.Name, BlackboardFunctionParameterInfo.Type.BooleanKey);
                }
                // enum key
                else if (paramType == typeof(string) && param.GetCustomAttribute<HiraBotsEnumKeyAttribute>() != null)
                {
                    paramInfos[i] = new BlackboardFunctionParameterInfo(param.Name, BlackboardFunctionParameterInfo.Type.EnumKey);
                }
                // float key
                else if (paramType == typeof(string) && param.GetCustomAttribute<HiraBotsFloatKeyAttribute>() != null)
                {
                    paramInfos[i] = new BlackboardFunctionParameterInfo(param.Name, BlackboardFunctionParameterInfo.Type.FloatKey);
                }
                // int key
                else if (paramType == typeof(string) && param.GetCustomAttribute<HiraBotsIntegerKeyAttribute>() != null)
                {
                    paramInfos[i] = new BlackboardFunctionParameterInfo(param.Name, BlackboardFunctionParameterInfo.Type.IntegerKey);
                }
                // object key
                else if (paramType == typeof(string) && param.GetCustomAttribute<HiraBotsObjectKeyAttribute>() != null)
                {
                    paramInfos[i] = new BlackboardFunctionParameterInfo(param.Name, BlackboardFunctionParameterInfo.Type.ObjectKey);
                }
                // quaternion key
                else if (paramType == typeof(string) && param.GetCustomAttribute<HiraBotsQuaternionKeyAttribute>() != null)
                {
                    paramInfos[i] = new BlackboardFunctionParameterInfo(param.Name, BlackboardFunctionParameterInfo.Type.QuaternionKey);
                }
                // vector key
                else if (paramType == typeof(string) && param.GetCustomAttribute<HiraBotsVectorKeyAttribute>() != null)
                {
                    paramInfos[i] = new BlackboardFunctionParameterInfo(param.Name, BlackboardFunctionParameterInfo.Type.VectorKey);
                }
                // object
                else if (typeof(Object).IsAssignableFrom(paramType))
                {
                    paramInfos[i] = new BlackboardFunctionParameterInfo(param.Name, BlackboardFunctionParameterInfo.Type.Object, paramType);
                }
                // unmanaged value
                else if (UnsafeUtility.IsUnmanaged(paramType))
                {
                    paramInfos[i] = new BlackboardFunctionParameterInfo(param.Name, BlackboardFunctionParameterInfo.Type.UnmanagedValue, paramType);
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
                var unmanagedMethod = declaringType.GetMethod($"{wannabeFunction.Name}Unmanaged", BindingFlags.Public | BindingFlags.Static);
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
                        case BlackboardFunctionParameterInfo.Type.BooleanKey:
                        case BlackboardFunctionParameterInfo.Type.EnumKey:
                        case BlackboardFunctionParameterInfo.Type.FloatKey:
                        case BlackboardFunctionParameterInfo.Type.IntegerKey:
                        case BlackboardFunctionParameterInfo.Type.ObjectKey:
                        case BlackboardFunctionParameterInfo.Type.QuaternionKey:
                        case BlackboardFunctionParameterInfo.Type.VectorKey:
                            success = (actualType == typeof(ushort));
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
    }
}