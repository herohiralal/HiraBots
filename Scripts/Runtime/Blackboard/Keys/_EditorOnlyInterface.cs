#if UNITY_EDITOR
using UnityEditor;

namespace HiraBots
{
    internal partial class BlackboardKey
    {
        internal class Serialized : CustomSerializedObject<BlackboardKey>
        {
            internal Serialized(BlackboardKey obj) : base(obj)
            {
                name = GetProperty("m_Name",
                    SerializedPropertyType.String, false, true);

                instanceSynced = GetProperty(nameof(m_InstanceSynced),
                    SerializedPropertyType.Boolean, false, true);

                essentialToDecisionMaking = GetProperty(nameof(m_EssentialToDecisionMaking),
                    SerializedPropertyType.Boolean, false, true);

                defaultValue = GetProperty("m_DefaultValue");
            }

            internal SerializedProperty name { get; }
            internal SerializedProperty instanceSynced { get; }
            internal SerializedProperty essentialToDecisionMaking { get; }
            internal SerializedProperty defaultValue { get; }
        }
    }
}
#endif