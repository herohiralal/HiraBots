#if UNITY_EDITOR // ideally validation is only needed within the editor (either when building, or when exiting play mode
using System.Collections.Generic;

namespace HiraBots
{
    internal interface IBlackboardTemplateValidatorContext
    {
        void MarkUnsuccessful();
        
        HashSet<BlackboardTemplate> CyclicalHierarchyCheckHelper { get; }
        BlackboardTemplate RecursionPoint { get; set; }

        void AddEmptyKeyIndex(int index);

        HashSet<string> SameNamedKeyCheckHelper { get; }
        void AddSameNamedKey(string keyName, BlackboardTemplate owner);
        
        IBlackboardKeyValidatorContext GetKeyValidatorContext(BlackboardKey key);
    }
}
#endif