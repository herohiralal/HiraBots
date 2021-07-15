using System.Collections.Generic;

namespace HiraBots.Editor
{
    internal class BlackboardTemplateValidatorContext : IBlackboardTemplateValidatorContext, IBlackboardKeyValidatorContext
    {
        public void Reset()
        {
            Validated = true;
            CyclicalHierarchyCheckHelper.Clear();
            SameNamedKeyCheckHelper.Clear();
            RecursionPoint = null;
            EmptyIndices.Clear();
            DuplicateKeys.Clear();
            BadKeys.Clear();
            _currentKey = null;
        }

        public bool Validated = true;

        // helpers
        public HashSet<BlackboardTemplate> CyclicalHierarchyCheckHelper { get; } = new HashSet<BlackboardTemplate>();
        public HashSet<string> SameNamedKeyCheckHelper { get; } = new HashSet<string>();

        // validation data
        public BlackboardTemplate RecursionPoint { get; set; } = null;
        public readonly List<int> EmptyIndices = new List<int>();
        public readonly List<(string, BlackboardTemplate)> DuplicateKeys = new List<(string, BlackboardTemplate)>();
        public readonly List<BlackboardKey> BadKeys = new List<BlackboardKey>();

        // state
        private BlackboardKey _currentKey = null;

        // other interface
        void IBlackboardTemplateValidatorContext.MarkUnsuccessful() => Validated = false;
        public void AddEmptyKeyIndex(int index) => EmptyIndices.Add(index);
        public void AddSameNamedKey(string keyName, BlackboardTemplate owner) => DuplicateKeys.Add((keyName, owner));

        public IBlackboardKeyValidatorContext GetKeyValidatorContext(BlackboardKey key)
        {
            _currentKey = key;
            return this;
        }

        void IBlackboardKeyValidatorContext.MarkUnsuccessful()
        {
            BadKeys.Add(_currentKey);
            Validated = false;
        }
    }
}