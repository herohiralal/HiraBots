using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace HiraBots
{
    internal partial class LGOAPDomain
    {
        /// <summary>
        /// The data compiled for this LGOAP domain.
        /// </summary>
        internal LGOAPDomainCompiledData compiledData { get; private set; } = null;

        /// <summary>
        /// Whether the LGOAP domain has been compiled.
        /// </summary>
        internal bool isCompiled => compiledData != null;

        /// <summary>
        /// Compile this LGOAP domain.
        /// </summary>
        internal unsafe void Compile()
        {
            if (isCompiled)
            {
                // ignore if already compiled
                return;
            }

            if (!m_Blackboard.isCompiled)
            {
                // the blackboard must be compiled before self
                Debug.LogError("Attempted to compile a domain before its blackboard.", this);
                return;
            }

            // prepare all layers for compilation
            m_TopLayer.PrepareForCompilation();

            for (var i = 0; i < m_IntermediateLayers.Length; i++)
            {
                m_IntermediateLayers[i].PrepareForCompilation();
            }

            m_BottomLayer.PrepareForCompilation();

            // get the required allocation size
            var requiredSize = ByteStreamHelpers.CombinedSizes<byte>(); // header - layer count

            requiredSize += m_TopLayer.GetMemorySizeRequiredForCompilation();

            for (var i = 0; i < m_IntermediateLayers.Length; i++)
            {
                requiredSize += m_IntermediateLayers[i].GetMemorySizeRequiredForCompilation();
            }

            requiredSize += m_BottomLayer.GetMemorySizeRequiredForCompilation();

            requiredSize = UnsafeHelpers.GetAlignedSize(requiredSize);

            // allocate
            var domain = new NativeArray<byte>(requiredSize, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            var domainAddress = (byte*) domain.GetUnsafePtr();

            CompilationRegistry.BeginObject(this);

            CompilationRegistry.IncreaseDepth();

            // write layer count header
            ByteStreamHelpers.Write<byte>(ref domainAddress, (byte) (m_IntermediateLayers.Length + 2));

            // write top layer
            var topLayerStart = domainAddress;
            m_TopLayer.Compile(ref domainAddress);
            CompilationRegistry.AddEntry("Top Layer", topLayerStart, domainAddress);

            // write intermediate layers
            for (var i = 0; i < m_IntermediateLayers.Length; i++)
            {
                var intermediateLayerStart = domainAddress;
                m_IntermediateLayers[i].Compile(ref domainAddress);
                CompilationRegistry.AddEntry($"Layer {i}", intermediateLayerStart, domainAddress);
            }

            // write bottom layer
            var bottomLayerStart = domainAddress;
            m_BottomLayer.Compile(ref domainAddress);
            CompilationRegistry.AddEntry($"Layer {m_IntermediateLayers.Length}", bottomLayerStart, domainAddress);

            CompilationRegistry.DecreaseDepth();

            CompilationRegistry.AddEntry(name, (byte*) domain.GetUnsafePtr(), domainAddress);
            CompilationRegistry.EndObject();

            // max plan sizes by layer
            var maxPlanSizesByLayer = new byte[m_IntermediateLayers.Length + 2];

            {
                maxPlanSizesByLayer[0] = m_TopLayer.m_MaxPlanSize = 1;

                for (var i = 0; i < maxPlanSizesByLayer.Length - 2; i++)
                {
                    maxPlanSizesByLayer[i + 1] = m_IntermediateLayers[i].m_MaxPlanSize;
                }

                maxPlanSizesByLayer[maxPlanSizesByLayer.Length - 1] = m_BottomLayer.m_MaxPlanSize;
            }

            // fallback plans
            var fallbackPlans = new short[m_IntermediateLayers.Length + 2][];

            {
                // create copies instead of passing around the main array and run the risk of it getting edited

                fallbackPlans[0] = (short[]) m_TopLayer.m_FallbackGoal.Clone();

                for (var i = 0; i < fallbackPlans.Length - 2; i++)
                {
                    fallbackPlans[i + 1] = (short[]) m_IntermediateLayers[i].m_FallbackPlan.Clone();
                }

                fallbackPlans[fallbackPlans.Length - 1] = (short[]) m_BottomLayer.m_FallbackPlan.Clone();
            }

            // goals
            var goals = new LGOAPGoalCompiledData[m_TopLayer.m_Goals.Length];

            {
                for (var i = 0; i < goals.Length; i++)
                {
                    goals[i] = new LGOAPGoalCompiledData(m_TopLayer.m_Goals[i]);
                }
            }

            // tasks layers
            var tasksLayers = new ReadOnlyArrayAccessor<LGOAPTaskCompiledData>[m_IntermediateLayers.Length + 1];

            {
                // intermediate layers
                for (var i = 0; i < tasksLayers.Length - 1; i++)
                {
                    var currentLayer = m_IntermediateLayers[i];

                    var tasks = new LGOAPTaskCompiledData[currentLayer.m_Tasks.Length];

                    for (var j = 0; j < tasks.Length; j++)
                    {
                        tasks[j] = new LGOAPTaskCompiledData(currentLayer.m_Tasks[j]);
                    }

                    tasksLayers[i] = tasks.ReadOnly();
                }

                // bottom layer
                {
                    var tasks = new LGOAPTaskCompiledData[m_BottomLayer.m_Tasks.Length];

                    for (var i = 0; i < tasks.Length; i++)
                    {
                        tasks[i] = new LGOAPTaskCompiledData(m_BottomLayer.m_Tasks[i]);
                    }

                    tasksLayers[m_IntermediateLayers.Length] = tasks.ReadOnly();
                }
            }

            compiledData = new LGOAPDomainCompiledData(m_Blackboard.compiledData,
                domain,
                maxPlanSizesByLayer.ReadOnly(),
                fallbackPlans.ReadOnly(),
                goals.ReadOnly(),
                tasksLayers.ReadOnly());
        }

        /// <summary>
        /// Free the compiled data.
        /// </summary>
        internal void Free()
        {
            compiledData.Dispose();
            compiledData = null;
        }
    }
}