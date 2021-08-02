using System.Collections.Generic;

namespace HiraBots.Editor
{
    internal class BlackboardTemplateValidatorContext : IBlackboardTemplateValidatorContext, IBlackboardKeyValidatorContext
    {
        internal void Reset()
        {
            m_Validated = true;
            cyclicalHierarchyCheckHelper.Clear();
            sameNamedKeyCheckHelper.Clear();
            recursionPoint = null;
            m_EmptyIndices.Clear();
            m_DuplicateKeys.Clear();
            m_BadKeys.Clear();
            m_CurrentKey = null;
        }

        internal bool m_Validated = true;

        // helpers
        public HashSet<BlackboardTemplate> cyclicalHierarchyCheckHelper { get; } = new HashSet<BlackboardTemplate>();
        public HashSet<string> sameNamedKeyCheckHelper { get; } = new HashSet<string>();

        // validation data
        public BlackboardTemplate recursionPoint { get; set; } = null;
        internal readonly List<int> m_EmptyIndices = new List<int>();
        internal readonly List<(string, BlackboardTemplate)> m_DuplicateKeys = new List<(string, BlackboardTemplate)>();
        internal readonly List<BlackboardKey> m_BadKeys = new List<BlackboardKey>();

        // state
        private BlackboardKey m_CurrentKey = null;

        // other interface
        void IBlackboardTemplateValidatorContext.MarkUnsuccessful() => m_Validated = false;
        public void AddEmptyKeyIndex(int index) => m_EmptyIndices.Add(index);
        public void AddSameNamedKey(string keyName, BlackboardTemplate owner) => m_DuplicateKeys.Add((keyName, owner));

        public IBlackboardKeyValidatorContext GetKeyValidatorContext(BlackboardKey key)
        {
            m_CurrentKey = key;
            return this;
        }

        void IBlackboardKeyValidatorContext.MarkUnsuccessful()
        {
            m_BadKeys.Add(m_CurrentKey);
            m_Validated = false;
        }
    }
}