#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace HiraBots
{
    internal partial class BlackboardTemplate
    {
        internal class Serialized : CustomSerializedObject<BlackboardTemplate>
        {
            private readonly System.Collections.Generic.List<BlackboardTemplate> m_Hierarchy;

            internal Serialized(BlackboardTemplate obj) : base(obj)
            {
                backends = GetProperty(nameof(m_Backends),
                    SerializedPropertyType.Enum, false, true);

                parent = GetProperty<BlackboardTemplate>(nameof(m_Parent),
                    false, true);

                keys = GetProperty<BlackboardKey>(nameof(m_Keys),
                    true, true);

                m_Hierarchy = new System.Collections.Generic.List<BlackboardTemplate>();
                UpdateParentSerializedObjects();
            }

            internal SerializedProperty backends { get; }
            internal SerializedProperty parent { get; }
            internal ReadOnlyListAccessor<BlackboardTemplate> hierarchy => m_Hierarchy.ReadOnly();
            internal SerializedProperty keys { get; }

            internal void UpdateParentSerializedObjects()
            {
                m_Hierarchy.Clear();

                var current = target.m_Parent;
                while (current != null)
                {
                    m_Hierarchy.Add(current);
                    current = current.m_Parent;
                }
            }

            internal bool CanBeAssignedParent(BlackboardTemplate bt)
            {
                while (!ReferenceEquals(bt, null))
                {
                    if (bt == target)
                    {
                        return false;
                    }

                    bt = bt.m_Parent;
                }

                return true;
            }
        }
    }
}
#endif