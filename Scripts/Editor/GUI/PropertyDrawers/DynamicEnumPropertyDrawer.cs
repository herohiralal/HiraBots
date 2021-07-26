using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    [InitializeOnLoad]
    [CustomPropertyDrawer(typeof(DynamicEnum))]
    public class DynamicEnumPropertyDrawer : PropertyDrawer
    {
        static DynamicEnumPropertyDrawer()
        {
            var dynamicEnumTypes = TypeCache
                .GetTypesWithAttribute<ExposedToHiraBotsAttribute>()
                .Where(t => t.IsEnum && (t.GetEnumUnderlyingType() == typeof(byte) || t.GetEnumUnderlyingType() == typeof(sbyte)));

            var typeToIdentifier = new Dictionary<Type, string>();
            var identifierToType = new Dictionary<string, Type>();

            foreach (var type in dynamicEnumTypes)
            {
                var identifier = type.GetCustomAttribute<ExposedToHiraBotsAttribute>().Identifier;
                typeToIdentifier.Add(type, identifier);
                identifierToType.Add(identifier, type);
            }

            type_to_identifier = typeToIdentifier;
            identifier_to_type = identifierToType;
        }

        private static readonly ReadOnlyDictionaryAccessor<Type, string> type_to_identifier;
        private static readonly ReadOnlyDictionaryAccessor<string, Type> identifier_to_type;

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
            if (typeRestriction != null && !type_to_identifier.ContainsKey(typeRestriction))
                EditorGUI.HelpBox(position, $"Unrecognized type - {typeRestriction.FullName}.", MessageType.Error);
            else
                DrawInternal(position, property, label, typeRestriction, false);
        }

        internal static void Draw(Rect position, SerializedProperty property, GUIContent label, string typeIdentifierRestriction)
        {
            Type t = null;
            if (typeIdentifierRestriction != null && !identifier_to_type.TryGetValue(typeIdentifierRestriction, out t))
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
                    identifier_to_type.TryGetValue(typeIdentifierProperty.stringValue, out typeRestriction);

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

            if (currentValue != "" && !identifier_to_type.ContainsKey(currentValue))
                menu.AddDisabledItem(GUIHelpers.ToGUIContent($"Unknown ({currentValue})"), true);

            menu.AddSeparator("");

            foreach (var kvp in identifier_to_type)
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