﻿namespace UnityEngine.AI
{
    public abstract partial class HiraBotsServiceProvider
    {
        /// <summary>
        /// The static description of this service provider.
        /// </summary>
        [SerializeField, HideInInspector] private string m_Description = "";

        /// <summary>
        /// The static description of this service provider.
        /// </summary>
        public string description => m_Description;

        internal void OnValidate()
        {
            tickInterval = m_TickInterval;

            try
            {
                OnValidateCallback();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

            try
            {
                UpdateDescription(out m_Description);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Callback for Unity's OnValidate message.
        /// </summary>
        protected virtual void OnValidateCallback()
        {
        }

        internal void OnTargetBlackboardTemplateChangedWrapped(HiraBots.BlackboardTemplate template, ReadOnlyHashSetAccessor<HiraBots.BlackboardKey> keySet)
        {
            try
            {
                OnTargetBlackboardTemplateChanged(template, new BlackboardTemplate.KeySet(keySet));
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

            try
            {
                UpdateDescription(out m_Description);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Callback for when the target blackboard template changes.
        /// </summary>
        protected virtual void OnTargetBlackboardTemplateChanged(BlackboardTemplate template, in BlackboardTemplate.KeySet keySet)
        {
        }

        /// <summary>
        /// Update the static description of this object.
        /// </summary>
        protected virtual void UpdateDescription(out string staticDescription)
        {
            staticDescription = "Customize this description by overriding UpdateDescription().";
        }
    }

    public abstract partial class HiraBotsTaskProvider
    {
        /// <summary>
        /// The static description of this task provider.
        /// </summary>
        [SerializeField, HideInInspector] private string m_Description = "Customize this description by overriding UpdateDescription().";

        /// <summary>
        /// The static description of this task provider.
        /// </summary>
        public string description => m_Description;

        internal void OnValidate()
        {
            tickInterval = m_TickInterval;

            try
            {
                OnValidateCallback();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

            try
            {
                UpdateDescription(out m_Description);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Callback for Unity's OnValidate message.
        /// </summary>
        protected virtual void OnValidateCallback()
        {
        }

        internal void OnTargetBlackboardTemplateChangedWrapped(HiraBots.BlackboardTemplate template, ReadOnlyHashSetAccessor<HiraBots.BlackboardKey> keySet)
        {
            try
            {
                OnTargetBlackboardTemplateChanged(template, new BlackboardTemplate.KeySet(keySet));
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

            try
            {
                UpdateDescription(out m_Description);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Callback for when the target blackboard template changes.
        /// </summary>
        protected virtual void OnTargetBlackboardTemplateChanged(BlackboardTemplate template, in BlackboardTemplate.KeySet keySet)
        {
        }

        /// <summary>
        /// Update the static description of this object.
        /// </summary>
        protected virtual void UpdateDescription(out string staticDescription)
        {
            staticDescription = "Customize this description by overriding UpdateDescription().";
        }
    }
}