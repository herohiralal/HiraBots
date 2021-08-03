using System.Collections.Generic;

namespace HiraBots.Editor
{
    /// <summary>
    /// Context to validate a blackboard template. Only used inside the editor.
    /// </summary>
    internal class BlackboardTemplateValidatorContext : IBlackboardTemplateValidatorContext, IBlackboardKeyValidatorContext
    {
        // reset this object, for reuse
        internal void Reset()
        {
            validated = true;
            cyclicalHierarchyCheckHelper.Clear();
            sameNamedKeyCheckHelper.Clear();
            recursionPoint = null;
            emptyIndices.Clear();
            duplicateKeys.Clear();
            badKeys.Clear();
            m_CurrentKey = null;
        }

        // the current status
        internal bool validated { get; private set; } = true;

        // helpers
        private HashSet<BlackboardTemplate> cyclicalHierarchyCheckHelper { get; } = new HashSet<BlackboardTemplate>();
        HashSet<BlackboardTemplate> IBlackboardTemplateValidatorContext.cyclicalHierarchyCheckHelper => cyclicalHierarchyCheckHelper;

        private HashSet<string> sameNamedKeyCheckHelper { get; } = new HashSet<string>();
        HashSet<string> IBlackboardTemplateValidatorContext.sameNamedKeyCheckHelper => sameNamedKeyCheckHelper;

        // validation data
        internal BlackboardTemplate recursionPoint { get; private set; } = null;
        BlackboardTemplate IBlackboardTemplateValidatorContext.recursionPoint
        {
            get => recursionPoint;
            set => recursionPoint = value;
        }

        internal List<int> emptyIndices { get; } = new List<int>();
        internal List<(string, BlackboardTemplate)> duplicateKeys { get; } = new List<(string, BlackboardTemplate)>();
        internal List<BlackboardKey> badKeys { get; } = new List<BlackboardKey>();

        // state
        private BlackboardKey m_CurrentKey = null;

        // other interface
        void IBlackboardTemplateValidatorContext.MarkUnsuccessful()
        {
            validated = false;
        }

        void IBlackboardTemplateValidatorContext.AddEmptyKeyIndex(int index)
        {
            emptyIndices.Add(index);
        }

        void IBlackboardTemplateValidatorContext.AddSameNamedKey(string keyName, BlackboardTemplate owner)
        {
            duplicateKeys.Add((keyName, owner));
        }

        IBlackboardKeyValidatorContext IBlackboardTemplateValidatorContext.GetKeyValidatorContext(BlackboardKey key)
        {
            m_CurrentKey = key;
            return this;
        }

        void IBlackboardKeyValidatorContext.MarkUnsuccessful()
        {
            badKeys.Add(m_CurrentKey);
            validated = false;
        }
    }
}