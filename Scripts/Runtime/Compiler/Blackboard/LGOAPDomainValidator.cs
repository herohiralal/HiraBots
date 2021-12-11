using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HiraBots
{
    internal class LGOAPDomainValidator : ILGOAPDomainValidatorContext
    {
        private readonly StringBuilder m_ErrorString;

        internal LGOAPDomainValidator(ReadOnlyDictionaryAccessor<BlackboardTemplate, ReadOnlyHashSetAccessor<BlackboardKey>> validatedBlackboardsAndTheirKeySets)
        {
            m_ErrorString = new StringBuilder(2000);
            m_BadComponents = new List<LGOAPDomainValidatorContext.BadComponentInfo>();
            m_BadFunctions = new List<LGOAPComponentValidatorContext.BadFunctionInfo>();
            m_BadlySelectedKeys = new List<BlackboardFunctionValidatorContext.BadKeyInfo>();
            m_ValidatedBlackboardsAndTheirKeySets = validatedBlackboardsAndTheirKeySets;
        }

        private void Reset()
        {
            m_MissingBackends = BackendType.None;
            m_ErrorString.Clear();
            m_Validated = true;
            m_MissingBlackboard = false;
            m_UnvalidatedBlackboard = null;
            m_BadComponents.Clear();
            m_BadFunctions.Clear();
            m_BadlySelectedKeys.Clear();
        }

        // the current status
        private bool m_Validated;

        // the backends missing in the blackboard
        private BackendType m_MissingBackends;

        // whether the blackboard is missing
        private bool m_MissingBlackboard;

        // whether the domain has an unvalidated blackboard
        private BlackboardTemplate m_UnvalidatedBlackboard;

        // list of bad components within the domain
        private readonly List<LGOAPDomainValidatorContext.BadComponentInfo> m_BadComponents;

        // pre-allocated list of bad functions
        private readonly List<LGOAPComponentValidatorContext.BadFunctionInfo> m_BadFunctions;

        // pre-allocated list of badly selected keys
        private readonly List<BlackboardFunctionValidatorContext.BadKeyInfo> m_BadlySelectedKeys;

        // hash map of validated blackboards and their key sets
        private readonly ReadOnlyDictionaryAccessor<BlackboardTemplate, ReadOnlyHashSetAccessor<BlackboardKey>> m_ValidatedBlackboardsAndTheirKeySets;

        internal bool Validate(LGOAPDomain target, out string errorText)
        {
            Reset();

            target.Validate(this);

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

            for (var i = 0; i < m_BadComponents.Count; i++)
            {
                var badComponentInfo = m_BadComponents[i];

                if (badComponentInfo.componentIsNull)
                {
                    m_ErrorString.AppendLine(FormatErrorStringForNullComponent(target,
                        ref badComponentInfo));
                    continue;
                }

                if (badComponentInfo.componentIsAbstractWhenItShouldNotBe)
                {
                    m_ErrorString.AppendLine(FormatErrorStringForComponentAbstractWhenItShouldNotBe(target,
                        ref badComponentInfo));
                }

                if (badComponentInfo.componentIsNotAbstractWhenItShouldBe)
                {
                    m_ErrorString.AppendLine(FormatErrorStringForComponentNotAbstractWhenItShouldBe(target,
                        ref badComponentInfo));
                }

                for (var j = 0; j < badComponentInfo.badFunctions.Length; j++)
                {
                    var badFunctionInfo = badComponentInfo.badFunctions[j];

                    if (badFunctionInfo.functionIsNull)
                    {
                        m_ErrorString.AppendLine(FormatStringForNullFunction(target,
                            ref badComponentInfo, ref badFunctionInfo));
                        continue;
                    }

                    if (badFunctionInfo.functionIsScoreCalculatorWhenItShouldNotBe)
                    {
                        m_ErrorString.AppendLine(FormatErrorStringForFunctionScoreCalculatorWhenItShouldNotBe(target,
                            ref badComponentInfo, ref badFunctionInfo));
                    }

                    if (badFunctionInfo.functionIsNotScoreCalculatorWhenItShouldBe)
                    {
                        m_ErrorString.AppendLine(FormatErrorStringForFunctionNotScoreCalculatorWhenItShouldBe(target,
                            ref badComponentInfo, ref badFunctionInfo));
                    }

                    for (var k = 0; k < badFunctionInfo.badKeys.Length; k++)
                    {
                        var badlySelectedKey = badFunctionInfo.badKeys[k];
                        m_ErrorString.AppendLine(FormatErrorStringForBadlySelectedKey(target,
                            ref badComponentInfo, ref badFunctionInfo, ref badlySelectedKey));
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

        internal static string FormatErrorStringForNullComponent(LGOAPDomain d,
            ref LGOAPDomainValidatorContext.BadComponentInfo c)
        {
            return $"{d.name}::Layer[{c.layerIndex}]::{c.componentType}[{c.componentIndex}] is null.";
        }

        internal static string FormatErrorStringForComponentAbstractWhenItShouldNotBe(LGOAPDomain d,
            ref LGOAPDomainValidatorContext.BadComponentInfo c)
        {
            return $"{d.name}::Layer{c.layerIndex}]::{c.componentType}[{c.componentIndex}] should not be abstract.";
        }

        internal static string FormatErrorStringForComponentNotAbstractWhenItShouldBe(LGOAPDomain d,
            ref LGOAPDomainValidatorContext.BadComponentInfo c)
        {
            return $"{d.name}::Layer{c.layerIndex}]::{c.componentType}[{c.componentIndex}] should be abstract.";
        }

        internal static string FormatStringForNullFunction(LGOAPDomain d,
            ref LGOAPDomainValidatorContext.BadComponentInfo c,
            ref LGOAPComponentValidatorContext.BadFunctionInfo f)
        {
            return $"{d.name}::Layer[{c.layerIndex}]::{c.componentType}[{c.componentIndex}]({f.componentName})" +
                   $"::{f.functionType}[{f.functionIndex}] is null.";
        }

        internal static string FormatErrorStringForFunctionScoreCalculatorWhenItShouldNotBe(LGOAPDomain d,
            ref LGOAPDomainValidatorContext.BadComponentInfo c,
            ref LGOAPComponentValidatorContext.BadFunctionInfo f)
        {
            return $"{d.name}::Layer[{c.layerIndex}]::{c.componentType}[{c.componentIndex}]({f.componentName})" +
                   $"::{f.functionType}[{f.functionIndex}] should not be a score calculator.";
        }

        internal static string FormatErrorStringForFunctionNotScoreCalculatorWhenItShouldBe(LGOAPDomain d,
            ref LGOAPDomainValidatorContext.BadComponentInfo c,
            ref LGOAPComponentValidatorContext.BadFunctionInfo f)
        {
            return $"{d.name}::Layer[{c.layerIndex}]::{c.componentType}[{c.componentIndex}]({f.componentName})" +
                   $"::{f.functionType}[{f.functionIndex}] should be a score calculator.";
        }

        internal static string FormatErrorStringForBadlySelectedKey(LGOAPDomain d,
            ref LGOAPDomainValidatorContext.BadComponentInfo c,
            ref LGOAPComponentValidatorContext.BadFunctionInfo f,
            ref BlackboardFunctionValidatorContext.BadKeyInfo k)
        {
            return $"{d.name}::Layer[{c.layerIndex}]::{c.componentType}[{c.componentIndex}]({f.componentName})" +
                   $"::{f.functionType}[{f.functionIndex}]({k.functionName})::{k.variableName} has an invalid value of " +
                   $"{(k.selectedKey == null ? "null" : k.selectedKey.name)}.";
        }

        //================================= validator context interface

        BackendType ILGOAPDomainValidatorContext.missingBackends
        {
            get => m_MissingBackends;
            set => m_MissingBackends = value;
        }

        void ILGOAPDomainValidatorContext.AddBadComponent(ref LGOAPDomainValidatorContext.BadComponentInfo info)
        {
            m_BadComponents.Add(info);
        }

        List<LGOAPComponentValidatorContext.BadFunctionInfo> ILGOAPDomainValidatorContext.badFunctions => m_BadFunctions;

        List<BlackboardFunctionValidatorContext.BadKeyInfo> ILGOAPDomainValidatorContext.badlySelectedKeys => m_BadlySelectedKeys;

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