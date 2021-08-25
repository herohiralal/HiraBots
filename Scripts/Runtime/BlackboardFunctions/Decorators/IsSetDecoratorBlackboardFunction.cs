﻿using System;
using AOT;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace HiraBots
{
    [BurstCompile]
    internal unsafe partial class IsSetDecoratorBlackboardFunction : DecoratorBlackboardFunction
    {
        private struct Memory
        {
            internal BlackboardKeyType m_KeyType;
            internal ushort m_Offset;
        }

        private static bool s_FunctionCompiled = false;
        private static FunctionPointer<DecoratorDelegate> s_Function;

        internal override void PrepareForCompilation()
        {
            base.PrepareForCompilation();
            m_MemorySize += ByteStreamHelpers.CombinedSizes<Memory>();
            if (!s_FunctionCompiled)
            {
                s_Function = BurstCompiler.CompileFunctionPointer<DecoratorDelegate>(ActualFunction);
                s_FunctionCompiled = true;
            }
        }

        [Tooltip("The key to look up.")]
        [SerializeField] private BlackboardTemplate.KeySelector m_Key = default;

        // compile override
        public override void Compile(ref byte* stream)
        {
            base.Compile(ref stream);

            // no offset
            ByteStreamHelpers.Write(ref stream, memory);

            // offset sizeof(Memory)
        }

        // function override
        protected override FunctionPointer<DecoratorDelegate> function => s_Function;

        // actual function
        [BurstCompile(DisableDirectCall = true), MonoPInvokeCallback(typeof(DecoratorDelegate))]
        private static bool ActualFunction(in LowLevelBlackboard blackboard, byte* rawMemory)
        {
            var memory = (Memory*) rawMemory;

            var offset = memory->m_Offset;
            switch (memory->m_KeyType)
            {
                case BlackboardKeyType.Boolean:
                    return blackboard.Access<bool>(offset);
                case BlackboardKeyType.Quaternion:
                    var value4 = blackboard.Access<quaternion>(offset).value != quaternion.identity.value;
                    return !value4.w || !value4.x || !value4.y || !value4.z;
                case BlackboardKeyType.Vector:
                    var value3 = blackboard.Access<float3>(offset) != float3.zero;
                    return !value3.x || !value3.y || !value3.z;
                default:
                    throw new ArgumentOutOfRangeException($"Invalid key type: {memory->m_KeyType}");
            }
        }

        // pack memory
        private Memory memory => new Memory {m_KeyType = m_Key.selectedKey.keyType, m_Offset = m_Key.selectedKey.compiledData.memoryOffset};
    }
}