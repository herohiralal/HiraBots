using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
        /// Get the context to validate blackboard keys.
        /// </summary>
        IBlackboardKeyValidatorContext GetKeyValidatorContext(BlackboardKey key);
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
            CyclicalHierarchyCheck(context);
            EmptyKeyCheck(context);
            SameNamedOrDuplicateKeyCheck(context);
            IndividualKeyValidationCheck(context);
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
            var count = m_Keys.Length;
            for (var i = 0; i < count; i++)
            {
                var key = m_Keys[i];
                if (key != null)
                {
                    key.Validate(context.GetKeyValidatorContext(key));
                }
            }
        }
    }
}