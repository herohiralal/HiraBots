#if UNITY_EDITOR || HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// The context required to validate a blackboard template.
    /// </summary>
    internal interface IBlackboardTemplateValidatorContext
    {
        /// <summary>
        /// Mark the validation as unsuccessful.
        /// </summary>
        void MarkUnsuccessful();

        /// <summary>
        /// Whether the backends are unsupported.
        /// </summary>
        BackendType missingBackends { get; set; }

        /// <summary>
        /// Get a cached hashset to check for cyclical hierarchy.
        /// </summary>
        HashSet<BlackboardTemplate> cyclicalHierarchyCheckHelper { get; }

        /// <summary>
        /// The recursion point, if cyclical hierarchy was indeed found.
        /// </summary>
        BlackboardTemplate recursionPoint { get; set; }

        /// <summary>
        /// Add the index of an empty key.
        /// </summary>
        void AddEmptyKeyIndex(int index);

        /// <summary>
        /// Get a cached hashset to check for same named keys.
        /// </summary>
        HashSet<string> sameNamedKeyCheckHelper { get; }

        /// <summary>
        /// Add the name of a same named key, and its corresponding template.
        /// </summary>
        void AddSameNamedKey(string keyName, BlackboardTemplate owner);

        /// <summary>
        /// Add a key that failed its internal validation.
        /// </summary>
        void AddBadKey(BlackboardKey key);
    }

    internal partial class BlackboardTemplate
    {
        /// <summary>
        /// Get the hierarchy index of this blackboard template.
        /// </summary>
        internal ushort hierarchyIndex => m_Parent == null ? (ushort) 0 : (ushort) (m_Parent.hierarchyIndex + 1);

        /// <summary>
        /// Validate this blackboard template.
        /// </summary>
        internal void Validate(IBlackboardTemplateValidatorContext context)
        {
            BackendCheck(context);
            CyclicalHierarchyCheck(context);
            EmptyKeyCheck(context);
            SameNamedOrDuplicateKeyCheck(context);
            IndividualKeyValidationCheck(context);
        }

        /// <summary>
        /// Check whether the parent template contains all the backends this template requires.
        /// </summary>
        private void BackendCheck(IBlackboardTemplateValidatorContext context)
        {
            if (m_Parent != null)
            {
                var parentBackends = m_Parent.m_Backends;
                var selfBackends = m_Backends;

                var missingBackends = 0;

                if (selfBackends.HasFlag(BackendType.RuntimeInterpreter) && !parentBackends.HasFlag(BackendType.RuntimeInterpreter))
                {
                    missingBackends |= (int) BackendType.RuntimeInterpreter;
                }

                if (selfBackends.HasFlag(BackendType.CodeGenerator) && !parentBackends.HasFlag(BackendType.CodeGenerator))
                {
                    missingBackends |= (int) BackendType.CodeGenerator;
                }

                if (missingBackends != 0)
                {
                    context.MarkUnsuccessful();
                    context.missingBackends = (BackendType) missingBackends;
                }
            }
        }

        /// <summary>
        /// Check for cyclical hierarchy within the template.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CyclicalHierarchyCheck(IBlackboardTemplateValidatorContext context)
        {
            var hashSet = context.cyclicalHierarchyCheckHelper;
            hashSet.Clear();

            var (t, prev) = (this, (BlackboardTemplate) null);
            do
            {
                if (hashSet.Contains(t))
                {
                    context.MarkUnsuccessful();
                    context.recursionPoint = prev;
                    return;
                }

                hashSet.Add(t);

                prev = t;
                t = t.m_Parent;
            } while (t != null);
        }

        /// <summary>
        /// Check for empty keys within the template.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EmptyKeyCheck(IBlackboardTemplateValidatorContext context)
        {
            var count = m_Keys.Length;
            for (ushort i = 0; i < count; i++)
            {
                if (m_Keys[i] != null)
                {
                    continue;
                }

                context.MarkUnsuccessful();
                context.AddEmptyKeyIndex(i);
            }
        }

        /// <summary>
        /// Checked for same named or duplicate keys.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SameNamedOrDuplicateKeyCheck(IBlackboardTemplateValidatorContext context)
            // as a ScriptableObject can only ever have one name, this will also invalidate any duplicate key objects
        {
            var hashSet = context.sameNamedKeyCheckHelper;
            hashSet.Clear();

            var recursionPoint = context.recursionPoint;

            BlackboardTemplate prev;
            var t = this;

            do
            {
                var currentKeySet = t.m_Keys;
                var count = currentKeySet.Length;
                for (var i = 0; i < count; i++)
                {
                    var currentKey = currentKeySet[i];
                    if (currentKey == null)
                    {
                        continue;
                    }

                    var currentKeyName = currentKey.name;
                    if (hashSet.Contains(currentKeyName))
                    {
                        context.MarkUnsuccessful();
                        context.AddSameNamedKey(currentKeyName, t);
                    }
                    else
                    {
                        hashSet.Add(currentKeyName);
                    }
                }

                prev = t;
                t = t.m_Parent;
            } while (t != null && prev != recursionPoint);
        }

        /// <summary>
        /// Check if any of the keys fail validation individually.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void IndividualKeyValidationCheck(IBlackboardTemplateValidatorContext context)
        {
            var keyValidatorContext = new BlackboardKeyValidatorContext();

            var count = m_Keys.Length;
            for (var i = 0; i < count; i++)
            {
                var key = m_Keys[i];
                if (key != null)
                {
                    keyValidatorContext.succeeded = true;

                    key.Validate(ref keyValidatorContext);

                    if (!keyValidatorContext.succeeded)
                    {
                        context.AddBadKey(key);
                    }
                }
            }
        }
    }

    internal struct BlackboardTemplateKeySelectorValidatorContext
    {
        /// <summary>
        /// Whether the validation succeeded.
        /// </summary>
        internal bool succeeded { get; set; }
        
        /// <summary>
        /// The pool of allowed keys.
        /// </summary>
        internal ReadOnlyHashSetAccessor<BlackboardKey> allowedKeyPool { get; set; }

        /// <summary>
        /// The allowed key types.
        /// </summary>
        internal BlackboardKeyType allowedKeyTypes { get; set; }
    }

    internal partial class BlackboardTemplate
    {
        internal partial struct KeySelector
        {
            /// <summary>
            /// Validate whether the selected key is compatible with the filters.
            /// </summary>
            internal void Validate(ref BlackboardTemplateKeySelectorValidatorContext context)
            {
                // validate key itself
                if (m_Key == null)
                {
                    context.succeeded = false;
                    return;
                }

                // validate template filter
                if (!context.allowedKeyPool.Contains(m_Key))
                {
                    context.succeeded = false;
                }

                // validate key types filter
                if (!context.allowedKeyTypes.HasFlag(m_Key.keyType))
                {
                    context.succeeded = false;
                }
            }
        }
    }
}
#endif