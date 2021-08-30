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
    /// Helper class to draw inlined Object references.
    /// </summary>
    internal class InlinedObjectReferencesHelper : ScriptableObject
    {
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            // get the singleton instance somehow
            var helpers = Resources.FindObjectsOfTypeAll<InlinedObjectReferencesHelper>();
            InlinedObjectReferencesHelper helper;
            switch (helpers.Length)
            {
                case 0:
                    helper = CreateInstance<InlinedObjectReferencesHelper>();
                    helper.hideFlags = HideFlags.HideAndDontSave & ~HideFlags.DontSaveInEditor;
                    break;
                case 1:
                    helper = helpers[0];
                    break;
                default:
                    helper = helpers[0];
                    for (var i = 1; i < helpers.Length; i++)
                    {
                        DestroyImmediate(helper);
                    }
                    break;
            }

            s_Instance = helper;
            s_SerializedObjectsForExpandedInlinedObjects = new Dictionary<Object, CustomSerializedObject>();

            // cache its serialized object representation
            for (var i = s_Instance.m_ExpandedInlinedObjects.Count - 1; i >= 0; i--)
            {
                var o = s_Instance.m_ExpandedInlinedObjects[i];

                if (o == null)
                {
                    s_Instance.m_ExpandedInlinedObjects.RemoveAt(i);
                    continue;
                }

                s_SerializedObjectsForExpandedInlinedObjects.Add(o, GetCustomSerializedObject(o));
            }
        }

        // using SerializedProperty.isExpanded has its pitfalls, such as being shared between all instances
        // of a blackboard template, which means that on a blackboard template with a parent, if you expand
        // a key of index 3, every parent in its chain of hierarchy will get index 3 expanded, which will be
        // reflected in the parent keys section of the blackboard template.
        private static Dictionary<Object, CustomSerializedObject> s_SerializedObjectsForExpandedInlinedObjects;
        private static InlinedObjectReferencesHelper s_Instance;

        [SerializeField] private List<Object> m_ExpandedInlinedObjects = new List<Object>(0);

        /// <summary>
        /// Check whether an inlined object reference is expanded.
        /// </summary>
        /// <param name="o">The value of the object reference.</param>
        internal static bool IsExpanded(Object o)
        {
            return s_SerializedObjectsForExpandedInlinedObjects.ContainsKey(o);
        }

        /// <summary>
        /// Check whether an inlined object reference is expanded.
        /// </summary>
        /// <param name="o">The value of the object reference.</param>
        /// <param name="so">The serialized object representation, if it is expanded.</param>
        internal static bool IsExpanded(Object o, out CustomSerializedObject so)
        {
            return s_SerializedObjectsForExpandedInlinedObjects.TryGetValue(o, out so);
        }

        /// <summary>
        /// Draw a header for inlined object references.
        /// </summary>
        /// <param name="position">The position and size of the property.</param>
        /// <param name="o">The value of the object reference.</param>
        /// <param name="theme">Background color.</param>
        /// <param name="subtitle">Subtitle to use.</param>
        /// <param name="so">The serialized object for an expanded object.</param>
        internal static bool DrawHeader<TObject>(Rect position, TObject o, Color theme, string subtitle, out CustomSerializedObject so)
            where TObject : Object
        {
            var expanded = IsExpanded(o, out so);

            var totalPos = position;
            totalPos.x -= 15f;
            totalPos.width += 15f;

            using (new GUIEnabledChanger(true))
            {
                if (EditorGUI.BeginFoldoutHeaderGroup(position, expanded, GUIContent.none) != expanded)
                {
                    expanded = !expanded;

                    if (expanded)
                    {
                        s_Instance.m_ExpandedInlinedObjects.Add(o);
                        so = GetCustomSerializedObject(o);
                        s_SerializedObjectsForExpandedInlinedObjects.Add(o, so);
                    }
                    else
                    {
                        s_SerializedObjectsForExpandedInlinedObjects[o].Dispose();
                        s_SerializedObjectsForExpandedInlinedObjects.Remove(o);
                        s_Instance.m_ExpandedInlinedObjects.Remove(o);
                    }
                }

                // background
                position.height -= 2f;
                EditorGUI.DrawRect(position, theme);
                position.height += 2f;
                    
                // name
                position.x += 10f;
                position = EditorGUI.PrefixLabel(position, GUIHelpers.ToGUIContent(o.name), EditorStyles.boldLabel);
                    
                // type
                EditorGUI.LabelField(position, GUIHelpers.ToGUIContent(subtitle), EditorStyles.miniLabel);

                EditorGUI.EndFoldoutHeaderGroup();
            }

            return expanded;
        }

        private static CustomSerializedObject GetCustomSerializedObject(Object o)
        {
            switch (o)
            {
                case null:
                    throw new ArgumentException("Cannot create a CustomSerializedObject from a null object.", nameof(o));
                case BlackboardTemplate c:
                    return new BlackboardTemplate.Serialized(c);
                case BlackboardKey c:
                    return new BlackboardKey.Serialized(c);
                default:
                    return new CustomSerializedObject(new SerializedObject(o));
            }
        }
    }

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
        internal static Color GetBlackboardKeyColor(BlackboardKey value)
        {
            switch (value.keyType)
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
        internal static Color GetBlackboardKeyColorFaded(BlackboardKey value)
        {
            Color.RGBToHSV(GetBlackboardKeyColor(value), out var h, out var s, out var v);
            s *= 0.35f;
            v = EditorGUIUtility.isProSkin ? 0.25f : 0.75f;
            return Color.HSVToRGB(h, s, v);
        }
    }
}