using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;

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

            internal BlackboardFunctionInfo(Type type, string typeName, string name, string guid, bool hasDescription, bool hasValidation, BlackboardFunctionParameterInfo[] parameters = null)
            {
                m_Type = type;
                m_TypeName = typeName;
                m_Name = name;
                m_Guid = guid;
                m_HasDescription = hasDescription;
                m_HasValidation = hasValidation;
                m_Parameters = parameters ?? new BlackboardFunctionParameterInfo[0];
            }

            internal readonly Type m_Type;
            internal readonly string m_TypeName;
            internal readonly string m_Name;
            internal readonly string m_Guid;
            internal readonly bool m_HasDescription;
            internal readonly bool m_HasValidation;
            internal readonly BlackboardFunctionParameterInfo[] m_Parameters;
        }

        private readonly struct InvalidatedBlackboardFunctionInfo
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
                        out var paramInfos, out var hasDescription, out var hasValidation,
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
                    hasDescription,
                    hasValidation,
                    paramInfos
                );

                functionInfos.Add(functionInfo);
            }

            foreach (var scoreCalculatorWannabeFunction in TypeCache.GetMethodsWithAttribute<GenerateHiraBotsScoreCalculatorAttribute>())
            {
                if (!ValidateMethodInfo(scoreCalculatorWannabeFunction,
                        typeof(float),
                        out var paramInfos, out var hasDescription, out var hasValidation,
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
                    hasDescription,
                    hasValidation,
                    paramInfos
                );

                functionInfos.Add(functionInfo);
            }

            foreach (var effectorWannabeFunction in TypeCache.GetMethodsWithAttribute<GenerateHiraBotsEffectorAttribute>())
            {
                if (!ValidateMethodInfo(effectorWannabeFunction,
                        typeof(void),
                        out var paramInfos, out var hasDescription, out var hasValidation,
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
                    hasDescription,
                    hasValidation,
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
            out BlackboardFunctionParameterInfo[] paramInfos, out bool hasDescription, out bool hasValidation,
            bool skipPublicStaticCheck, params Type[] extraExpectedArgs)
        {
            paramInfos = null;
            hasValidation = false;
            hasDescription = false;

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
            if (parameters.Length < numberOfExtraArguments)
            {
                Debug.LogError(
                    $"{wannabeFunction.DeclaringType}.{wannabeFunction.Name} does not have enough arguments.");
                return false;
            }

            // extra args check
            for (var i = 0; i < numberOfExtraArguments; i++)
            {
                if (extraExpectedArgs[i] != parameters[i].ParameterType)
                {
                    Debug.LogError(
                        $"Argument {parameters[i].Name} in {wannabeFunction.DeclaringType}.{wannabeFunction.Name} must" +
                        $" be of type {extraExpectedArgs[i]}.");
                    return false;
                }
            }

            paramInfos = new BlackboardFunctionParameterInfo[parameters.Length - numberOfExtraArguments];

            // individual args checks
            for (var i = 0; i < parameters.Length - numberOfExtraArguments; i++)
            {
                // start with the third arg
                var param = parameters[i + numberOfExtraArguments];
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
                // blackboard key
                else if (paramType.IsByRef)
                {
                    UnityEngine.BlackboardKeyType keyType;
                    if (paramType == typeof(bool).MakeByRefType())
                    {
                        keyType = UnityEngine.BlackboardKeyType.Boolean;
                    }
                    else if (paramType == typeof(byte).MakeByRefType())
                    {
                        keyType = UnityEngine.BlackboardKeyType.Enum;
                    }
                    else if (paramType == typeof(float).MakeByRefType())
                    {
                        keyType = UnityEngine.BlackboardKeyType.Float;
                    }
                    else if (paramType == typeof(int).MakeByRefType())
                    {
                        keyType = param.GetCustomAttribute<HiraBotsObjectKey>() == null
                            ? UnityEngine.BlackboardKeyType.Integer
                            : UnityEngine.BlackboardKeyType.Object;
                    }
                    else if (paramType == typeof(Unity.Mathematics.quaternion).MakeByRefType())
                    {
                        keyType = UnityEngine.BlackboardKeyType.Quaternion;
                    }
                    else if (paramType == typeof(Unity.Mathematics.float3).MakeByRefType())
                    {
                        keyType = UnityEngine.BlackboardKeyType.Vector;
                    }
                    else
                    {
                        Debug.LogError(
                            $"Argument {param.Name} in {wannabeFunction.DeclaringType}.{wannabeFunction.Name} is not a valid" +
                            " blackboard key argument because it doesn't use a supported type.");
                        return false;
                    }

                    paramInfos[i] = new BlackboardFunctionParameterInfo(param.Name, keyType);
                }
                // object
                else if (param.GetCustomAttribute<HiraBotsObjectValue>() != null)
                {
                    var attr = param.GetCustomAttribute<HiraBotsObjectValue>();
                    if (paramType != typeof(int))
                    {
                        Debug.LogError(
                            $"Argument {param.Name} in {wannabeFunction.DeclaringType}.{wannabeFunction.Name} is not a valid" +
                            $" Object value argument because it isn't an integer.");
                        return false;
                    }

                    paramInfos[i] = new BlackboardFunctionParameterInfo(param.Name, attr.objectType);
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

            var declaringType = wannabeFunction.DeclaringType;

            if (declaringType == null)
            {
                return true;
            }

            var updateDescriptionMethod = declaringType.GetMethod($"{wannabeFunction.Name}UpdateDescription",
                (skipPublicStaticCheck
                    ? BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static
                    : BindingFlags.Public | BindingFlags.Static));

            if (updateDescriptionMethod != null)
            {
                var updateDescParams = updateDescriptionMethod.GetParameters();

                if (updateDescParams.Length != paramInfos.Length + 1) // last one should be out string staticDescription
                {
                    Debug.LogError(
                        $"{declaringType}.{updateDescriptionMethod.Name} is not a valid description updater because it" +
                        $" doesn't have the correct number of arguments {paramInfos.Length + 1}.");
                    return false;
                }

                var lastParam = updateDescParams[paramInfos.Length];

                if (!lastParam.IsOut || lastParam.ParameterType != typeof(string).MakeByRefType())
                {
                    Debug.LogError($"{declaringType}.{updateDescriptionMethod.Name} is not a valid description updater because it" +
                                   $" doesn't have \"out string\" as the last argument.");
                    return false;
                }

                for (var i = 0; i < paramInfos.Length; i++)
                {
                    var updateDescParam = updateDescParams[i];
                    var paramInfo = paramInfos[i];
                    switch (paramInfo.m_Type)
                    {
                        case BlackboardFunctionParameterInfo.Type.UnmanagedValue:
                            if (updateDescParam.ParameterType != paramInfo.m_ObjectType)
                            {
                                Debug.LogError($"Argument {updateDescParam} in {declaringType}.{updateDescriptionMethod.Name} does not match the type it should.");
                                return false;
                            }

                            break;
                        case BlackboardFunctionParameterInfo.Type.BlackboardKey:
                            if (updateDescParam.ParameterType != typeof(UnityEngine.BlackboardTemplate.KeySelector))
                            {
                                Debug.LogError($"Argument {updateDescParam} in {declaringType}.{updateDescriptionMethod.Name} does not match the type it should.");
                                return false;
                            }

                            break;
                        case BlackboardFunctionParameterInfo.Type.Object:
                            if (updateDescParam.ParameterType != paramInfo.m_ObjectType)
                            {
                                Debug.LogError($"Argument {updateDescParam} in {declaringType}.{updateDescriptionMethod.Name} does not match the type it should.");
                                return false;
                            }

                            break;
                        case BlackboardFunctionParameterInfo.Type.DynamicEnum:
                            if (updateDescParam.ParameterType != typeof(UnityEngine.DynamicEnum))
                            {
                                Debug.LogError($"Argument {updateDescParam} in {declaringType}.{updateDescriptionMethod.Name} does not match the type it should.");
                                return false;
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                hasDescription = true;
            }

            var validateMethod = declaringType.GetMethod($"{wannabeFunction.Name}OnValidate",
                (skipPublicStaticCheck
                    ? BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static
                    : BindingFlags.Public | BindingFlags.Static));

            if (validateMethod != null)
            {
                var validateMethodParams = validateMethod.GetParameters();

                if (validateMethodParams.Length != paramInfos.Length)
                {
                    Debug.LogError(
                        $"{declaringType}.{validateMethod.Name} is not a valid validator because it" +
                        $" doesn't have the correct number of arguments {paramInfos.Length}.");
                    return false;
                }

                for (var i = 0; i < paramInfos.Length; i++)
                {
                    var validateParam = validateMethodParams[i];
                    var paramInfo = paramInfos[i];
                    switch (paramInfo.m_Type)
                    {
                        case BlackboardFunctionParameterInfo.Type.UnmanagedValue:
                            if (validateParam.ParameterType != paramInfo.m_ObjectType.MakeByRefType())
                            {
                                Debug.LogError($"Argument {validateParam} in {declaringType}.{validateMethod.Name} does not match the type it should.");
                                return false;
                            }

                            break;
                        case BlackboardFunctionParameterInfo.Type.BlackboardKey:
                            if (validateParam.ParameterType != typeof(UnityEngine.BlackboardTemplate.KeySelector).MakeByRefType())
                            {
                                Debug.LogError($"Argument {validateParam} in {declaringType}.{validateMethod.Name} does not match the type it should.");
                                return false;
                            }

                            break;
                        case BlackboardFunctionParameterInfo.Type.Object:
                            if (validateParam.ParameterType != paramInfo.m_ObjectType.MakeByRefType())
                            {
                                Debug.LogError($"Argument {validateParam} in {declaringType}.{validateMethod.Name} does not match the type it should.");
                                return false;
                            }

                            break;
                        case BlackboardFunctionParameterInfo.Type.DynamicEnum:
                            if (validateParam.ParameterType != typeof(UnityEngine.DynamicEnum).MakeByRefType())
                            {
                                Debug.LogError($"Argument {validateParam} in {declaringType}.{validateMethod.Name} does not match the type it should.");
                                return false;
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                hasValidation = true;
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
                            return $"_{i.m_Name} = GeneratedBlackboardHelpers.ObjectToInstanceID({i.m_Name})";
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }));

            var allUnmanagedFunctionParams = Enumerable
                .Empty<string>();

            var allManagedFunctionParams = Enumerable
                .Empty<string>();

            if (!string.IsNullOrWhiteSpace(extraPassingParams))
            {
                allUnmanagedFunctionParams = allUnmanagedFunctionParams.Append(extraPassingParams);
                allManagedFunctionParams = allManagedFunctionParams.Append(extraPassingParams);
            }

            allUnmanagedFunctionParams = allUnmanagedFunctionParams.Concat(function
                .m_Parameters
                .Select(i =>
                {
                    switch (i.m_Type)
                    {
                        case BlackboardFunctionParameterInfo.Type.UnmanagedValue:
                        case BlackboardFunctionParameterInfo.Type.Object:
                        case BlackboardFunctionParameterInfo.Type.DynamicEnum:
                            return $"memory->_{i.m_Name}";
                        case BlackboardFunctionParameterInfo.Type.BlackboardKey:
                            switch (i.m_KeyType)
                            {
                                case UnityEngine.BlackboardKeyType.Boolean:
                                    return $"ref blackboard.Access<bool>(memory->_{i.m_Name}.offset)";
                                case UnityEngine.BlackboardKeyType.Enum:
                                    return $"ref blackboard.Access<byte>(memory->_{i.m_Name}.offset)";
                                case UnityEngine.BlackboardKeyType.Float:
                                    return $"ref blackboard.Access<float>(memory->_{i.m_Name}.offset)";
                                case UnityEngine.BlackboardKeyType.Integer:
                                case UnityEngine.BlackboardKeyType.Object:
                                    return $"ref blackboard.Access<int>(memory->_{i.m_Name}.offset)";
                                case UnityEngine.BlackboardKeyType.Quaternion:
                                    return $"ref blackboard.Access<Unity.Mathematics.quaternion>(memory->_{i.m_Name}.offset)";
                                case UnityEngine.BlackboardKeyType.Vector:
                                    return $"ref blackboard.Access<Unity.Mathematics.float3>(memory->_{i.m_Name}.offset)";
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }));

            allManagedFunctionParams = allManagedFunctionParams.Concat(function
                .m_Parameters
                .Select(i =>
                {
                    switch (i.m_Type)
                    {
                        case BlackboardFunctionParameterInfo.Type.UnmanagedValue:
                        case BlackboardFunctionParameterInfo.Type.DynamicEnum:
                            return i.m_Name;
                        case BlackboardFunctionParameterInfo.Type.BlackboardKey:
                            return $"ref _{i.m_Name}";
                        case BlackboardFunctionParameterInfo.Type.Object:
                            return $"{i.m_Name}.GetInstanceID()";
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }));

            var unmanagedFunctionCall = (returnType == "void" ? "" : "return ") +
                                        $"{function.m_TypeName}.{function.m_Name}({string.Join(", ", allUnmanagedFunctionParams)});";

            var managedFunctionCall = (returnType == "void" ? "" : "var output = ") +
                                      $"{function.m_TypeName}.{function.m_Name}({string.Join(", ", allManagedFunctionParams)});";

            var keyParams = function
                .m_Parameters
                .Where(i => i.m_Type == BlackboardFunctionParameterInfo.Type.BlackboardKey)
                .ToArray();

            var managedFunctionCallKeyParamTempVars = string.Join("", keyParams.Select(i =>
            {
                switch (i.m_KeyType)
                {
                    case UnityEngine.BlackboardKeyType.Boolean:
                        return $"var _{i.m_Name} = blackboard.GetBooleanValue({i.m_Name}.selectedKey.name); ";
                    case UnityEngine.BlackboardKeyType.Enum:
                        return $"var _{i.m_Name} = blackboard.GetEnumValue({i.m_Name}.selectedKey.name); ";
                    case UnityEngine.BlackboardKeyType.Float:
                        return $"var _{i.m_Name} = blackboard.GetFloatValue({i.m_Name}.selectedKey.name); ";
                    case UnityEngine.BlackboardKeyType.Integer:
                        return $"var _{i.m_Name} = blackboard.GetIntegerValue({i.m_Name}.selectedKey.name); ";
                    case UnityEngine.BlackboardKeyType.Object:
                        return $"var _{i.m_Name} = blackboard.GetObjectValue({i.m_Name}.selectedKey.name).GetInstanceID(); ";
                    case UnityEngine.BlackboardKeyType.Quaternion:
                        return $"var _{i.m_Name} = blackboard.GetQuaternionValue({i.m_Name}.selectedKey.name); ";
                    case UnityEngine.BlackboardKeyType.Vector:
                        return $"var _{i.m_Name} = blackboard.GetVectorValue({i.m_Name}.selectedKey.name); ";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }));

            var managedFunctionCallKeyTempVarsClearUp = string.Join("", keyParams.Select(i =>
            {
                switch (i.m_KeyType)
                {
                    case UnityEngine.BlackboardKeyType.Boolean:
                        return $" blackboard.SetBooleanValue({i.m_Name}.selectedKey.name, _{i.m_Name}, expected);";
                    case UnityEngine.BlackboardKeyType.Enum:
                        return $" blackboard.SetEnumValue({i.m_Name}.selectedKey.name, _{i.m_Name}, expected);";
                    case UnityEngine.BlackboardKeyType.Float:
                        return $" blackboard.SetFloatValue({i.m_Name}.selectedKey.name, _{i.m_Name}, expected);";
                    case UnityEngine.BlackboardKeyType.Integer:
                        return $" blackboard.SetIntegerValue({i.m_Name}.selectedKey.name, _{i.m_Name}, expected);";
                    case UnityEngine.BlackboardKeyType.Object:
                        return $" blackboard.SetObjectValue({i.m_Name}.selectedKey.name, GeneratedBlackboardHelpers.InstanceIDToObject(_{i.m_Name}), expected);";
                    case UnityEngine.BlackboardKeyType.Quaternion:
                        return $" blackboard.SetQuaternionValue({i.m_Name}.selectedKey.name, _{i.m_Name}, expected);";
                    case UnityEngine.BlackboardKeyType.Vector:
                        return $" blackboard.SetVectorValue({i.m_Name}.selectedKey.name, _{i.m_Name}, expected);";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }));

            managedFunctionCall = managedFunctionCallKeyParamTempVars
                                  + managedFunctionCall
                                  + (function.m_Type == BlackboardFunctionInfo.Type.Effector
                                      ? managedFunctionCallKeyTempVarsClearUp
                                      : "")
                                  + (returnType == "void" ? "" : " return output;");

            var templateChangedCallback = string.Join(" ", keyParams
                .Select(i =>
                    CodeGenHelpers.ReadTemplate("BlackboardFunctions/BlackboardFunctionTemplateChangedCallback",
                        ("<BLACKBOARD-FUNCTION-PARAM-NAME>", i.m_Name))));

            var keySelectorValidators = string.Join("", keyParams
                .Select(i =>
                    CodeGenHelpers.ReadTemplate("BlackboardFunctions/BlackboardFunctionKeySelectorValidator",
                        ("<BLACKBOARD-FUNCTION-PARAM-NAME>", i.m_Name),
                        ("<BLACKBOARD-FUNCTION-PARAM-KEY-FILTER>", i.m_KeyType.ToCode()))));

            var keyFilterUpdates = string.Join("", keyParams
                .Select(i =>
                    CodeGenHelpers.ReadTemplate("BlackboardFunctions/BlackboardFunctionKeyTypeFilterUpdate",
                        ("<BLACKBOARD-FUNCTION-PARAM-NAME>", i.m_Name),
                        ("<BLACKBOARD-FUNCTION-PARAM-KEY-FILTER>", i.m_KeyType.ToCode()))));

            var enumTypeIdentifierUpdates = string.Join("", function
                .m_Parameters
                .Where(i => i.m_Type == BlackboardFunctionParameterInfo.Type.DynamicEnum)
                .Select(i =>
                    CodeGenHelpers.ReadTemplate("BlackboardFunctions/BlackboardFunctionEnumKeyTypeIdentifierFilterUpdate",
                        ("<BLACKBOARD-FUNCTION-PARAM-NAME>", i.m_Name),
                        ("<BLACKBOARD-FUNCTION-DEPENDENT-KEY-NAME>", i.m_DynamicEnumKeyDependency))));

            keyFilterUpdates += enumTypeIdentifierUpdates;

            string descriptionUpdater;
            if (function.m_HasDescription)
            {
                descriptionUpdater =
                    $"{function.m_TypeName}.{function.m_Name}UpdateDescription("
                    + string.Join(", ", function
                        .m_Parameters
                        .Select(i => i.m_Name))
                    + "out staticDescription);";
            }
            else
            {
                descriptionUpdater = "base.UpdateDescription(out staticDescription);";
            }

            string externalValidator;
            if (function.m_HasValidation)
            {
                externalValidator =
                    $"{function.m_TypeName}.{function.m_Name}OnValidate("
                    + string.Join(", ", function
                        .m_Parameters
                        .Select(i => $"ref {i.m_Name}"))
                    + ");";
            }
            else
            {
                externalValidator = "// no external validator";
            }

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
                ("<BLACKBOARD-FUNCTION-TEMPLATE-CHANGED-CALLBACK>", templateChangedCallback),
                ("<BLACKBOARD-FUNCTION-KEY-SELECTOR-FILTER-UPDATE>", keyFilterUpdates),
                ("<BLACKBOARD-FUNCTION-KEY-SELECTOR-VALIDATORS>", keySelectorValidators),
                ("<BLACKBOARD-FUNCTION-EXTERNAL-VALIDATOR>", externalValidator),
                ("<BLACKBOARD-FUNCTION-EXTERNAL-DESCRIPTION-UPDATER>", descriptionUpdater)
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

        private static string ToCode(this UnityEngine.BlackboardKeyType keyType)
        {
            var output = $"{typeof(UnityEngine.BlackboardKeyType)}.{UnityEngine.BlackboardKeyType.Invalid}";

            string Check(UnityEngine.BlackboardKeyType flag)
            {
                // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
                return (keyType & flag) != UnityEngine.BlackboardKeyType.Invalid
                    ? $" | {typeof(UnityEngine.BlackboardKeyType)}.{flag}"
                    : "";
            }

            output += Check((UnityEngine.BlackboardKeyType) (1 << 0));
            output += Check((UnityEngine.BlackboardKeyType) (1 << 1));
            output += Check((UnityEngine.BlackboardKeyType) (1 << 2));
            output += Check((UnityEngine.BlackboardKeyType) (1 << 3));
            output += Check((UnityEngine.BlackboardKeyType) (1 << 4));
            output += Check((UnityEngine.BlackboardKeyType) (1 << 5));
            output += Check((UnityEngine.BlackboardKeyType) (1 << 6));

            return output;
        }
    }
}