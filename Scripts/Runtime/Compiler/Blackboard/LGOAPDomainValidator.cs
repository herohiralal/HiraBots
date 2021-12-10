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
            m_BadObjects = new List<string>();
            m_ValidatedBlackboardsAndTheirKeySets = validatedBlackboardsAndTheirKeySets;
        }

        private void Reset()
        {
            m_MissingBackends = BackendType.None;
            m_ErrorString.Clear();
            m_Validated = true;
            m_MissingBlackboard = false;
            m_UnvalidatedBlackboard = null;
            m_BadObjects.Clear();
        }

        // the current status
        private bool m_Validated;

        // the backends missing in the blackboard
        private BackendType m_MissingBackends;

        // whether the blackboard is missing
        private bool m_MissingBlackboard;

        // whether the domain has an unvalidated blackboard
        private BlackboardTemplate m_UnvalidatedBlackboard;

        // list of bad objects within the domain
        private readonly List<string> m_BadObjects;

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

            if (m_MissingBackends != BackendType.None)
            {
                m_ErrorString.Append(FormatErrorStringForUnsupportedBackends(m_MissingBackends));
            }

            if (m_MissingBlackboard)
            {
                m_ErrorString.AppendLine(FormatErrorStringForMissingBlackboard());
            }

            if (m_UnvalidatedBlackboard != null)
            {
                m_ErrorString.AppendLine(FormatErrorStringForUnvalidatedBlackboard(m_UnvalidatedBlackboard));
            }

            if (m_BadObjects.Count > 0)
            {
                m_ErrorString.AppendLine("The domain contains one or more bad sub-objects:");

                foreach (var badObject in m_BadObjects)
                {
                    m_ErrorString.Append(badObject);
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

        //================================= validator context interface

        BackendType ILGOAPDomainValidatorContext.missingBackends
        {
            get => m_MissingBackends;
            set => m_MissingBackends = value;
        }

        List<string> ILGOAPDomainValidatorContext.badObjects => m_BadObjects;

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