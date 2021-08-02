namespace HiraBots
{
    internal class BlackboardTemplateCollection : CookedDataSingleton<BlackboardTemplateCollection>
    {
        [UnityEngine.SerializeField] private BlackboardTemplate[] templates = new BlackboardTemplate[0];

        internal int count => templates.Length;
        internal BlackboardTemplate this[int index] => templates[index];

#if UNITY_EDITOR
        internal static BlackboardTemplateCollection Create(BlackboardTemplate[] input)
        {
            var createdInstance = CreateInstance<BlackboardTemplateCollection>();
            createdInstance.templates = input;
            return createdInstance;
        }
#endif
    }
}