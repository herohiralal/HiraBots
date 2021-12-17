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
            CompilationRegistry.AddEntry("Layer Count", domainAddress, domainAddress + sizeof(byte));

            // write layer count header
            ByteStreamHelpers.Write<byte>(ref domainAddress, (byte) (m_IntermediateLayers.Length + 1));

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

            compiledData = new LGOAPDomainCompiledData(m_Blackboard.compiledData, domain, (byte) (m_IntermediateLayers.Length + 1));
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