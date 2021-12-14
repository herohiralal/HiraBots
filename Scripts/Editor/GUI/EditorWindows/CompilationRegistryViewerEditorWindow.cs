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
            if (EditorGUILayout.DropdownButton(GUIHelpers.TempContent(m_CurrentObject), FocusType.Keyboard, EditorStyles.popup, GUILayout.Width(200)))
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

            m_Scroller = EditorGUILayout.BeginScrollView(m_Scroller, true, true);

            if (CompilationRegistry.database.TryGetValue(m_CurrentObject, out var dataForCurrentObject))
            {
                var startAddress = dataForCurrentObject[0][0].startAddress;
                for (var currentDepth = 0; currentDepth < dataForCurrentObject.count; currentDepth++)
                {
                    var dataForCurrentDepth = dataForCurrentObject[currentDepth];

                    for (var i = 0; i < dataForCurrentDepth.count; i++)
                    {
                        var entry = dataForCurrentDepth[i];
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }
    }
}