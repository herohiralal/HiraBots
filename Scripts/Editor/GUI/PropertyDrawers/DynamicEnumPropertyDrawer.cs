using System;
using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    [CustomPropertyDrawer(typeof(DynamicEnum))]
    public class DynamicEnumPropertyDrawer : PropertyDrawer
    {
        private const float property_height = 21f;

        internal static void DrawWithAutomaticLayout(SerializedProperty property, GUIContent label, Type typeRestriction)
        {
            var position = EditorGUILayout.GetControlRect(true, property_height);
            Draw(position, property, label, typeRestriction);
        }

        internal static void DrawWithAutomaticLayout(SerializedProperty property, GUIContent label, string typeIdentifierRestriction)
        {
            var position = EditorGUILayout.GetControlRect(true, property_height);
            Draw(position, property, label, typeIdentifierRestriction);
        }

        internal static void Draw(Rect position, SerializedProperty property, GUIContent label, Type typeRestriction)
        {
            if (typeRestriction != null && !DynamicEnum.Helpers.TYPE_TO_IDENTIFIER.ContainsKey(typeRestriction))
                EditorGUI.HelpBox(position, $"Unrecognized type - {typeRestriction.FullName}.", MessageType.Error);
            else
                DrawInternal(position, property, label, typeRestriction, false);
        }

        internal static void Draw(Rect position, SerializedProperty property, GUIContent label, string typeIdentifierRestriction)
        {
            Type t = null;
            if (typeIdentifierRestriction != null && !DynamicEnum.Helpers.IDENTIFIER_TO_TYPE.TryGetValue(typeIdentifierRestriction, out t))
                EditorGUI.HelpBox(position, $"Unrecognized Identifier.", MessageType.Error);
            else
                DrawInternal(position, property, label, t, false);
        }

        internal static void DrawInternal(Rect position, SerializedProperty property, GUIContent label, Type typeRestriction, bool allowChangingType)
        {
            var typeIdentifierProperty = property.FindPropertyRelative("typeIdentifier");
            if (typeIdentifierProperty == null || typeIdentifierProperty.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.HelpBox(position, "Missing type identifier property.", MessageType.Error);
                return;
            }

            var valueProperty = property.FindPropertyRelative("value");
            if (valueProperty == null || valueProperty.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.HelpBox(position, "Missing value property.", MessageType.Error);
                return;
            }

            position.y += 1;
            position.height = 19f;
            position = EditorGUI.PrefixLabel(position, label);

            using (new IndentNullifier())
            {
                var buttonPosition = position;
                buttonPosition.width = 19f;

                position.x += 20f;
                position.width -= 20f;

                if (typeRestriction == null)
                    DynamicEnum.Helpers.IDENTIFIER_TO_TYPE.TryGetValue(typeIdentifierProperty.stringValue, out typeRestriction);

                using (new GUIEnabledChanger(GUI.enabled && allowChangingType))
                {
                    if (EditorGUI.DropdownButton(buttonPosition, GUIContent.none, FocusType.Passive))
                        GenerateMenu(typeIdentifierProperty).DropDown(buttonPosition);
                }

                if (typeRestriction == null) EditorGUI.PropertyField(position, valueProperty, GUIContent.none);
                else unsafe
                {
                    var currentValue = (byte) valueProperty.intValue;
                    GUIHelpers.DynamicEnumPopup(position, GUIContent.none, (IntPtr) (&currentValue), typeRestriction);
                    valueProperty.intValue = currentValue;
                }
            }
        }

        private static GenericMenu GenerateMenu(SerializedProperty property)
        {
            var currentValue = property.stringValue;
            var menu = new GenericMenu();

            menu.AddItem(GUIHelpers.ToGUIContent("None"),
                currentValue == "",
                () =>
                {
                    property.stringValue = "";
                    property.serializedObject.ApplyModifiedProperties();
                });

            if (currentValue != "" && !DynamicEnum.Helpers.IDENTIFIER_TO_TYPE.ContainsKey(currentValue))
                menu.AddDisabledItem(GUIHelpers.ToGUIContent($"Unknown ({currentValue})"), true);

            menu.AddSeparator("");

            foreach (var kvp in DynamicEnum.Helpers.IDENTIFIER_TO_TYPE)
            {
                var identifier = kvp.Key;
                var displayText = kvp.Value.FullName?.Replace('.', '/');

                menu.AddItem(GUIHelpers.ToGUIContent(displayText),
                    currentValue == identifier,
                    () =>
                    {
                        property.stringValue = identifier;
                        property.serializedObject.ApplyModifiedProperties();
                    });
            }

            return menu;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => property_height;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) =>
            DrawInternal(position, property, label, null, true);
    }
}