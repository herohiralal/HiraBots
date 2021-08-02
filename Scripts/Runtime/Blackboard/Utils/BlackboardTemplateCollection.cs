namespace HiraBots
{
    internal class BlackboardTemplateCollection : CookedDataSingleton<BlackboardTemplateCollection>
    {
        [UnityEngine.SerializeField] private BlackboardTemplate[] m_Templates = new BlackboardTemplate[0];

        internal int count => m_Templates.Length;
        internal BlackboardTemplate this[int index] => m_Templates[index];

#if UNITY_EDITOR
        internal static BlackboardTemplateCollection Create(BlackboardTemplate[] input)
        {
            var createdInstance = CreateInstance<BlackboardTemplateCollection>();
            createdInstance.m_Templates = input;
            return createdInstance;
        }
#endif
    }
}