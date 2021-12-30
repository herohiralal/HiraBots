#if UNITY_EDITOR || HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// This class is responsible for validating a blackboard template, and in the case of
    /// failure, extracting failure data to show to the user.
    /// </summary>
    internal class BlackboardTemplateValidator : IBlackboardTemplateValidatorContext
    {
        private readonly StringBuilder m_ErrorString;

        internal BlackboardTemplateValidator()
        {
            m_ErrorString = new StringBuilder(2000);
            m_CyclicalHierarchyCheckHelper = new HashSet<BlackboardTemplate>();
            m_SameNamedKeyCheckHelper = new HashSet<string>();
            m_EmptyIndices = new List<int>();
            m_DuplicateKeys = new List<(string, BlackboardTemplate)>();
            m_BadKeys = new List<BlackboardKey>();
        }

        private void Reset()
        {
            m_MissingBackends = BackendType.None;
            m_ErrorString.Clear();
            m_Validated = true;
            m_CyclicalHierarchyCheckHelper.Clear();
            m_SameNamedKeyCheckHelper.Clear();
            m_RecursionPoint = null;
            m_EmptyIndices.Clear();
            m_DuplicateKeys.Clear();
            m_BadKeys.Clear();
        }

        // the current status
        private bool m_Validated;

        // the backends missing in the parent
        private BackendType m_MissingBackends;

        // the recursion point
        private BlackboardTemplate m_RecursionPoint;

        // the indices of empty keys
        private readonly List<int> m_EmptyIndices;

        // the data about duplicated keys and which blackboard template they are from
        private readonly List<(string, BlackboardTemplate)> m_DuplicateKeys;

        // the keys that failed to compile
        private readonly List<BlackboardKey> m_BadKeys;

        // HELPER - cyclical key check helper
        private readonly HashSet<BlackboardTemplate> m_CyclicalHierarchyCheckHelper;

        // HELPER - same named key check helper
        private readonly HashSet<string> m_SameNamedKeyCheckHelper;

        /// <summary>
        /// Validate a blackboard template.
        /// </summary>
        internal bool Validate(BlackboardTemplate target, out string errorText)
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

            m_ErrorString.AppendLine($"Failed to validate blackboard template {target.name}.\n\n");

            if (m_MissingBackends != BackendType.None)
            {
                m_ErrorString.AppendLine(FormatErrorStringForUnsupportedBackends(m_MissingBackends));
            }

            if (m_RecursionPoint != null)
            {
                m_ErrorString.AppendLine(FormatErrorStringForRecursionPoint(m_RecursionPoint));
            }

            foreach (var index in m_EmptyIndices)
            {
                m_ErrorString.AppendLine(FormatErrorStringForEmptyIndex(index));
            }

            foreach (var (keyName, template) in m_DuplicateKeys)
            {
                m_ErrorString.AppendLine(FormatErrorStringForDuplicateKey(target, keyName, template));
            }

            foreach (var key in m_BadKeys)
            {
                m_ErrorString.AppendLine(FormatErrorStringForBadKey(key));
            }

            errorText = m_ErrorString.ToString();
            return false;
        }

        internal static string FormatErrorStringForUnsupportedBackends(BackendType missingBackends)
        {
            return $"Parent blackboard template does not support the following backends: {missingBackends}.";
        }

        /// <summary>
        /// Format an error string for a recursion point because of cyclical hierarchy in a blackboard template.
        /// </summary>
        internal static string FormatErrorStringForRecursionPoint(BlackboardTemplate rp)
        {
            return $"Contains cyclical hierarchy. Recursion point - {rp.name}.";
        }

        /// <summary>
        /// Format an error string for an empty index within a blackboard template.
        /// </summary>
        internal static string FormatErrorStringForEmptyIndex(int index)
        {
            return $"The key at index {index} is empty.";
        }

        /// <summary>
        /// Format an error string for a duplicate key in a blackboard template.
        /// </summary>
        internal static string FormatErrorStringForDuplicateKey(BlackboardTemplate target, string keyName, BlackboardTemplate template)
        {
            return $"Contains duplicate keys named {keyName}." +
                   (template == target ? "" : $"Inherited from {template.name}.");
        }

        /// <summary>
        /// Format an error string for a bad blackboard key.
        /// </summary>
        internal static string FormatErrorStringForBadKey(BlackboardKey key)
        {
            return $"Contains invalid data for the key {key.name}.";
        }

        //================================= validator context interface

        BackendType IBlackboardTemplateValidatorContext.missingBackends
        {
            get => m_MissingBackends;
            set => m_MissingBackends = value;
        }

        HashSet<BlackboardTemplate> IBlackboardTemplateValidatorContext.cyclicalHierarchyCheckHelper => m_CyclicalHierarchyCheckHelper;

        HashSet<string> IBlackboardTemplateValidatorContext.sameNamedKeyCheckHelper => m_SameNamedKeyCheckHelper;

        BlackboardTemplate IBlackboardTemplateValidatorContext.recursionPoint
        {
            get => m_RecursionPoint;
            set => m_RecursionPoint = value;
        }

        // other interface
        void IBlackboardTemplateValidatorContext.MarkUnsuccessful()
        {
            m_Validated = false;
        }

        void IBlackboardTemplateValidatorContext.AddEmptyKeyIndex(int index)
        {
            m_EmptyIndices.Add(index);
        }

        void IBlackboardTemplateValidatorContext.AddSameNamedKey(string keyName, BlackboardTemplate owner)
        {
            m_DuplicateKeys.Add((keyName, owner));
        }

        void IBlackboardTemplateValidatorContext.AddBadKey(BlackboardKey key)
        {
            m_BadKeys.Add(key);
        }
    }
}
#endif