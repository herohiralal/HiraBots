#if UNITY_EDITOR // ideally validation is only needed within the editor (either when building, or when exiting play mode)
using System.Runtime.CompilerServices;

namespace HiraBots
{
    internal partial class BlackboardTemplate
    {
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
            var hashSet = context.CyclicalHierarchyCheckHelper;
            hashSet.Clear();
            var (t, prev) = (this, (BlackboardTemplate) null);
            do
            {
                if (hashSet.Contains(t))
                {
                    context.MarkUnsuccessful();
                    context.RecursionPoint = prev;
                    return;
                }

                hashSet.Add(t);

                prev = t;
                t = t.parent;
            } while (t != null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EmptyKeyCheck(IBlackboardTemplateValidatorContext context)
        {
            var count = keys.Length;
            for (ushort i = 0; i < count; i++)
            {
                if (keys[i] != null)
                    continue;

                context.MarkUnsuccessful();
                context.AddEmptyKeyIndex(i);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SameNamedOrEmptyKeyCheck(IBlackboardTemplateValidatorContext context)
            // as a ScriptableObject can only ever have one name, this will also invalidate any duplicate key objects
        {
            var hashSet = context.SameNamedKeyCheckHelper;
            hashSet.Clear();

            var recursionPoint = context.RecursionPoint;

            BlackboardTemplate prev;
            var t = this;

            do
            {
                var currentKeySet = t.keys;
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
                t = t.parent;
            } while (t != null && prev != recursionPoint);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void IndividualKeyValidationCheck(IBlackboardTemplateValidatorContext context)
        {
            var count = keys.Length;
            for (var i = 0; i < count; i++)
            {
                var key = keys[i];
                if (key != null)
                    key.Validate(context.GetKeyValidatorContext(key));
            }
        }
    }
}

#endif