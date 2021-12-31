using UnityEngine;

namespace HiraBots
{
    internal partial class LGOAPDomainCompiledData
    {
        internal string GetContainerName(int layerIndex, int containerIndex)
        {
            return layerIndex == 0 ? m_Goals[containerIndex].name : m_TaskLayers[layerIndex - 1][containerIndex].name;
        }

        internal bool IsTaskAbstract(int layerIndex, int containerIndex)
        {
            return m_TaskLayers[layerIndex - 1][containerIndex].isAbstract;
        }

        internal bool CheckTargetOnBlackboard(int layerIndex, int containerIndex, BlackboardComponent blackboard)
        {
            var target = layerIndex == 0 ? m_Goals[containerIndex].target : m_TaskLayers[layerIndex - 1][containerIndex].target;

            for (var i = 0; i < target.count; i++)
            {
                if (!target[i].Execute(blackboard, true))
                {
                    return false;
                }
            }

            return true;
        }

        internal bool CheckPreconditionOnBlackboard(int layerIndex, int containerIndex, BlackboardComponent blackboard)
        {
            var precondition = m_TaskLayers[layerIndex - 1][containerIndex].precondition;

            for (var i = 0; i < precondition.count; i++)
            {
                if (!precondition[i].Execute(blackboard, true))
                {
                    return false;
                }
            }

            return true;
        }

        internal void ApplyEffectOnBlackboard(int layerIndex, int containerIndex, BlackboardComponent blackboard, bool expected)
        {
            var effect = m_TaskLayers[layerIndex - 1][containerIndex].effect;

            for (var i = 0; i < effect.count; i++)
            {
                effect[i].Execute(blackboard, expected);
            }
        }

        internal void GetTaskProvider(int layerIndex, int containerIndex, out HiraBotsTaskProvider taskProvider)
        {
            taskProvider = m_TaskLayers[layerIndex - 1][containerIndex].taskProvider;
        }

        internal void GetServiceProviders(int layerIndex, int containerIndex, out ReadOnlyArrayAccessor<HiraBotsServiceProvider> serviceProviders)
        {
            serviceProviders = m_TaskLayers[layerIndex - 1][containerIndex].serviceProviders;
        }
    }
}