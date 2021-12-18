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

        [SerializeField] private Vector2 m_ContentScroller;
        [SerializeField] private Vector2 m_InfoScroller;
        [NonSerialized] private string m_CurrentObject = k_SelectObject;
        [NonSerialized] private int m_CurrentDepth = 0;
        [NonSerialized] private int m_CurrentIndex = 0;

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
                var contentRect = new Rect(0, buttonRect.y + buttonRect.height + 10, position.width * 0.75f, position.height - buttonRect.height - 10);

                var startAddress = dataForCurrentObject[0][0].startAddress;
                var width = dataForCurrentObject[0][0].size * 20;
                var r = new Rect
                {
                    height = 18
                };

                var viewRect = new Rect(0, 0, width, dataForCurrentObject.count * 20);

                m_ContentScroller = GUI.BeginScrollView(contentRect, m_ContentScroller, viewRect);

                for (var currentDepth = 0; currentDepth < dataForCurrentObject.count; currentDepth++)
                {
                    var dataForCurrentDepth = dataForCurrentObject[currentDepth];
                    r.y = (currentDepth * 20) + 1;

                    for (var i = 0; i < dataForCurrentDepth.count; i++)
                    {
                        var entry = dataForCurrentDepth[i];
                        r.x = ((int) ((long) entry.startAddress - (long) startAddress) * 20) + 1;
                        r.width = (entry.size * 20) - 2;

                        if (GUI.Button(r, entry.name))
                        {
                            m_CurrentDepth = currentDepth;
                            m_CurrentIndex = i;
                        }
                    }
                }

                GUI.EndScrollView();

                var infoRect = new Rect(contentRect.x + contentRect.width + 10, contentRect.y, position.width - contentRect.width - 10, contentRect.height);

                viewRect = new Rect(0, 0, 500, 21 * 4);

                m_InfoScroller = GUI.BeginScrollView(infoRect, m_InfoScroller, viewRect);

                r.x = 0;
                r.y = 0;
                r.width = 500;
                r.height = 21;

                if (m_CurrentDepth < dataForCurrentObject.count)
                {
                    if (m_CurrentIndex < dataForCurrentObject[m_CurrentDepth].count)
                    {
                        var entry = dataForCurrentObject[m_CurrentDepth][m_CurrentIndex];

                        EditorGUI.LabelField(r, "Label", entry.name);
                        r.y += r.height;

                        EditorGUI.LabelField(r, "Start Address", entry.startAddress.ToString("X8"));
                        r.y += r.height;

                        EditorGUI.LabelField(r, "Last Address", entry.endAddress.ToString("X8"));
                        r.y += r.height;

                        EditorGUI.LabelField(r, "Size (in bytes)", entry.size.ToString());
                        r.y += r.height;
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