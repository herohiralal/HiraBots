using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    /// <summary>
    /// Custom property drawer for a Blackboard Key.
    /// The main feature is that it draws the key object directly.
    /// todo: implement this in UIElements (get rid of repeated creation/disposal of SerializedObjects per key)
    /// </summary>
    [CustomPropertyDrawer(typeof(BlackboardKey))]
    internal class BlackboardKeyObjectReferencePropertyDrawer : PropertyDrawer
    {
        // property names
        private const string k_InstanceSyncedProperty = "m_InstanceSynced";
        private const string k_EssentialToDecisionMakingProperty = "m_EssentialToDecisionMaking";
        private const string k_DefaultValueProperty = "m_DefaultValue";

        // using SerializedProperty.isExpanded has its pitfalls, such as being shared between all instances
        // of a blackboard template, which means that on a blackboard template with a parent, if you expand
        // a key of index 3, every parent in its chain of hierarchy will get index 3 expanded, which will be
        // reflected in the parent keys section of the blackboard template.
        private static readonly Dictionary<int, bool> s_ExpansionStatus = new Dictionary<int, bool>(40);

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var value = property.objectReferenceValue;

            if (value == null)
            {
                return 21f;
            }

            s_ExpansionStatus.TryGetValue(value.GetInstanceID(), out var expanded);

            return !expanded
                ? 21f
                : 0f
                  + 21f // header
                  + 21f // name edit
                  + 21f // traits - instance synced
                  + 21f // traits - essential to decision making
                  + 21f // default value
                  + 0f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // try find object
            var value = property.objectReferenceValue;

            if (value == null)
            {
                position.height -= 2f;
                EditorGUI.HelpBox(position, "Empty key detected.", MessageType.Error);
                return;
            }

            var so = new SerializedObject(value);

            var currentRect = position;
            currentRect.x += 20f;
            currentRect.width -= 20f;
            currentRect.height = 21f;

            // draw header, and check if it's expanded
            if (DrawHeader(currentRect, so))
            {
                currentRect.y += 1f;
                currentRect.height = 19f;

                // name field
                currentRect.y += 21f;
                var nameProperty = so.FindProperty("m_Name");
                if (nameProperty == null)
                {
                    EditorGUI.HelpBox(currentRect, "Missing name property", MessageType.Error);
                }
                else
                {
                    EditorGUI.PropertyField(currentRect, nameProperty);
                }

                // instance synced property field
                currentRect.y += 21f;
                var instanceSyncedProperty = so.FindProperty(k_InstanceSyncedProperty);
                if (instanceSyncedProperty == null)
                {
                    EditorGUI.HelpBox(currentRect, "Missing instance sync property.", MessageType.Error);
                }
                else
                {
                    EditorGUI.PropertyField(currentRect, instanceSyncedProperty);
                }

                // essential to decision-making property field
                currentRect.y += 21f;
                var essentialToDecisionMakingProperty = so.FindProperty(k_EssentialToDecisionMakingProperty);
                if (essentialToDecisionMakingProperty == null)
                {
                    EditorGUI.HelpBox(currentRect, "Missing essential to decision making property.", MessageType.Error);
                }
                else
                {
                    EditorGUI.PropertyField(currentRect, essentialToDecisionMakingProperty);
                }

                // default value property field
                currentRect.y += 21f;
                var defaultValueProperty = so.FindProperty(k_DefaultValueProperty);
                if (defaultValueProperty == null)
                {
                    EditorGUI.HelpBox(currentRect, "Unsupported default value property.", MessageType.Info);
                }
                else
                {
                    EditorGUI.PropertyField(currentRect, defaultValueProperty);
                }
            }

            so.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draw a header for a specific key object.
        /// </summary>
        /// <returns>Whether the header is expanded.</returns>
        private static bool DrawHeader(Rect position, SerializedObject so)
        {
            var instanceID = so.targetObject.GetInstanceID();
            if (!s_ExpansionStatus.ContainsKey(instanceID))
            {
                s_ExpansionStatus.Add(instanceID, false);
            }

            using (new GUIEnabledChanger(true))
            {
                // begin group
                var expanded = EditorGUI.BeginFoldoutHeaderGroup(position, s_ExpansionStatus[instanceID], GUIContent.none);

                // background
                position.height -= 2f;
                EditorGUI.DrawRect(position, BlackboardGUIHelpers.GetBlackboardKeyColor(so.targetObject) * 0.35f);

                // name
                position.height += 2f;
                position.x += 10f;
                position = EditorGUI.PrefixLabel(position, GUIHelpers.ToGUIContent(so.targetObject.name), EditorStyles.boldLabel);

                // type
                EditorGUI.LabelField(position, GUIHelpers.ToGUIContent(BlackboardGUIHelpers.formattedNames[so.targetObject.GetType()]));

                // end group
                EditorGUI.EndFoldoutHeaderGroup();

                // retrieve data
                s_ExpansionStatus[instanceID] = expanded;
                return expanded;
            }
        }
    }
}