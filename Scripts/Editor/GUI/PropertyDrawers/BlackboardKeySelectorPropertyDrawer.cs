using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HiraBots.Editor
{
    /// <summary>
    /// Property drawer for a blackboard key selector.
    /// </summary>
    [CustomPropertyDrawer(typeof(BlackboardTemplate.KeySelector))]
    internal class BlackboardKeySelectorPropertyDrawer : PropertyDrawer
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

                if (keyProperty.objectReferenceValue == null && templateProperty.objectReferenceValue is BlackboardTemplate template)
                {
                    var keyTypesFilter = (BlackboardKeyType) keyTypesProperty.intValue;

                    if (keyTypesFilter != BlackboardKeyType.Invalid)
                    {
                        var propertyName = label.text.Replace(" ", "");
                        var hs = new HashSet<BlackboardKey>();
                        template.GetKeySet(hs, true);
                        foreach (var key in hs)
                        {
                            var keyName = key.name.Replace(" ", "");
                            if (string.Equals(propertyName, keyName, System.StringComparison.InvariantCultureIgnoreCase)
                                && keyTypesFilter.HasFlag(key.keyType))
                            {
                                keyProperty.objectReferenceValue = key;
                                isValidProperty.boolValue = true;
                                keyProperty.serializedObject.ApplyModifiedProperties();
                                break;
                            }
                        }
                    }
                }

                var currentKey = keyProperty.objectReferenceValue;
                var currentKeyTextToDisplay = GUIHelpers.TempContent(currentKey == null ? "None" : currentKey.name);
                if (EditorGUI.DropdownButton(position, currentKeyTextToDisplay, FocusType.Keyboard, EditorStyles.popup))
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
            InlinedObjectReferencesHelper.Expand(template, out var so);

            if (!(so is BlackboardTemplate.Serialized cso) || cso.hasError)
            {
                yield break;
            }

            // include parent keys
            if (cso.parent.objectReferenceValue is BlackboardTemplate parentTemplate)
            {
                foreach (var inheritedKey in GetKeys(parentTemplate, typesFilter, categoryNameWithSlash))
                {
                    yield return inheritedKey;
                }
            }

            // check if the key fits the type filter
            var count = cso.keys.arraySize;
            for (var i = 0; i < count; i++)
            {
                var current = (BlackboardKey) cso.keys.GetArrayElementAtIndex(i).objectReferenceValue;

                if (typesFilter.HasFlag(current.keyType))
                {
                    yield return (current, categoryNameWithSlash + current.name);
                }
            }
        }
    }
}