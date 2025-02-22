using UnityEditor;
using UnityEditor.AI;
using UnityEngine;
using UnityEngine.AI;

namespace HiraBots.Editor
{
    [CustomPropertyDrawer(typeof(NavAgentType))]
    internal class NavAgentTypePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 21f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueProperty = property.FindPropertyRelative("m_Value");
            if (valueProperty == null)
            {
                EditorGUI.HelpBox(position, "Could not find value property.", MessageType.Error);
                return;
            }

            var index = -1;
            var count = NavMesh.GetSettingsCount();
            var agentTypeNames = new GUIContent[count];

            // build agent type names
            {
                for (var i = 0; i < count; i++)
                {
                    var id = NavMesh.GetSettingsByIndex(i).agentTypeID;
                    agentTypeNames[i] = new GUIContent(NavMesh.GetSettingsNameFromID(id));
                    if (id == valueProperty.intValue)
                    {
                        index = i;
                    }
                }

                index = index == -1 ? 0 : index;
            }

            position.height = 19f;
            position.width -= 21f;
            index = EditorGUI.Popup(position, label, index, agentTypeNames);

            if (index >= 0 && index < count)
            {
                valueProperty.intValue = NavMesh.GetSettingsByIndex(index).agentTypeID;
            }

            position.x += position.width + 2f;
            position.width = 19f;

            var settingsIcon = EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_SettingsIcon" : "SettingsIcon");
            var buttonContent = new GUIContent(settingsIcon);

            if (GUI.Button(position, buttonContent))
            {
                NavMeshEditorHelpers.OpenAgentSettings(-1);
            }
        }
    }
}