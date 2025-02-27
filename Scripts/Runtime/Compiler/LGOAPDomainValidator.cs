﻿using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

namespace HiraBots
{
    internal class LGOAPDomainValidator : ILGOAPDomainValidatorContext
    {
        private readonly StringBuilder m_ErrorString;

        internal LGOAPDomainValidator(ReadOnlyDictionaryAccessor<BlackboardTemplate, ReadOnlyHashSetAccessor<BlackboardKey>> validatedBlackboardsAndTheirKeySets)
        {
            m_ErrorString = new StringBuilder(2000);
            m_BadContainers = new List<LGOAPDomainValidatorContext.BadContainerInfo>();
            m_BadFunctions = new List<LGOAPContainerValidatorContext.BadFunctionInfo>();
            m_BadExecutables = new List<LGOAPContainerValidatorContext.BadExecutableInfo>();
            m_BadlySelectedKeys = new List<HiraBotsBlackboardFunction.ValidatorContext.BadKeyInfo>();
            m_ExecutablesErrors = new List<string>();
            m_ValidatedBlackboardsAndTheirKeySets = validatedBlackboardsAndTheirKeySets;
        }

        private void Reset()
        {
            m_MissingBackends = BackendType.None;
            m_ErrorString.Clear();
            m_Validated = true;
            m_MissingBlackboard = false;
            m_UnvalidatedBlackboard = null;
            m_BadContainers.Clear();
            m_BadFunctions.Clear();
            m_BadExecutables.Clear();
            m_BadlySelectedKeys.Clear();
            m_ExecutablesErrors.Clear();
        }

        // the current status
        private bool m_Validated;

        // the backends missing in the blackboard
        private BackendType m_MissingBackends;

        // whether the blackboard is missing
        private bool m_MissingBlackboard;

        // whether the domain has an unvalidated blackboard
        private BlackboardTemplate m_UnvalidatedBlackboard;

        // list of bad containers within the domain
        private readonly List<LGOAPDomainValidatorContext.BadContainerInfo> m_BadContainers;

        // pre-allocated list of bad functions
        private readonly List<LGOAPContainerValidatorContext.BadFunctionInfo> m_BadFunctions;

        // pre-allocated list of bad executables
        private readonly List<LGOAPContainerValidatorContext.BadExecutableInfo> m_BadExecutables;

        // pre-allocated list of badly selected keys
        private readonly List<HiraBotsBlackboardFunction.ValidatorContext.BadKeyInfo> m_BadlySelectedKeys;

        // pre-allocated list of executables errors
        private readonly List<string> m_ExecutablesErrors;

        // hash map of validated blackboards and their key sets
        private readonly ReadOnlyDictionaryAccessor<BlackboardTemplate, ReadOnlyHashSetAccessor<BlackboardKey>> m_ValidatedBlackboardsAndTheirKeySets;

        internal bool Validate(LGOAPDomain target, out string errorText)
        {
            Reset();

            try
            {
                target.Validate(this);
            }
            catch (System.Exception e)
            {
                errorText = e.Message;
                Debug.LogException(e);
                return false;
            }

            if (m_Validated)
            {
                errorText = null;
                return true;
            }

            m_ErrorString.AppendLine($"Failed to validate LGOAP domain {target.name}.\n\n");

            if (m_MissingBlackboard)
            {
                m_ErrorString.AppendLine(FormatErrorStringForMissingBlackboard());
            }
            else if (m_MissingBackends != BackendType.None)
            {
                m_ErrorString.AppendLine(FormatErrorStringForUnsupportedBackends(m_MissingBackends));
            }
            else if (m_UnvalidatedBlackboard != null)
            {
                m_ErrorString.AppendLine(FormatErrorStringForUnvalidatedBlackboard(m_UnvalidatedBlackboard));
            }

            if (m_BadContainers != null)
            {
                for (var i = 0; i < m_BadContainers.Count; i++)
                {
                    var badContainerInfo = m_BadContainers[i];

                    if (badContainerInfo.containerIsNull)
                    {
                        m_ErrorString.AppendLine(FormatErrorStringForNullContainer(target,
                            ref badContainerInfo));
                        continue;
                    }

                    if (badContainerInfo.containerIsAbstractWhenItShouldNotBe)
                    {
                        m_ErrorString.AppendLine(FormatErrorStringForContainerAbstractWhenItShouldNotBe(target,
                            ref badContainerInfo));
                    }

                    if (badContainerInfo.badFunctions != null)
                    {
                        for (var j = 0; j < badContainerInfo.badFunctions.Length; j++)
                        {
                            var badFunctionInfo = badContainerInfo.badFunctions[j];

                            if (badFunctionInfo.functionIsNull)
                            {
                                m_ErrorString.AppendLine(FormatStringForNullFunction(target,
                                    ref badContainerInfo, ref badFunctionInfo));
                                continue;
                            }

                            if (badFunctionInfo.badKeys != null)
                            {
                                for (var k = 0; k < badFunctionInfo.badKeys.Length; k++)
                                {
                                    var badlySelectedKey = badFunctionInfo.badKeys[k];

                                    m_ErrorString.AppendLine(FormatErrorStringForBadlySelectedKey(target,
                                        ref badContainerInfo, ref badFunctionInfo, ref badlySelectedKey));
                                }
                            }
                        }
                    }

                    if (badContainerInfo.badExecutables != null)
                    {
                        for (var j = 0; j < badContainerInfo.badExecutables.Length; j++)
                        {
                            var badExecutableInfo = badContainerInfo.badExecutables[j];

                            if (badExecutableInfo.executableIsNull)
                            {
                                m_ErrorString.AppendLine(FormatStringForNullExecutable(target,
                                    ref badContainerInfo, ref badExecutableInfo));
                                continue;
                            }

                            if (badExecutableInfo.errors != null)
                            {
                                for (var k = 0; k < badExecutableInfo.errors.Length; k++)
                                {
                                    var executableError = badExecutableInfo.errors[k];

                                    m_ErrorString.AppendLine(FormatErrorStringForBadExecutableError(target,
                                        ref badContainerInfo, ref badExecutableInfo, executableError));
                                }
                            }
                        }
                    }
                }
            }

            errorText = m_ErrorString.ToString();
            return false;
        }

