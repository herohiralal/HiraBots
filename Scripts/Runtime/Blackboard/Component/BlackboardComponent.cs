using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace HiraBots
{
    internal unsafe partial class BlackboardComponent
    {
        private BlackboardTemplateCompiledData m_Template;
        private NativeArray<byte> m_Data;

        private byte* dataPtr => (byte*) m_Data.GetUnsafePtr();
        private byte* dataReadOnlyPtr => (byte*) m_Data.GetUnsafeReadOnlyPtr();

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
            m_UnexpectedChanges = new System.Collections.Generic.List<ushort>(template.m_KeyCount);
            m_Template.AddInstanceSyncListener(this);
        }

        ~BlackboardComponent()
        {
            m_Template.RemoveInstanceSyncListener(this);

            if (m_Data.IsCreated)
                m_Data.Dispose();

            m_Template = null;
        }
    }
}