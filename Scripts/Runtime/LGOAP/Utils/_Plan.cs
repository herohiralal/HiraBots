using Unity.Collections;

namespace HiraBots
{
    internal struct LGOAPPlan
    {
        internal struct ReadOnly
        {
            [ReadOnly] private NativeArray<short> m_Internal;

            internal ReadOnly(short[] plan)
            {
                m_Internal = new NativeArray<short>(plan, Allocator.Persistent);
            }

            internal void Dispose()
            {
                if (m_Internal.IsCreated)
                {
                    m_Internal.Dispose();
                }
            }

            internal short length => (short) m_Internal.Length;

            internal short this[int index]
            {
                get => m_Internal[index];
                set => m_Internal[index] = value;
            }

            internal void CopyTo(LGOAPPlan dst)
            {
                var selfLength = length;

                NativeArray<short>.Copy(m_Internal, 0, dst.m_Internal, k_HeaderSize, selfLength);
                dst.length = selfLength;
                dst.resultType = Type.NewPlan;
                dst.currentIndex = 0;
            }
        }

        internal struct Set
        {
            // ReSharper disable once MemberHidesStaticFromOuterClass
            internal struct ReadOnly
            {
                private LGOAPPlan.ReadOnly[] m_Internal;

                internal ReadOnly(UnityEngine.ReadOnlyArrayAccessor<short[]> plans)
                {
                    var layerCount = plans.count;

                    m_Internal = new LGOAPPlan.ReadOnly[layerCount];

                    for (var i = 0; i < layerCount; i++)
                    {
                        m_Internal[i] = new LGOAPPlan.ReadOnly(plans[i]);
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

                /// <summary>
                /// The plan at the given layer index.
                /// </summary>
                internal ref LGOAPPlan.ReadOnly this[int layerIndex] => ref m_Internal[layerIndex];

                /// <summary>
                /// Copy the plan set to another.
                /// </summary>
                internal void CopyTo(Set dst)
                {
                    var countA = m_Internal.Length;
                    var countB = dst.m_Internal.Length;

                    if (countA != countB)
                    {
                        throw new System.InvalidOperationException();
                    }

                    // individual size checking will be done by LGOAPPlan.ReadOnly itsel
                    // no point in doing it here again

                    for (var i = 0; i < countA; i++)
                    {
                        m_Internal[i].CopyTo(dst.m_Internal[i]);
                    }
                }
            }

            private LGOAPPlan[] m_Internal;

            internal Set(UnityEngine.ReadOnlyArrayAccessor<byte> maxPlanSizesByLayer)
            {
                var layerCount = maxPlanSizesByLayer.count;

                m_Internal = new LGOAPPlan[layerCount];

                for (var i = 0; i < layerCount; i++)
                {
                    m_Internal[i] = new LGOAPPlan(maxPlanSizesByLayer[i], Allocator.Persistent);
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

            /// <summary>
            /// The plan at the given layer index.
            /// </summary>
            internal ref LGOAPPlan this[int layerIndex] => ref m_Internal[layerIndex];

            /// <summary>
            /// Copy the plan set to another.
            /// </summary>
            internal void CopyTo(Set dst)
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

        internal enum Type : short
        {
            NotRequired = 1,
            Unchanged = 2,
            NewPlan = 3
        }

        private const short k_TypeIndex = 0;
        private const short k_CountIndex = 1;
        private const short k_CurrentIndexIndex = 2;
        private const short k_HeaderSize = 3;

        private NativeArray<short> m_Internal;

        internal LGOAPPlan(short bufferSize, Allocator allocator)
        {
            m_Internal = new NativeArray<short>(bufferSize + k_HeaderSize, allocator, NativeArrayOptions.UninitializedMemory);
            length = 0;
            resultType = Type.NewPlan;
            currentIndex = 0;
        }

        internal void Dispose()
        {
            if (m_Internal.IsCreated)
            {
                m_Internal.Dispose();
            }
        }

        internal Type resultType
        {
            get => (Type) m_Internal[k_TypeIndex];
            set => m_Internal[k_TypeIndex] = (short) value;
        }

        internal short length
        {
            get => m_Internal[k_CountIndex];
            set => m_Internal[k_CountIndex] = value;
        }

        internal short maxLength => (byte) (m_Internal.Length - k_HeaderSize);

        internal short this[short index]
        {
            get => m_Internal[index + k_HeaderSize];
            set => m_Internal[index + k_HeaderSize] = value;
        }

        internal void CopyTo(LGOAPPlan dst)
        {
            m_Internal.CopyTo(dst.m_Internal);
        }

        internal short currentIndex
        {
            get => m_Internal[k_CurrentIndexIndex];
            set => m_Internal[k_CurrentIndexIndex] = value;
        }
    }
}