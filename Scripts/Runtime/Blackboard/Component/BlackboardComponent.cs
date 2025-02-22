using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace HiraBots
{
    /// <summary>
    /// Blackboard component to be used as a part of an AI brain.
    /// </summary>
    internal unsafe partial class BlackboardComponent
    {
        private static ulong s_Id = 0;

        private readonly ulong m_Id;
        private BlackboardTemplateCompiledData m_Template;
        private NativeArray<byte> m_Data;
        private ObjectCache m_ObjectCache;

        /// <summary>
        /// Reset the static id assigner.
        /// </summary>
        internal static void ResetStaticIDAssigner()
        {
            s_Id = 0;
        }

        private byte* dataPtr => (byte*) m_Data.GetUnsafePtr();
        private byte* dataReadOnlyPtr => (byte*) m_Data.GetUnsafeReadOnlyPtr();

        /// <summary>
        /// Create a copy of the blackboard.
        /// </summary>
        internal NativeArray<byte> Copy(Allocator allocator)
        {
            return new NativeArray<byte>(m_Data, allocator);
        }

        /// <summary>
        /// Attempt to create a BlackboardComponent from the compiled data of a template.
        /// </summary>
        /// <returns>Whether the process was successful.</returns>
        internal static bool TryCreate(BlackboardTemplateCompiledData template, out BlackboardComponent component)
        {
            if (template == null)
            {
                component = null;
                return false;
            }

            component = new BlackboardComponent(template);
            return true;
        }

        private BlackboardComponent(BlackboardTemplateCompiledData template)
        {
            m_Id = ++s_Id;

            m_Template = template;
            m_Data = new NativeArray<byte>(m_Template.templateSize, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            m_Template.CopyTemplateTo(m_Data);
            m_UnexpectedChanges = new System.Collections.Generic.List<string>(template.keyCount);
            m_Template.AddInstanceSyncListener(this);
            m_ObjectCache = new ObjectCache(0);
        }

        internal void Dispose()
        {
            m_ObjectCache.Clear();

            m_Template.RemoveInstanceSyncListener(this);

            m_UnexpectedChanges.Clear();

            foreach (var keyData in m_Template.keyNameToKeyData.values)
            {
                if (keyData.keyType == BlackboardKeyType.Object && !keyData.instanceSynced)
                {
                    SetObjectValueWithoutValidation(in keyData, null, true);
                }
            }

            m_Data.Dispose();

            m_Template = null;
        }

        /// <summary>
        /// Implicitly convert a BlackboardComponent to its public interface.
        /// </summary>
        public static implicit operator UnityEngine.AI.BlackboardComponent(BlackboardComponent actualComponent)
        {
            return new UnityEngine.AI.BlackboardComponent(actualComponent);
        }
    }
}