        internal static string FormatErrorStringForUnsupportedBackends(BackendType missingBackends)
        {
            return $"The assigned blackboard template does not support the following backends: {missingBackends}.";
        }

        internal static string FormatErrorStringForMissingBlackboard()
        {
            return "A blackboard template has not been assigned.";
        }

        internal static string FormatErrorStringForUnvalidatedBlackboard(BlackboardTemplate blackboard)
        {
            return $"The assigned blackboard template ({blackboard.name}) was invalidated.";
        }

        internal static string FormatErrorStringForNullContainer(LGOAPDomain d,
            ref LGOAPDomainValidatorContext.BadContainerInfo c)
        {
            return $"{d.name}::Layer[{c.layerIndex}]::{c.containerType}[{c.containerIndex}] is null.";
        }

        internal static string FormatErrorStringForContainerAbstractWhenItShouldNotBe(LGOAPDomain d,
            ref LGOAPDomainValidatorContext.BadContainerInfo c)
        {
            return $"{d.name}::Layer{c.layerIndex}]::{c.containerType}[{c.containerIndex}] should provide a task.";
        }

        internal static string FormatStringForNullFunction(LGOAPDomain d,
            ref LGOAPDomainValidatorContext.BadContainerInfo c,
            ref LGOAPContainerValidatorContext.BadFunctionInfo f)
        {
            return $"{d.name}::Layer[{c.layerIndex}]::{c.containerType}[{c.containerIndex}]({f.containerName})" +
                   $"::{f.functionType}[{f.functionIndex}] is null.";
        }

        internal static string FormatErrorStringForBadlySelectedKey(LGOAPDomain d,
            ref LGOAPDomainValidatorContext.BadContainerInfo c,
            ref LGOAPContainerValidatorContext.BadFunctionInfo f,
            ref HiraBotsBlackboardFunction.ValidatorContext.BadKeyInfo k)
        {
            return $"{d.name}::Layer[{c.layerIndex}]::{c.containerType}[{c.containerIndex}]({f.containerName})" +
                   $"::{f.functionType}[{f.functionIndex}]({k.functionName})::{k.variableName} has an invalid value of " +
                   $"{(k.selectedKey.isValid ? k.selectedKey.name : "null")}.";
        }

        internal static string FormatStringForNullExecutable(LGOAPDomain d,
            ref LGOAPDomainValidatorContext.BadContainerInfo c,
            ref LGOAPContainerValidatorContext.BadExecutableInfo e)
        {
            return $"{d.name}::Layer[{c.layerIndex}]::{c.containerType}[{c.containerIndex}]({e.containerName})" +
                   $"::{e.executableType}[{e.executableIndex}] is null.";
        }

        internal static string FormatErrorStringForBadExecutableError(LGOAPDomain d,
            ref LGOAPDomainValidatorContext.BadContainerInfo c,
            ref LGOAPContainerValidatorContext.BadExecutableInfo e,
            string error)
        {
            return $"{d.name}::Layer[{c.layerIndex}]::{c.containerType}[{c.containerIndex}]({e.containerName})" +
                   $"::{e.executableType}[{e.executableIndex}] reported \"{error}\"";
        }

        //================================= validator context interface

        BackendType ILGOAPDomainValidatorContext.missingBackends
        {
            get => m_MissingBackends;
            set => m_MissingBackends = value;
        }

        void ILGOAPDomainValidatorContext.AddBadContainer(ref LGOAPDomainValidatorContext.BadContainerInfo info)
        {
            m_BadContainers.Add(info);
        }

        List<LGOAPContainerValidatorContext.BadFunctionInfo> ILGOAPDomainValidatorContext.badFunctions => m_BadFunctions;

        List<LGOAPContainerValidatorContext.BadExecutableInfo> ILGOAPDomainValidatorContext.badExecutables => m_BadExecutables;

        List<HiraBotsBlackboardFunction.ValidatorContext.BadKeyInfo> ILGOAPDomainValidatorContext.badlySelectedKeys => m_BadlySelectedKeys;

        List<string> ILGOAPDomainValidatorContext.executablesErrors => m_ExecutablesErrors;

        // other interface
        void ILGOAPDomainValidatorContext.MarkUnsuccessful()
        {
            m_Validated = false;
        }

        void ILGOAPDomainValidatorContext.MarkMissingBlackboard()
        {
            m_MissingBlackboard = true;
        }

        void ILGOAPDomainValidatorContext.MarkUnvalidatedBlackboard(BlackboardTemplate b)
        {
            m_UnvalidatedBlackboard = b;
        }

        bool ILGOAPDomainValidatorContext.TryGetBlackboardKeySet(BlackboardTemplate b, out ReadOnlyHashSetAccessor<BlackboardKey> keys)
        {
            return m_ValidatedBlackboardsAndTheirKeySets.TryGetValue(b, out keys);
        }
    }
}