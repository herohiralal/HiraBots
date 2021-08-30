#if UNITY_EDITOR
using UnityEditor;

namespace HiraBots
{
    internal partial class BlackboardTemplate
    {
        internal class Serialized : CustomSerializedObject<BlackboardTemplate>
        {
            internal Serialized(BlackboardTemplate obj) : base(obj)
            {
                backends = GetProperty(nameof(m_Backends),
                    SerializedPropertyType.Enum, false, true);

                parent = GetProperty<BlackboardTemplate>(nameof(m_Parent),
                    false, true);

                keys = GetProperty<BlackboardKey>(nameof(m_Keys),
                    true, true);
            }

            private SerializedProperty backends { get; }
            private SerializedProperty parent { get; }
            private SerializedProperty keys { get; }
        }
    }
}
#endif