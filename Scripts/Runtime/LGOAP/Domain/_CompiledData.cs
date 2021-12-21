using Unity.Collections;

namespace HiraBots
{
    internal class LGOAPDomainCompiledData
    {
        private BlackboardTemplateCompiledData m_BlackboardCompiledData;
        private readonly byte[] m_PlanSizesByLayer;
        private NativeArray<byte> m_Domain = default;

        internal LGOAPDomainCompiledData(BlackboardTemplateCompiledData blackboardCompiledData, NativeArray<byte> domain,
            byte[] planSizesByLayer)
        {
            m_BlackboardCompiledData = blackboardCompiledData;
            m_Domain = domain;
            m_PlanSizesByLayer = planSizesByLayer;
        }

        internal void Dispose()
        {
            if (m_Domain.IsCreated)
            {
                m_Domain.Dispose();
            }

            m_BlackboardCompiledData = null;
        }
    }
}