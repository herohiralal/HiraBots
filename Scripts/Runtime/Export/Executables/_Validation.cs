using System.Collections.Generic;

namespace UnityEngine
{
    public abstract partial class HiraBotsServiceProvider
    {
        internal void Validate(List<string> errors, ReadOnlyHashSetAccessor<HiraBots.BlackboardKey> keySet)
        {
            try
            {
                Validate(errors.Add, new BlackboardTemplate.KeySet(keySet));
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Callback before entering playmode or building a player. Report errors if any.
        /// </summary>
        protected virtual void Validate(System.Action<string> reportError, in BlackboardTemplate.KeySet keySet)
        {
        }
    }

    public abstract partial class HiraBotsTaskProvider
    {
        internal void Validate(List<string> errors, ReadOnlyHashSetAccessor<HiraBots.BlackboardKey> keySet)
        {
            try
            {
                Validate(errors.Add, new BlackboardTemplate.KeySet(keySet));
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Callback before entering playmode or building a player. Report errors if any.
        /// </summary>
        protected virtual void Validate(System.Action<string> reportError, in BlackboardTemplate.KeySet keySet)
        {
        }
    }
}