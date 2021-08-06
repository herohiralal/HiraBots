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
                    new[] {typeof(Rect), typeof(GUIContent), typeof(IntPtr)},
                    null);
        }

        // cache to convert string text to GUI, no need to repeatedly create it
        private static readonly Dictionary<string, GUIContent> s_GUIContentCache;

        // cache the DynamicPopup MethodInfo to not repeatedly search for it for every call
        private static readonly MethodInfo s_DynamicPopupMethodInfo;

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
                .Invoke(null, new object[] {position, label, value});
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
    }

    /// <summary>
    /// Helper class for Blackboard GUI.
    /// </summary>
    internal static class BlackboardGUIHelpers
    {
        /// <summary>
        /// Format a Type.Name to a more readable version.
        /// </summary>
        internal static IReadOnlyDictionary<Type, string> formattedNames { get; } = TypeCache
            .GetTypesDerivedFrom<BlackboardKey>()
            .ToDictionary(blackboardKeyType => blackboardKeyType,
                blackboardKeyType => blackboardKeyType.Name.Replace("Blackboard", " "));

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
    }
}