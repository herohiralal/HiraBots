using Unity.Collections;

namespace HiraBots
{
    internal class LGOAPDomainCompiledData
    {
        private BlackboardTemplateCompiledData m_BlackboardCompiledData;
        private NativeArray<byte> m_Domain = default;

        /// <summary>
        /// The number of layers within this LGOAP domain.
        /// </summary>
        internal byte layerCount { get; }

        /// <summary>
        /// The actual domain.
        /// </summary>
        internal NativeArray<byte> lowLevelDomain => m_Domain;

        /// <summary>
        /// The relevant blackboard.
        /// </summary>
        internal BlackboardTemplateCompiledData blackboard => m_BlackboardCompiledData;

        internal LGOAPDomainCompiledData(BlackboardTemplateCompiledData blackboardCompiledData, NativeArray<byte> domain,
            byte layerCount)
        {
            m_BlackboardCompiledData = blackboardCompiledData;
            m_Domain = domain;
            this.layerCount = layerCount;
        }

        internal void Dispose()
        {
            m_BlackboardCompiledData = null;

            if (m_Domain.IsCreated)
                m_Domain.Dispose();
        }
    }
}