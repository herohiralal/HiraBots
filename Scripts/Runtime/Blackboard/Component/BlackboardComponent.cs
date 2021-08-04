using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace HiraBots
{
    /// <summary>
    /// Blackboard component to be used as a part of an AI brain.
    /// </summary>
    internal unsafe partial class BlackboardComponent
    {
        private BlackboardTemplateCompiledData m_Template;
        private NativeArray<byte> m_Data;

        private byte* dataPtr => (byte*) m_Data.GetUnsafePtr();
        private byte* dataReadOnlyPtr => (byte*) m_Data.GetUnsafeReadOnlyPtr();

        /// <summary>
        /// Attempt to create a BlackboardComponent from a template.
        /// </summary>
        /// <returns>Whether the process was successful.</returns>
        internal static bool TryCreate(BlackboardTemplate template, out BlackboardComponent component)
        {
            if (template == null)
            {
                component = null;
                return false;
            }

            if (!template.isCompiled)
            {
                component = null;
                return false;
            }

            component = new BlackboardComponent(template.compiledData);
            return true;
        }

        private BlackboardComponent(BlackboardTemplateCompiledData template)
        {
            m_Template = template;
            m_Data = new NativeArray<byte>(m_Template.templateSize, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            m_Template.CopyTemplateTo(m_Data);
            m_UnexpectedChanges = new System.Collections.Generic.List<ushort>(template.keyCount);
            m_Template.AddInstanceSyncListener(this);
        }

        ~BlackboardComponent()
        {
            m_Template.RemoveInstanceSyncListener(this);

            foreach (var keyData in m_Template.memoryOffsetToKeyData.values)
            {
                if (keyData.keyType == BlackboardKeyType.Object && !keyData.instanceSynced)
                {
                    SetObjectValueWithoutValidation(in keyData, null, true);
                }
            }

            if (m_Data.IsCreated)
            {
                m_Data.Dispose();
            }

            m_Template = null;
        }

        internal NativeArray<byte> GetCopy(Allocator allocator)
        {
            var length = m_Data.Length;
            var copy = new NativeArray<byte>(length, allocator, NativeArrayOptions.UninitializedMemory);
            UnsafeUtility.MemCpy(copy.GetUnsafePtr(), dataReadOnlyPtr, length);
            return copy;
        }

        /// <summary>
        /// Implicitly convert a BlackboardComponent to its public interface.
        /// </summary>
        public static implicit operator UnityEngine.BlackboardComponent(BlackboardComponent actualComponent)
        {
            return new UnityEngine.BlackboardComponent(actualComponent);
        }
    }
}