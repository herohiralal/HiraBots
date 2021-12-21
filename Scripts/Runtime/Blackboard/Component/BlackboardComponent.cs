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
            m_Template = template;
            m_Template.InitializeBlackboardComponent(out m_Data);
            m_UnexpectedChanges = new System.Collections.Generic.List<string>(template.keyCount);
            m_Template.AddInstanceSyncListener(this);
        }

        internal void Dispose()
        {
            m_Template.RemoveInstanceSyncListener(this);

            m_UnexpectedChanges.Clear();

            foreach (var keyData in m_Template.keyNameToKeyData.values)
            {
                if (keyData.keyType == BlackboardKeyType.Object && !keyData.instanceSynced)
                {
                    SetObjectValueWithoutValidation(in keyData, null, true);
                }
            }

            m_Template.DisposeBlackboardComponent(m_Data);

            m_Template = null;
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