using Unity.Collections;
using UnityEngine;

namespace HiraBots
{
    internal partial class LGOAPPlannerComponent
    {
        private struct PlanSet
        {
            internal PlanSet(ReadOnlyArrayAccessor<byte> planSizesByLayer)
            {
                var layerCount = planSizesByLayer.count;

                m_Internal = new LGOAPPlan[layerCount + 1];

                m_Internal[0] = new LGOAPPlan(1, Allocator.Persistent);

                for (var i = 0; i < layerCount; i++)
                {
                    m_Internal[i + 1] = new LGOAPPlan(planSizesByLayer[i], Allocator.Persistent);
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

            private LGOAPPlan[] m_Internal;

            /// <summary>
            /// The goal result.
            /// </summary>
            internal LGOAPPlan goalResult => m_Internal[0];

            /// <summary>
            /// The result for a plan at the given layer index.
            /// </summary>
            internal ref LGOAPPlan this[int layerIndex] => ref m_Internal[layerIndex + 1]; // first one is goal layer

            /// <summary>
            /// Copy the result set to another.
            /// </summary>
            internal void CopyTo(PlanSet dst)
            {
                var countA = m_Internal.Length;
                var countB = dst.m_Internal.Length;

                if (countA != countB)
                {
                    throw new System.InvalidOperationException();
                }

                // individual size checking will be done by NativeArray<T>.Copy itself
                // no point in doing it here again

                for (var i = 0; i < countA; i++)
                {
                    m_Internal[i].CopyTo(dst.m_Internal[i]);
                }
            }
        }
    }
}