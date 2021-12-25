namespace HiraBots
{
    internal class LGOAPDomainCollection : CookedDataSingleton<LGOAPDomainCollection>
    {
        [UnityEngine.SerializeField] private LGOAPDomain[] m_Domains = new LGOAPDomain[0];

        /// <summary>
        /// The number of templates present in this collection.
        /// </summary>
        internal int count => m_Domains.Length;

        /// <summary>
        /// Access a blackboard template in this collection.
        /// </summary>
        internal LGOAPDomain this[int index] => m_Domains[index];

#if UNITY_EDITOR

        /// <summary>
        /// Factory method to create a BlackboardTemplateCollection from an array of blackboard templates
        /// within the editor.
        /// </summary>
        internal static LGOAPDomainCollection Create(LGOAPDomain[] input)
        {
            var createdInstance = CreateInstance<LGOAPDomainCollection>();
            createdInstance.m_Domains = input;
            return createdInstance;
        }
#endif
    }
}