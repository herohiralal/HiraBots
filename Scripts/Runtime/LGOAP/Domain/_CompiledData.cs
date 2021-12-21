using System.Collections.Generic;
using Unity.Collections;

namespace HiraBots
{
    internal class LGOAPDomainCompiledData
    {
        private BlackboardTemplateCompiledData m_BlackboardCompiledData;
        private byte m_LayerCount;
        private NativeArray<byte> m_Domain = default;
        private readonly HashSet<BlackboardComponent> m_BlackboardComponents = new HashSet<BlackboardComponent>();

        /// <summary>
        /// Create a blackboard component.
        /// </summary>
        internal BlackboardComponent CreateBlackboardComponent()
        {
            BlackboardComponent.TryCreate(m_BlackboardCompiledData, out var output);
            m_BlackboardComponents.Add(output);
            return output;
        }

        /// <summary>
        /// Dispose a blackboard component if registered on this domain.
        /// </summary>
        internal void DisposeBlackboardComponent(BlackboardComponent component)
        {
            if (m_BlackboardComponents.Remove(component))
            {
                component.Dispose();
            }
        }

        internal LGOAPDomainCompiledData(BlackboardTemplateCompiledData blackboardCompiledData, NativeArray<byte> domain,
            byte layerCount)
        {
            m_BlackboardCompiledData = blackboardCompiledData;
            m_Domain = domain;
            m_LayerCount = layerCount;
        }

        internal void Dispose()
        {
            foreach (var blackboard in m_BlackboardComponents)
            {
                blackboard.Dispose();
            }

            m_BlackboardComponents.Clear();

            m_LayerCount = 0;

            if (m_Domain.IsCreated)
            {
                m_Domain.Dispose();
            }

            m_BlackboardCompiledData = null;
        }
    }
}