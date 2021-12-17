using System;
using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    internal class CompilationRegistryViewerEditorWindow : EditorWindow
    {
        [MenuItem("HiraBots/Compilation Registry Viewer")]
        internal static void Open()
        {
            GetWindow<CompilationRegistryViewerEditorWindow>().Show();
        }

        private const string k_SelectObject = "Select Object";

        [SerializeField] private Vector2 m_Scroller;
        [NonSerialized] private string m_CurrentObject = k_SelectObject;

        private void OnGUI()
        {
            var buttonRect = new Rect(0, 0, 200, 21);

            if (EditorGUI.DropdownButton(buttonRect, GUIHelpers.TempContent(m_CurrentObject), FocusType.Keyboard, EditorStyles.popup))
            {
                var m = new GenericMenu();

                m.AddItem(GUIHelpers.ToGUIContent("None"), m_CurrentObject == k_SelectObject, () => m_CurrentObject = k_SelectObject);

                foreach (var keyValuePair in CompilationRegistry.database)
                {
                    var currentObject = keyValuePair.Key;
                    m.AddItem(GUIHelpers.ToGUIContent(currentObject), m_CurrentObject == currentObject, () => m_CurrentObject = currentObject);
                }

                m.ShowAsContext();
            }

            if (CompilationRegistry.database.TryGetValue(m_CurrentObject, out var dataForCurrentObject))
            {
                var contentRect = new Rect(0, buttonRect.y + buttonRect.height, position.width * 0.66f, position.height - buttonRect.height);

                var startAddress = dataForCurrentObject[0][0].startAddress;
                var width = dataForCurrentObject[0][0].size * 20;
                var r = new Rect
                {
                    height = 18
                };

                var viewRect = new Rect(0, 0, width, dataForCurrentObject.count * 20);

                m_Scroller = GUI.BeginScrollView(contentRect, m_Scroller, viewRect);

                for (var currentDepth = 0; currentDepth < dataForCurrentObject.count; currentDepth++)
                {
                    var dataForCurrentDepth = dataForCurrentObject[currentDepth];
                    r.y = (currentDepth * 20) + 1;

                    for (var i = 0; i < dataForCurrentDepth.count; i++)
                    {
                        var entry = dataForCurrentDepth[i];
                        r.x = ((int) ((long) entry.startAddress - (long) startAddress) * 20) + 1;
                        r.width = (entry.size * 20) - 2;

                        EditorGUI.DrawRect(r, Color.black);
                        EditorGUI.LabelField(r, entry.name);
                    }
                }

                GUI.EndScrollView();
            }
            else
            {
                m_CurrentObject = k_SelectObject;
            }
        }
    }
}