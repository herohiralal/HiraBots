using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HiraBots.Editor
{
    /// <summary>
    /// Helper functionality for IMGUI.
    /// </summary>
    [InitializeOnLoad]
    internal static class GUIHelpers
    {
        /// <summary>
        /// Initialize
        /// </summary>
        static GUIHelpers()
        {
            s_GUIContentCache = new Dictionary<string, GUIContent>();

            s_DynamicPopupMethodInfo = typeof(GUIHelpers)
                .GetMethod(
                    nameof(DynamicEnumPopupInternal),
                    BindingFlags.NonPublic | BindingFlags.Static,
                    null,
                    new[] { typeof(Rect), typeof(GUIContent), typeof(IntPtr) },
                    null);
        }

        // cache to convert string text to GUI, no need to repeatedly create it
        private static readonly Dictionary<string, GUIContent> s_GUIContentCache;

        // cache the DynamicPopup MethodInfo to not repeatedly search for it for every call
        private static readonly MethodInfo s_DynamicPopupMethodInfo;

        // get instance id's of all expanded elements
        // using SerializedProperty.isExpanded has its pitfalls, such as being shared between all instances
        // of a blackboard template, which means that on a blackboard template with a parent, if you expand
        // a key of index 3, every parent in its chain of hierarchy will get index 3 expanded, which will be
        // reflected in the parent keys section of the blackboard template.
        private static readonly Dictionary<int, bool> s_ExpansionStatus = new Dictionary<int, bool>(40);

        /// <summary>
        /// Get cached GUIContent for a label string.
        /// </summary>
        internal static GUIContent ToGUIContent(string value)
        {
            // return if one exists
            if (s_GUIContentCache.TryGetValue(value, out var content))
            {
                return content;
            }

            // create and cache otherwise
            content = new GUIContent(value);
            s_GUIContentCache.Add(value, content);
            return content;
        }

        /// <summary>
        /// Draw a dynamic enum popup using a reflected enum type.
        /// </summary>
        /// <param name="position">The position and size of the property.</param>
        /// <param name="label">The label to use.</param>
        /// <param name="value">The value as a reference.</param>
        /// <param name="enumType">The type of enum.</param>
        internal static void DynamicEnumPopup(Rect position, GUIContent label, IntPtr value, Type enumType)
        {
            // ignore if type is not enum.
            if (!enumType.IsEnum)
            {
                EditorGUI.HelpBox(position, "Dynamic Enum Popup used without an enum.", MessageType.Error);
                return;
            }

            // validate cached method info
            if (s_DynamicPopupMethodInfo == null || !s_DynamicPopupMethodInfo.IsGenericMethod)
            {
                EditorGUI.HelpBox(position, "Dynamic Enum Popup failed to find method using reflection.", MessageType.Error);
                return;
            }

            // resolve the generic type using reflection and invoke it.
            s_DynamicPopupMethodInfo
                .MakeGenericMethod(enumType)
                .Invoke(null, new object[] { position, label, value });
        }

        /// <summary>
        /// Internal generic method to draw a dynamic enum popup.
        /// </summary>
        /// <param name="position">The position and size of the property.</param>
        /// <param name="label">The label to use.</param>
        /// <param name="value">The value as a reference.</param>
        /// <typeparam name="T">The type of enum.</typeparam>
        private static unsafe void DynamicEnumPopupInternal<T>(Rect position, GUIContent label, IntPtr value) where T : unmanaged, Enum
        {
            // reinterpret the value
            var enumValue = *(T*) value;

            // check if the type is flags and draw the appropriate popup
            var enumOutput = typeof(T).GetCustomAttribute<FlagsAttribute>() == null
                ? (T) EditorGUI.EnumPopup(position, label, enumValue)
                : (T) EditorGUI.EnumFlagsField(position, label, enumValue);

            // store the value back
            *(T*) value = enumOutput;
        }

        internal static bool GetInlinedObjectReferenceExpansionStatus(int instanceID)
        {
            s_ExpansionStatus.TryGetValue(instanceID, out var value);
            return value;
        }

        /// <summary>
        /// Draw a header for inlined object references.
        /// </summary>
        /// <param name="position">The position and size of the property.</param>
        /// <param name="o">The value of the object reference.</param>
        /// <param name="getColor">Calculate background color.</param>
        /// <param name="getTypeName">Calculate a formatted type-name.</param>
        internal static bool DrawInlinedObjectReferenceHeader(Rect position, Object o, Func<Object, Color> getColor, Func<Object, string> getTypeName)
        {
            var instanceID = o.GetInstanceID();
            if (!s_ExpansionStatus.TryGetValue(instanceID, out var expanded))
            {
                s_ExpansionStatus.Add(instanceID, false);
                expanded = false;
            }

            var totalPos = position;
            totalPos.x -= 15f;
            totalPos.width += 15f;

            using (new GUIEnabledChanger(true)) // user expanding the header won't cause any problems
            {
                var e = Event.current;
                if (e.type == EventType.Repaint)
                {
                    EditorGUI.BeginFoldoutHeaderGroup(position, expanded, GUIContent.none);
                    EditorGUI.EndFoldoutHeaderGroup();

                    // background
                    position.height -= 2f;
                    EditorGUI.DrawRect(position, getColor(o));
                    position.height += 2f;

                    // name
                    position.x += 10f;
                    position = EditorGUI.PrefixLabel(position, ToGUIContent(o.name), EditorStyles.boldLabel);

                    // type
                    EditorGUI.LabelField(position, ToGUIContent(getTypeName(o)), EditorStyles.miniLabel);
                }
                else if (e.type == EventType.MouseDown && totalPos.Contains(e.mousePosition) && e.button == 0)
                {
                    expanded = !expanded;
                    GUI.changed = true;
                    e.Use();
                    // do not disable this, otherwise the moment a foldout opens, it'll be longer
                    // than the next element in a reorderable list
                }
                else if (e.type == EventType.KeyDown)
                {
                    var controlID = GUIUtility.GetControlID("FoldoutHeader".GetHashCode(), FocusType.Keyboard, totalPos);
                    if (GUIUtility.keyboardControl == controlID)
                    {
                        var keyCode = e.keyCode;
                        if ((keyCode == KeyCode.LeftArrow && expanded) || (keyCode == KeyCode.RightArrow && !expanded))
                        {
                            expanded = !expanded;
                            GUI.changed = true;
                            e.Use();
                        }
                    }
                }
            }

            // expansion
            s_ExpansionStatus[instanceID] = expanded;
            return expanded;
        }
    }

    /// <summary>
    /// Helper class for Blackboard GUI.
    /// </summary>
    internal static class BlackboardGUIHelpers
    {
        /// <summary>
        /// Format a Type.Name to a more readable version.
        /// </summary>
        internal static ReadOnlyDictionaryAccessor<Type, string> formattedNames { get; } = TypeCache
            .GetTypesDerivedFrom<BlackboardKey>()
            .ToDictionary(blackboardKeyType => blackboardKeyType,
                blackboardKeyType => blackboardKeyType.Name.Replace("Blackboard", " "))
            .ReadOnly();

        internal static string GetFormattedName(Object value)
        {
            return formattedNames[value.GetType()];
        }

        /// <summary>
        /// Resolve a theme colour for a blackboard key.
        /// </summary>
        /// <param name="value">The key object.</param>
        /// <returns>Its respective theme color.</returns>
        internal static Color GetBlackboardKeyColor(Object value)
        {
            if (!(value is BlackboardKey blackboardKey))
            {
                return Color.black;
            }

            switch (blackboardKey.keyType)
            {
                case BlackboardKeyType.Boolean:
                    return new Color(144f / 255, 0f / 255, 0f / 255);
                case BlackboardKeyType.Float:
                    return new Color(122f / 255, 195f / 255, 51f / 255);
                case BlackboardKeyType.Enum:
                    return new Color(0f / 255, 107f / 255, 97f / 255);
                case BlackboardKeyType.Integer:
                    return new Color(28f / 255, 220f / 255, 169f / 255);
                case BlackboardKeyType.Object:
                    return new Color(0f / 255, 166f / 255, 239f / 255);
                case BlackboardKeyType.Quaternion:
                    return new Color(159f / 255, 100f / 255, 198f / 255);
                case BlackboardKeyType.Vector:
                    return new Color(253f / 255, 201f / 255, 4f / 255);
                default:
                    return Color.black;
            }
        }

        /// <summary>
        /// Resolve a faded theme colour for a blackboard key.
        /// </summary>
        /// <param name="value">The key object.</param>
        /// <returns>Its respective faded theme color.</returns>
        internal static Color GetBlackboardKeyColorFaded(Object value)
        {
            return GetBlackboardKeyColor(value) * 0.35f;
        }
    }
}