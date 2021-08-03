namespace HiraBots
{
    /// <summary>
    /// A collection of blackboard templates ordered by their hierarchical indices.
    /// </summary>
    internal class BlackboardTemplateCollection : CookedDataSingleton<BlackboardTemplateCollection>
    {
        [UnityEngine.SerializeField] private BlackboardTemplate[] m_Templates = new BlackboardTemplate[0];

        /// <summary>
        /// The number of templates present in this collection.
        /// </summary>
        internal int count => m_Templates.Length;

        /// <summary>
        /// Access a blackboard template in this collection.
        /// </summary>
        internal BlackboardTemplate this[int index] => m_Templates[index];

#if UNITY_EDITOR

        /// <summary>
        /// Factory method to create a BlackboardTemplateCollection from an array of blackboard templates
        /// within the editor.
        /// </summary>
        internal static BlackboardTemplateCollection Create(BlackboardTemplate[] input)
        {
            var createdInstance = CreateInstance<BlackboardTemplateCollection>();
            createdInstance.m_Templates = input;
            return createdInstance;
        }
#endif
    }
}