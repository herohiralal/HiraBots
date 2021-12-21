using Unity.Collections;
using UnityEngine;

namespace HiraBots
{
    internal partial class LGOAPComponent
    {
        private struct PlannerResultsSet
        {
            internal PlannerResultsSet(ReadOnlyArrayAccessor<byte> planSizesByLayer)
            {
                var layerCount = planSizesByLayer.count;

                m_Internal = new PlannerResult[layerCount + 1];

                m_Internal[0] = new PlannerResult(1, Allocator.Persistent);

                for (var i = 0; i < layerCount; i++)
                {
                    m_Internal[i + 1] = new PlannerResult(planSizesByLayer[i], Allocator.Persistent);
                }
            }

            internal void Dispose()
            {
                for (var i = 0; i < m_Internal.Length; i++)
                {
                    m_Internal[i].Dispose();
                }

                m_Internal = null;
            }

            private PlannerResult[] m_Internal;

            /// <summary>
            /// The goal result.
            /// </summary>
            private PlannerResult goalResult => m_Internal[0];

            /// <summary>
            /// The result for a plan at the given layer index.
            /// </summary>
            private PlannerResult this[int layerIndex] => m_Internal[layerIndex + 1]; // first one is goal layer

            /// <summary>
            /// Check if the results set is valid.
            /// </summary>
            internal bool isValid => m_Internal != null;

            /// <summary>
            /// Copy the result set to another.
            /// </summary>
            internal void CopyTo(PlannerResultsSet other)
            {
                var countA = m_Internal.Length;
                var countB = other.m_Internal.Length;

                if (countA != countB)
                {
                    throw new System.InvalidOperationException();
                }

                for (var i = 0; i < countA; i++)
                {
                    m_Internal[i].CopyTo(other.m_Internal[i]);
                }
            }
        }
    }
}