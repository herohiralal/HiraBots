using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HiraBots.Editor
{
    /// <summary>
    /// Custom property drawer for a DynamicEnum.
    /// It displays a dropdown to select which enum type should be used for the property drawer.
    /// todo: implement this in UIElements
    /// </summary>
    [CustomPropertyDrawer(typeof(DynamicEnum))]
    internal class DynamicEnumPropertyDrawer : PropertyDrawer
    {
        private const float k_PropertyHeight = 21f;

        // property names
        private const string k_TypeIdentifierProperty = "m_TypeIdentifier";
        private const string k_ValueProperty = "m_Value";

        // it's a single-line property, don't need much more than that
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return k_PropertyHeight;
        }

        // for a regular property drawer, don't need any type restrictions
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawInternal(position, property, label, null, true);
        }

        /// <summary>
        /// Draw a DynamicEnumPropertyDrawer with automatic layout and a type restriction for enum type selection.
        /// </summary>
        /// <param name="property">The target property.</param>
        /// <param name="label">The label for the property.</param>
        /// <param name="typeRestriction">A type restriction.</param>
        internal static void DrawWithAutomaticLayout(SerializedProperty property, GUIContent label, Type typeRestriction)
        {
            // just get the control rect and pass it on to manual layout method
            var position = EditorGUILayout.GetControlRect(true, k_PropertyHeight);
            Draw(position, property, label, typeRestriction);
        }

        /// <summary>
        /// Draw a DynamicEnumPropertyDrawer with automatic layout and a type restriction for enum type selection using the type identifier GUID.
        /// </summary>
        /// <param name="property">The target property.</param>
        /// <param name="label">The label for the property.</param>
        /// <param name="typeIdentifierRestriction">A type restriction using type identifier GUID.</param>
        internal static void DrawWithAutomaticLayout(SerializedProperty property, GUIContent label, string typeIdentifierRestriction)
        {
            // just get the control rect and pass it on to manual layout method
            var position = EditorGUILayout.GetControlRect(true, k_PropertyHeight);
            Draw(position, property, label, typeIdentifierRestriction);
        }

        /// <summary>
        /// Draw a DynamicEnumPropertyDrawer with manual layout and a type restriction for enum type selection.
        /// </summary>
        /// <param name="position">The position and size of the property.</param>
        /// <param name="property">The target property.</param>
        /// <param name="label">The label for the property.</param>
        /// <param name="typeRestriction">A type restriction.</param>
        internal static void Draw(Rect position, SerializedProperty property, GUIContent label, Type typeRestriction)
        {
            // validate type
            if (typeRestriction != null && !DynamicEnum.Helpers.typeToIdentifier.ContainsKey(typeRestriction))
            {
                EditorGUI.HelpBox(position, $"Unrecognized type - {typeRestriction.FullName}.", MessageType.Error);
            }
            else
            {
                DrawInternal(position, property, label, typeRestriction, false);
            }
        }

        /// <summary>
        /// Draw a DynamicEnumPropertyDrawer with manual layout and a type restriction for enum type selection using the type identifier GUID.
        /// </summary>
        /// <param name="position">The position and size of the property.</param>
        /// <param name="property">The target property.</param>
        /// <param name="label">The label for the property.</param>
        /// <param name="typeIdentifierRestriction">A type restriction using type identifier GUID.</param>
        internal static void Draw(Rect position, SerializedProperty property, GUIContent label, string typeIdentifierRestriction)
        {
            Type t = null;

            // try get type from the string
            if (typeIdentifierRestriction != null && !DynamicEnum.Helpers.identifierToType.TryGetValue(typeIdentifierRestriction, out t))
            {
                EditorGUI.HelpBox(position, $"Unrecognized Identifier.", MessageType.Error);
            }
            else
            {
                DrawInternal(position, property, label, t, false);
            }
        }

        /// <summary>
        /// The main method which draws the property.
        /// </summary>
        /// <param name="position">The position and size of the property.</param>
        /// <param name="property">The target property</param>
        /// <param name="label">The label for the property</param>
        /// <param name="typeRestriction">A type restriction, if any.</param>
        /// <param name="allowChangingType">Whether to disable the GUI for the dropdown button.</param>
        private static void DrawInternal(Rect position, SerializedProperty property, GUIContent label, Type typeRestriction, bool allowChangingType)
        {
            // try get type identifier property
            var typeIdentifierProperty = property.FindPropertyRelative(k_TypeIdentifierProperty);
            if (typeIdentifierProperty == null || typeIdentifierProperty.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.HelpBox(position, "Missing type identifier property.", MessageType.Error);
                return;
            }

            // try get value property
            var valueProperty = property.FindPropertyRelative(k_ValueProperty);
            if (valueProperty == null || valueProperty.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.HelpBox(position, "Missing value property.", MessageType.Error);
                return;
            }

            // prefix label
            position.y += 1;
            position.height = 19f;
            position = EditorGUI.PrefixLabel(position, label);

            // indent is handled by prefix label
            using (new IndentNullifier(0))
            {
                var buttonPosition = position;
                buttonPosition.width = 19f;

                position.x += 20f;
                position.width -= 20f;

                // if no type restriction was provided to the method, check if one already exists
                if (typeRestriction == null)
                {
                    DynamicEnum.Helpers.identifierToType.TryGetValue(typeIdentifierProperty.stringValue, out typeRestriction);
                }

                // disable GUI as per requirement
                using (new GUIEnabledChanger(GUI.enabled && allowChangingType))
                {
                    // type selection dropdown
                    if (EditorGUI.DropdownButton(buttonPosition, GUIContent.none, FocusType.Passive))
                    {
                        GenerateMenu(typeIdentifierProperty, property.serializedObject.targetObjects).DropDown(buttonPosition);
                    }
                }

                // draw a simple integer field if there's no valid type restriction
                if (typeRestriction == null)
                {
                    EditorGUI.PropertyField(position, valueProperty, GUIContent.none);
                }
                else unsafe
                {
                    // get the current value as byte (dynamic popups are hard-coded to be 8-bits)
                    var currentValue = (byte) valueProperty.intValue;

                    // pass its address as an IntPtr to helper function.
                    GUIHelpers.DynamicEnumPopup(position, GUIContent.none, (IntPtr) (&currentValue), typeRestriction);

                    // apply the value
                    valueProperty.intValue = currentValue;
                }
            }
        }

        /// <summary>
        /// Generate a dropdown menu for when the user presses the button to set enum type.
        /// </summary>
        /// <param name="property">The property to modify.</param>
        /// <param name="targetObjects">The target objects to update the values on.</param>
        private static GenericMenu GenerateMenu(SerializedProperty property, Object[] targetObjects)
        {
            var path = property.propertyPath;

            var currentValue = property.stringValue;
            var menu = new GenericMenu();

            // none option to clear the type string
            menu.AddItem(GUIHelpers.ToGUIContent("None"),
                currentValue == "",
                () =>
                {
                    var so = new SerializedObject(targetObjects);
                    so.Update();
                    so.FindProperty(path).stringValue = "";
                    so.ApplyModifiedProperties();
                    so.Dispose();
                });

            // display current value as unknown if it cannot be discerned
            if (currentValue != "" && !DynamicEnum.Helpers.identifierToType.ContainsKey(currentValue))
            {
                menu.AddDisabledItem(GUIHelpers.ToGUIContent($"Unknown ({currentValue})"), true);
            }

            // separator
            menu.AddSeparator("");

            foreach (var kvp in DynamicEnum.Helpers.identifierToType)
            {
                var identifier = kvp.Key;
                var displayText = kvp.Value.FullName?.Replace('.', '/'); // use namespaces as groups

                menu.AddItem(GUIHelpers.ToGUIContent(displayText),
                    currentValue == identifier,
                    () =>
                    {
                        var so = new SerializedObject(targetObjects);
                        so.Update();
                        so.FindProperty(path).stringValue = identifier;
                        so.ApplyModifiedProperties();
                        so.Dispose();
                    });
            }

            return menu;
        }
    }
}