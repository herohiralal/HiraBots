using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    /// <summary>
    /// Property drawer for a blackboard key selector.
    /// </summary>
    [CustomPropertyDrawer(typeof(BlackboardTemplate.KeySelector))]
    public class BlackboardKeySelectorPropertyDrawer : PropertyDrawer
    {
        private const string k_KeyPropertyName = "m_Key";
        private const string k_TemplatePropertyName = "m_Template";
        private const string k_KeyTypesPropertyName = "m_KeyTypes";
        private const string k_IsValidPropertyName = "m_IsValid";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 21f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 19f;

            var keyProperty = property.FindPropertyRelative(k_KeyPropertyName);
            var templateProperty = property.FindPropertyRelative(k_TemplatePropertyName);
            var keyTypesProperty = property.FindPropertyRelative(k_KeyTypesPropertyName);
            var isValidProperty = property.FindPropertyRelative(k_IsValidPropertyName);

            if (keyProperty == null || templateProperty == null || keyTypesProperty == null || isValidProperty == null)
            {
                EditorGUI.HelpBox(position, "Could not find one of the properties.", MessageType.Error);
                return;
            }

            position = EditorGUI.PrefixLabel(position, label);

            using (new IndentNullifier(0))
            {
                if (!isValidProperty.boolValue)
                {
                    var errorIconPosition = position;
                    errorIconPosition.width = 19f;
                    position.width -= 21f;
                    position.x += 21f;

                    var errorIcon = (Texture2D) EditorGUIUtility.Load("console.erroricon");
                    var errorMessage = new GUIContent(errorIcon, "Invalid selection.");
                    GUI.Label(errorIconPosition, errorMessage, EditorStyles.helpBox);
                }

                var currentEvent = Event.current;
                if (currentEvent.type == EventType.Repaint)
                {
                    // get a name
                    var currentKey = keyProperty.objectReferenceValue;
                    var name = currentKey == null ? "None" : currentKey.name;

                    // get control id
                    var controlID = GUIUtility.GetControlID("EditorPopup".GetHashCode(), FocusType.Keyboard, position);

                    // hover state
                    var hover = position.Contains(currentEvent.mousePosition);

                    EditorStyles.popup.Draw(position, new GUIContent(name), controlID, false, hover);
                }

                // actual dropdown
                if (EditorGUI.DropdownButton(position, GUIContent.none, FocusType.Passive, GUIStyle.none))
                {
                    GenerateMenu(
                            keyProperty,
                            templateProperty,
                            keyTypesProperty,
                            isValidProperty,
                            property.serializedObject.targetObjects)
                        .DropDown(position);
                }
            }
        }

        private static GenericMenu GenerateMenu(
            SerializedProperty keyProperty,
            SerializedProperty templateProperty,
            SerializedProperty keyTypesProperty,
            SerializedProperty isValidProperty,
            Object[] targetObjects)
        {
            var keyPropertyPath = keyProperty.propertyPath;
            var isValidPropertyPath = isValidProperty.propertyPath;

            var menu = new GenericMenu();

            var currentKey = (BlackboardKey) keyProperty.objectReferenceValue;
            var templateFilter = (BlackboardTemplate) templateProperty.objectReferenceValue;
            var typesFilter = (BlackboardKeyType) keyTypesProperty.intValue;

            menu.AddItem(GUIHelpers.ToGUIContent("None"),
                currentKey == null,
                () =>
                {
                    var so = new SerializedObject(targetObjects);
                    so.Update();
                    so.FindProperty(keyPropertyPath).objectReferenceValue = null;
                    so.FindProperty(isValidPropertyPath).boolValue = false;
                    so.ApplyModifiedProperties();
                    so.Dispose();
                });

            menu.AddSeparator("");

            var keysToShow = new List<(BlackboardKey key, string label)>();

            if (templateFilter == null)
            {
                // get all templates
                var templates = AssetDatabase
                    .FindAssets($"t:{typeof(BlackboardTemplate).FullName}")
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Select(AssetDatabase.LoadAssetAtPath<BlackboardTemplate>);

                foreach (var template in templates)
                {
                    keysToShow.AddRange(GetKeys(template, typesFilter, $"{template.name}/"));
                }
            }
            else
            {
                keysToShow.AddRange(GetKeys(templateFilter, typesFilter, ""));
            }

            var currentKeyIncluded = false;
            foreach (var (key, label) in keysToShow)
            {
                var labelContent = new GUIContent(label);

                // disabled item for current key
                if (key == currentKey)
                {
                    menu.AddDisabledItem(labelContent, true);
                    currentKeyIncluded = true;
                }
                // normally otherwise
                else
                {
                    menu.AddItem(labelContent,
                        false,
                        () =>
                        {
                            var so = new SerializedObject(targetObjects);
                            so.Update();
                            so.FindProperty(keyPropertyPath).objectReferenceValue = key;
                            so.FindProperty(isValidPropertyPath).boolValue = true;
                            so.ApplyModifiedProperties();
                            so.Dispose();
                        });
                }
            }

            // manually include the 
            if (!currentKeyIncluded)
            {
                menu.AddSeparator("");
                var currentKeyName = currentKey == null ? "None" : currentKey.name;
                menu.AddDisabledItem(new GUIContent($"{currentKeyName} [INVALID]"), true);
            }

            return menu;
        }

        private static IEnumerable<(BlackboardKey key, string label)> GetKeys(BlackboardTemplate template, BlackboardKeyType typesFilter,
            string categoryNameWithSlash)
        {
            // ignore if can't find parent properties
            if (!TryFindTemplateProperties(template, out var parentProperty, out var keysProperty))
            {
                yield break;
            }

            // include parent keys
            if (parentProperty.objectReferenceValue is BlackboardTemplate parentTemplate)
            {
                foreach (var inheritedKey in GetKeys(parentTemplate, typesFilter, categoryNameWithSlash))
                {
                    yield return inheritedKey;
                }
            }

            // check if the key fits the type filter
            var count = keysProperty.arraySize;
            for (var i = 0; i < count; i++)
            {
                var current = (BlackboardKey) keysProperty.GetArrayElementAtIndex(i).objectReferenceValue;

                if (typesFilter.HasFlag(current.keyType))
                {
                    yield return (current, categoryNameWithSlash + current.name);
                }
            }
        }

        // whether the existence of parent properties has been checked
        private static bool? s_CheckedParentPropertiesExistence = null;

        // try and get the serialized properties from the template
        private static bool TryFindTemplateProperties(Object template, out SerializedProperty parentProperty, out SerializedProperty keysProperty)
        {
            var so = new SerializedObject(template);
            parentProperty = so.FindProperty("m_Parent");
            keysProperty = so.FindProperty("m_Keys");

            // if already confirmed, just return true
            if (s_CheckedParentPropertiesExistence.HasValue)
            {
                return s_CheckedParentPropertiesExistence.Value;
            }

            // confirm it and return true
            if (keysProperty != null
                && keysProperty.isArray
                && parentProperty != null
                && parentProperty.propertyType == SerializedPropertyType.ObjectReference)
            {
                s_CheckedParentPropertiesExistence = true;
                return true;
            }

            // log otherwise
            Debug.LogError($"Could not find template properties.");

            s_CheckedParentPropertiesExistence = false;
            return false;
        }
    }
}