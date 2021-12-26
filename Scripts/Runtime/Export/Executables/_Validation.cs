using System.Collections.Generic;

namespace UnityEngine
{
    public abstract partial class HiraBotsServiceProvider
    {
        public void Validate(List<string> errors)
        {
            try
            {
                Validate(errors.Add);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Callback before entering playmode or building a player. Report errors if any.
        /// </summary>
        protected virtual void Validate(System.Action<string> reportError)
        {
        }
    }

    public abstract partial class HiraBotsTaskProvider
    {
        public void Validate(List<string> errors)
        {
            try
            {
                Validate(errors.Add);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Callback before entering playmode or building a player. Report errors if any.
        /// </summary>
        protected virtual void Validate(System.Action<string> reportError)
        {
        }
    }
}