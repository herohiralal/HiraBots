using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HiraBots.Editor
{
    internal static class GUIHelpers
    {
        private static readonly Dictionary<string, GUIContent> gui_content_cache = new Dictionary<string, GUIContent>();

        [InitializeOnLoadMethod]
        private static void Clear()
            => gui_content_cache.Clear();

        internal static GUIContent ToGUIContent(string value)
        {
            if (gui_content_cache.TryGetValue(value, out var content))
                return content;

            content = new GUIContent(value);
            gui_content_cache.Add(value, content);
            return content;
        }

        internal static void DrawNameField(Rect rect, Object value, string fieldName = "Name")
        {
            rect = EditorGUI.PrefixLabel(rect, ToGUIContent(fieldName));
            EditorGUI.BeginChangeCheck();
            var updatedName = EditorGUI.DelayedTextField(rect, GUIContent.none, value.name);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(value, $"Renamed {value.name}");
                value.name = updatedName;
            }
        }
    }

    internal static class BlackboardGUIHelpers
    {
        internal static readonly IReadOnlyDictionary<Type, string> FORMATTED_NAMES = TypeCache
            .GetTypesDerivedFrom<BlackboardKey>()
            .ToDictionary(blackboardKeyType => blackboardKeyType,
                blackboardKeyType => blackboardKeyType.Name.Replace("Blackboard", " "));

        internal static Color GetBlackboardKeyColor(Object value)
        {
            switch (value)
            {
                case BooleanBlackboardKey _:
                    return new Color(144f / 255, 0f / 255, 0f / 255);
                case FloatBlackboardKey _:
                    return new Color(122f / 255, 195f / 255, 51f / 255);
                // todo: enum key here
                //     return new Color(26f / 255, 122f / 255, 49f / 255);
                case IntegerBlackboardKey _:
                    return new Color(28f / 255, 220f / 255, 169f / 255);
                case ObjectBlackboardKey _:
                    return new Color(0f / 255, 166f / 255, 239f / 255);
                case QuaternionBlackboardKey _:
                    return new Color(159f / 255, 100f / 255, 198f / 255);
                case VectorBlackboardKey _:
                    return new Color(253f / 255, 201f / 255, 4f / 255);
                case null:
                    return Color.black;
                default:
                    return Color.black;
            }
        }
    }
}