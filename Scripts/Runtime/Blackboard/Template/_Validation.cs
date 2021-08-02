#if UNITY_EDITOR // ideally validation is only needed within the editor (either when building, or when exiting play mode)
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HiraBots
{
    internal interface IBlackboardKeyValidatorContext
    {
        void MarkUnsuccessful();
    }

    internal interface IBlackboardTemplateValidatorContext
    {
        void MarkUnsuccessful();

        HashSet<BlackboardTemplate> cyclicalHierarchyCheckHelper { get; }
        BlackboardTemplate recursionPoint { get; set; }

        void AddEmptyKeyIndex(int index);

        HashSet<string> sameNamedKeyCheckHelper { get; }
        void AddSameNamedKey(string keyName, BlackboardTemplate owner);

        IBlackboardKeyValidatorContext GetKeyValidatorContext(BlackboardKey key);
    }

    internal abstract partial class BlackboardKey
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Validate(IBlackboardKeyValidatorContext context)
        {
            if (m_KeyType == BlackboardKeyType.Invalid)
                context.MarkUnsuccessful();
        }
    }

    internal partial class BlackboardTemplate
    {
        internal ushort hierarchyIndex => m_Parent == null ? (ushort) 0 : (ushort) (m_Parent.hierarchyIndex + 1);

        internal void Validate(IBlackboardTemplateValidatorContext context)
        {
            CyclicalHierarchyCheck(context);
            EmptyKeyCheck(context);
            SameNamedOrEmptyKeyCheck(context);
            IndividualKeyValidationCheck(context);
        }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EmptyKeyCheck(IBlackboardTemplateValidatorContext context)
        {
            var count = m_Keys.Length;
            for (ushort i = 0; i < count; i++)
            {
                if (m_Keys[i] != null)
                    continue;

                context.MarkUnsuccessful();
                context.AddEmptyKeyIndex(i);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SameNamedOrEmptyKeyCheck(IBlackboardTemplateValidatorContext context)
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
                    if (currentKey == null) continue;

                    var currentKeyName = currentKey.name;
                    if (hashSet.Contains(currentKeyName))
                    {
                        context.MarkUnsuccessful();
                        context.AddSameNamedKey(currentKeyName, t);
                    }
                    else hashSet.Add(currentKeyName);
                }

                prev = t;
                t = t.m_Parent;
            } while (t != null && prev != recursionPoint);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void IndividualKeyValidationCheck(IBlackboardTemplateValidatorContext context)
        {
            var count = m_Keys.Length;
            for (var i = 0; i < count; i++)
            {
                var key = m_Keys[i];
                if (key != null)
                    key.Validate(context.GetKeyValidatorContext(key));
            }
        }
    }
}

#endif