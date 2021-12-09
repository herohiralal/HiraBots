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

        [MenuItem("HiraBots/Flush Inlined Objects Cache")]
        private static void FlushCache()
        {
            s_Instance.m_ExpandedInlinedObjects.Clear();
            s_SerializedObjectsForExpandedInlinedObjects.Clear();
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
        /// Expand an inlined object reference without drawing its header.
        /// </summary>
        /// <param name="o">The value of the object reference.</param>
        /// <param name="so">The serialized object for an expanded object.</param>
        internal static void Expand<TObject>(TObject o, out CustomSerializedObject so)
            where TObject : Object
        {
            if (IsExpanded(o, out so))
            {
                return;
            }

            s_Instance.m_ExpandedInlinedObjects.Add(o);
            so = GetCustomSerializedObject(o);
            s_SerializedObjectsForExpandedInlinedObjects.Add(o, so);
        }

        /// <summary>
        /// Collapse an inlined object reference without drawing its header.
        /// </summary>
        /// <param name="o">The value of the object reference.</param>
        internal static void Collapse(Object o)
        {
            if (!IsExpanded(o, out _))
            {
                return;
            }

            s_SerializedObjectsForExpandedInlinedObjects[o].Dispose();
            s_SerializedObjectsForExpandedInlinedObjects.Remove(o);
            s_Instance.m_ExpandedInlinedObjects.Remove(o);
        }

        /// <summary>
        /// Draw a header for inlined object references.
        /// </summary>
        /// <param name="position">The position and size of the property.</param>
        /// <param name="o">The value of the object reference.</param>
        /// <param name="theme">Background color.</param>
        /// <param name="subtitle">Subtitle to use.</param>
        /// <param name="so">The serialized object for an expanded object.</param>
        internal static bool DrawHeader<TObject>(Rect position, TObject o, Color? theme, string subtitle, out CustomSerializedObject so)
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
                        Expand(o, out so);
                    }
                    else
                    {
                        Collapse(o);
                    }
                }

                // background
                if (theme.HasValue)
                {
                    position.height -= 2f;
                    EditorGUI.DrawRect(position, theme.Value);
                    position.height += 2f;
                }

                // name
                position.x += 10f;
                position = EditorGUI.PrefixLabel(position, GUIHelpers.ToGUIContent(o.name), EditorStyles.boldLabel);

                // type
                if (subtitle != null)
                {
                    EditorGUI.LabelField(position, GUIHelpers.ToGUIContent(subtitle), EditorStyles.miniLabel);
                }

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

                case DecoratorBlackboardFunction c:
                    return new DecoratorBlackboardFunction.Serialized(c);
                case EffectorBlackboardFunction c:
                    return new EffectorBlackboardFunction.Serialized(c);

                case LGOAPGoal c:
                    return new LGOAPGoal.Serialized(c);
                case LGOAPTask c:
                    return new LGOAPTask.Serialized(c);

                case LGOAPDomain c:
                    return new LGOAPDomain.Serialized(c);

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

            s_TempContent = new GUIContent(null, null, null);

            s_DynamicPopupMethodInfo = typeof(GUIHelpers)
                .GetMethod(
                    nameof(DynamicEnumPopupInternal),
                    BindingFlags.NonPublic | BindingFlags.Static,
                    null,
                    new[] { typeof(Rect), typeof(GUIContent), typeof(IntPtr) },
                    null);

            s_EmptyStringHashSet = new HashSet<string>();
        }

        // cache to convert string text to GUI, no need to repeatedly create it
        private static readonly Dictionary<string, GUIContent> s_GUIContentCache;

        // cache the DynamicPopup MethodInfo to not repeatedly search for it for every call
        private static readonly MethodInfo s_DynamicPopupMethodInfo;

        // temp GUI content like Unity's internal implementation in EditorGUIUtility
        private static readonly GUIContent s_TempContent;

        // empty hash-set for use with auto property drawer
        private static readonly HashSet<string> s_EmptyStringHashSet;

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
        /// Get a temporary GUIContent to avoid allocations.
        /// </summary>
        internal static GUIContent TempContent(string text = null, string tooltip = null, Texture image = null)
        {
            s_TempContent.text = text;
            s_TempContent.image = image;
            s_TempContent.tooltip = tooltip;
            return s_TempContent;
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

        internal static void DrawDefaultPropertyDrawers(SerializedObject serializedObject, bool includeNameField = true, HashSet<string> propertiesToSkip = null)
        {
            var rect = EditorGUILayout.GetControlRect(true, GetTotalHeightForPropertyDrawers(serializedObject, includeNameField, propertiesToSkip));
            DrawDefaultPropertyDrawers(rect, serializedObject, includeNameField, propertiesToSkip);
        }

        internal static float GetTotalHeightForPropertyDrawers(SerializedObject serializedObject, bool includeNameField = true, HashSet<string> propertiesToSkip = null)
        {
            if (propertiesToSkip == null)
            {
                propertiesToSkip = s_EmptyStringHashSet;
            }

            var height = 0f;

            var iterator = serializedObject.GetIterator();
            iterator.NextVisible(true); // skip over "m_Script"
            iterator.Next(false); // "m_Name"
            if (includeNameField)
            {
                height += EditorGUI.GetPropertyHeight(iterator) + 2f;
            }

            iterator.Next(false); // skip over "m_EditorClassIdentifier"

            while (iterator.Next(false))
            {
                if (propertiesToSkip.Contains(iterator.name))
                {
                    continue;
                }

                height += EditorGUI.GetPropertyHeight(iterator) + 2f;
            }

            return height;
        }

        internal static void DrawDefaultPropertyDrawers(Rect rect, SerializedObject serializedObject, bool includeNameField = true, HashSet<string> propertiesToSkip = null)
        {
            if (propertiesToSkip == null)
            {
                propertiesToSkip = s_EmptyStringHashSet;
            }

            serializedObject.Update();

            var iterator = serializedObject.GetIterator();
            iterator.NextVisible(true); // skip over "m_Script"
            iterator.Next(false); // "m_Name"
            if (includeNameField)
            {
                rect.height = EditorGUI.GetPropertyHeight(iterator);

                EditorGUI.PropertyField(rect, iterator, true);

                rect.y += rect.height + 2f;
            }

            iterator.Next(false); // skip over "m_EditorClassIdentifier"

            while (iterator.Next(false))
            {
                if (propertiesToSkip.Contains(iterator.name))
                {
                    continue;
                }

                rect.height = EditorGUI.GetPropertyHeight(iterator);

                EditorGUI.PropertyField(rect, iterator, true);

                rect.y += rect.height + 2f;
            }

            serializedObject.ApplyModifiedProperties();
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
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .ToDictionary(blackboardKeyType => blackboardKeyType,
                blackboardKeyType => blackboardKeyType.Name
                    .Replace("Blackboard", " "))
            .ReadOnly();

        /// <summary>
        /// Resolve a theme colour for a blackboard.
        /// </summary>
        internal static Color blackboardHeaderColor
        {
            get
            {
                var c = new Color(80f / 255, 61f / 255, 66f / 255);
                Color.RGBToHSV(c, out var h, out var s, out var v);
                v = EditorGUIUtility.isProSkin ? 0.25f : 0.75f;
                return Color.HSVToRGB(h, s, v);
            }
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

    /// <summary>
    /// Helper class for Blackboard Function GUI.
    /// </summary>
    internal static class BlackboardFunctionGUIHelpers
    {
        /// <summary>
        /// Format a Type.Name to a more readable version.
        /// </summary>
        internal static ReadOnlyDictionaryAccessor<Type, string> formattedDecoratorNames { get; } = TypeCache
            .GetTypesDerivedFrom<DecoratorBlackboardFunction>()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .ToDictionary(f => f,
                f => f.Name
                    .Replace("DecoratorBlackboardFunction", ""))
            .ReadOnly();

        /// <summary>
        /// Format a Type.Name to a more readable version.
        /// </summary>
        internal static ReadOnlyDictionaryAccessor<Type, string> formattedScoreCalculatorNames { get; } = TypeCache
            .GetTypesDerivedFrom<DecoratorBlackboardFunction>()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .ToDictionary(f => f,
                f => f.Name
                    .Replace("DecoratorBlackboardFunction", "")
                    .Replace("AlwaysSucceed", "BaseScore"))
            .ReadOnly();

        /// <summary>
        /// Format a Type.Name to a more readable version.
        /// </summary>
        internal static ReadOnlyDictionaryAccessor<Type, string> formattedEffectorNames { get; } = TypeCache
            .GetTypesDerivedFrom<EffectorBlackboardFunction>()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .ToDictionary(f => f,
                f => f.Name
                    .Replace("EffectorBlackboardFunction", ""))
            .ReadOnly();

        private const string k_Target = "Target";
        internal const string k_GoalInsistence = "Insistence";
        internal const string k_GoalTarget = k_Target;
        internal const string k_TaskPrecondition = "Precondition";
        internal const string k_TaskCost = "Cost";
        internal const string k_TaskEffect = "Effect";
        internal const string k_TaskTarget = k_Target;

        /// <summary>
        /// Resolve a theme colour for a blackboard function.
        /// </summary>
        /// <param name="value">The function object.</param>
        /// <returns>Its respective theme color.</returns>
        internal static Color GetBlackboardFunctionColor(BlackboardFunction value)
        {
            switch (value.subtitle)
            {
                case k_GoalInsistence:
                    return new Color(203f / 255, 232f / 255, 150f / 255);
                case k_TaskPrecondition:
                    return new Color(0f / 255, 62f / 255, 160f / 255);
                case k_TaskCost:
                    return new Color(238f / 255, 150f / 255, 75f / 255);
                case k_TaskEffect:
                    return new Color(148f / 255, 201f / 255, 169f /255);
                case k_Target:
                    return new Color(239f / 255, 71f / 255, 111f / 255);
                default:
                    return Color.black;
            }
        }

        /// <summary>
        /// Resolve a faded theme colour for a blackboard function.
        /// </summary>
        /// <param name="value">The function object.</param>
        /// <returns>Its respective faded theme color.</returns>
        internal static Color GetBlackboardFunctionColorFaded(BlackboardFunction value)
        {
            Color.RGBToHSV(GetBlackboardFunctionColor(value), out var h, out var s, out var v);
            s *= 0.35f;
            v = EditorGUIUtility.isProSkin ? 0.25f : 0.75f;
            return Color.HSVToRGB(h, s, v);
        }
    }

    /// <summary>
    /// Helper class for LGOAP Domain GUI.
    /// </summary>
    internal static class LGOAPDomainGUIHelpers
    {
        /// <summary>
        /// Resolve a theme colour for an LGOAP domain component
        /// </summary>
        /// <param name="value">The component object.</param>
        /// <returns>Its respective theme color.</returns>
        internal static Color GetComponentColor(ScriptableObject value)
        {
            switch (value)
            {
                case LGOAPGoal _:
                    return new Color(117f / 255, 91f / 255, 82f / 255);
                case LGOAPTask c when c.isAbstract:
                    return new Color(53f / 255, 66f / 255, 143f / 255);
                case LGOAPTask c when !c.isAbstract:
                    return new Color(115f / 255, 26f / 255, 76f / 255);
                default:
                    return Color.black;
            }
        }

        /// <summary>
        /// Resolve a theme colour for a faded LGOAP domain component
        /// </summary>
        /// <param name="value">The component object.</param>
        /// <returns>Its respective faded theme color.</returns>
        internal static Color GetComponentColorFaded(ScriptableObject value)
        {
            Color.RGBToHSV(GetComponentColor(value), out var h, out var s, out var v);
            s *= 0.35f;
            v = EditorGUIUtility.isProSkin ? 0.25f : 0.75f;
            return Color.HSVToRGB(h, s, v);
        }
    }
}