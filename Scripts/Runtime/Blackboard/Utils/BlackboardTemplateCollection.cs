namespace HiraBots
{
    internal class BlackboardTemplateCollection : CookedDataSingleton<BlackboardTemplateCollection>
    {
        [UnityEngine.SerializeField] private BlackboardTemplate[] templates = new BlackboardTemplate[0];

        internal int Count => templates.Length;
        internal BlackboardTemplate this[int index] => templates[index];

#if UNITY_EDITOR
        internal static BlackboardTemplateCollection Create(BlackboardTemplate[] input)
        {
            var instance = CreateInstance<BlackboardTemplateCollection>();
            instance.templates = input;
            return instance;
        }
#endif
    }
}