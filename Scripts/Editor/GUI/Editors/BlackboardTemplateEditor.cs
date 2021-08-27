using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HiraBots.Editor
{
    /// <summary>
    /// Custom editor for Blackboard Template.
    /// todo: implement this in UIElements (the current issue is that ListView has a predefined length for each element)
    /// </summary>
    [CustomEditor(typeof(BlackboardTemplate), true)]
    internal class BlackboardTemplateEditor : UnityEditor.Editor
    {
        [MenuItem("Assets/Create/HiraBots/Blackboard", false)]
        private static void CreateBlackboard()
        {
            AssetDatabaseUtility.CreateNewObject<BlackboardTemplate>("NewBlackboard");
        }

        [MenuItem("Assets/Create/HiraBots/Blackboard", true)]
        private static bool CanCreateBlackboard()
        {
            return !EditorApplication.isPlayingOrWillChangePlaymode;
        }

        // property names
        private const string k_ParentProperty = "m_Parent";
        private const string k_KeysProperty = "m_Keys";
        private const string k_BackendsProperty = "m_Backends";

        // undo helper
        [SerializeField] private bool m_Dirty = false;
        private ReorderableList m_ReorderableList = null;

        private SerializedProperty m_ParentProperty;
        private SerializedProperty m_KeysProperty;
        private SerializedProperty m_BackendsProperty;
        private string m_Errors = "";

        private void OnEnable()
        {
            // try find parent property
            m_ParentProperty = serializedObject.FindProperty(k_ParentProperty);
            if (m_ParentProperty == null || m_ParentProperty.propertyType != SerializedPropertyType.ObjectReference)
            {
                m_Errors += "\nCould not find parent property.";
            }

            // try find keys property
            m_KeysProperty = serializedObject.FindProperty(k_KeysProperty);
            if (m_KeysProperty == null || !m_KeysProperty.isArray)
            {
                m_Errors += "\nCould not find keys property.";
            }

            // try to find backends property
            m_BackendsProperty = serializedObject.FindProperty(k_BackendsProperty);
            if (m_BackendsProperty == null || m_BackendsProperty.propertyType != SerializedPropertyType.Enum)
            {
                m_Errors += "\nCould not find backends property.";
            }

            m_ReorderableList = new ReorderableList(serializedObject, m_KeysProperty,
                true, true, true, true)
            {
                drawHeaderCallback = DrawKeyListHeader,
                onAddDropdownCallback = OnAddDropdown,
                onRemoveCallback = OnRemove,
                elementHeightCallback = GetKeyHeight,
                drawElementCallback = DrawSelfKey,
                drawElementBackgroundCallback = DrawElementBackground
            };

            Undo.undoRedoPerformed += OnUndoPerformed;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoPerformed;
            m_ReorderableList = null;
            m_Errors = "";
            m_Dirty = false;
        }

        private void OnUndoPerformed()
        {
            m_Dirty = true;
        }

        public override void OnInspectorGUI()
        {
            if (m_Errors != "")
            {
                EditorGUILayout.HelpBox(m_Errors, MessageType.Error);
                return;
            }

            var editingDisabled = EditorApplication.isPlayingOrWillChangePlaymode;

            if (editingDisabled)
            {
                EditorGUILayout.HelpBox("Blackboard Templates are read-only while in play mode.", MessageType.Warning);
            }

            if (m_Dirty)
            {
                serializedObject.Update();
                var hs = new HashSet<Object>(
                    m_KeysProperty
                        .ToSerializedArrayProperty()
                        .Select(p => p.objectReferenceValue));

                AssetDatabaseUtility.SynchronizeFileToCompoundObject(target, hs);
                m_Dirty = false;
                serializedObject.ApplyModifiedProperties();
            }

            // parent property field
            using (new GUIEnabledChanger(!editingDisabled))
            {
                serializedObject.Update();

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_ParentProperty);
                if (EditorGUI.EndChangeCheck())
                {
                    using (new UndoMerger("Changed Blackboard Parent"))
                    {
                        serializedObject.ApplyModifiedProperties();
                        var newParent = m_ParentProperty.objectReferenceValue;
                        if (CheckForCyclicalDependency(target))
                        {
                            m_ParentProperty.objectReferenceValue = null;
                            Debug.LogError($"Cyclical dependency created in blackboard {target.name}. Removing parent.");
                        }
                        else if (newParent != null && CheckForCyclicalDependency(target))
                        {
                            m_ParentProperty.objectReferenceValue = null;
                            Debug.LogError($"Cyclical dependency created in blackboard {newParent.name}. Removing parent.");
                        }

                        serializedObject.ApplyModifiedProperties();
                    }
                }

                EditorGUILayout.PropertyField(m_BackendsProperty);
                serializedObject.ApplyModifiedProperties();

                // parent keys list
                var parent = m_ParentProperty.objectReferenceValue;

                if (parent != null)
                {
                    var selfBackends = m_BackendsProperty.intValue;
                    var parentSerializedObject = new SerializedObject(parent);
                    parentSerializedObject.Update();
                    var parentBackends = parentSerializedObject.FindProperty(k_BackendsProperty).intValue;
                    parentSerializedObject.Dispose();

                    if ((parentBackends & selfBackends) != selfBackends)
                    {
                        EditorGUILayout.HelpBox("The parent must contain all the backends" +
                                                " that this blackboard template requires.", MessageType.Error);
                    }
                }

                EditorGUILayout.Space();

                if (parent != null)
                {
                    var inheritedKeysLabel = EditorGUILayout.GetControlRect();
                    if (Event.current.type == EventType.Repaint)
                    {
                        ReorderableList.defaultBehaviours.headerBackground.Draw(inheritedKeysLabel,
                            false, false, false, false);
                    }

                    inheritedKeysLabel.x += 25;
                    inheritedKeysLabel.width -= 25;
                    EditorGUI.LabelField(inheritedKeysLabel, GUIHelpers.ToGUIContent("Inherited Keys"), EditorStyles.boldLabel);

                    using (new GUIEnabledChanger(false))
                    {
                        DrawKeysFor(new SerializedObject(parent));
                    }
                }

                // self keys list
                m_ReorderableList.DoLayoutList();
            }
        }

        private void DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index >= 0)
            {
                var value = m_KeysProperty.GetArrayElementAtIndex(index).objectReferenceValue;

                rect.y -= 2;
                rect.height -= 2;

                // default reorderable list background
                rect.x += 20f;
                rect.width -= 20f;
                ReorderableList.defaultBehaviours.DrawElementBackground(rect, index, isActive, true, true);

                // override a portion with a colour
                rect.x -= 20f;
                rect.width = 20f;
                EditorGUI.DrawRect(rect, BlackboardGUIHelpers.GetBlackboardKeyColor(value));
            }
        }

        private float GetKeyHeight(int index)
        {
            return EditorGUI.GetPropertyHeight(m_KeysProperty.GetArrayElementAtIndex(index)) + 4;
        }

        private void DrawSelfKey(Rect rect, int index, bool isActive, bool isFocused)
        {
            EditorGUI.PropertyField(rect, m_KeysProperty.GetArrayElementAtIndex(index), GUIContent.none, true);
        }

        private static void DrawKeyListHeader(Rect rect)
        {
            rect.x += 20;
            EditorGUI.LabelField(rect, "Self Keys", EditorStyles.boldLabel);
        }

        private void OnAddDropdown(Rect buttonRect, ReorderableList list)
        {
            var menu = new GenericMenu();

            foreach (var type in TypeCache.GetTypesDerivedFrom<BlackboardKey>().Where(t => !t.IsAbstract && !t.IsInterface))
            {
                menu.AddItem(GUIHelpers.ToGUIContent(BlackboardGUIHelpers.formattedNames[type]), false,
                    () => AssetDatabaseUtility.AddInlinedObject(target, serializedObject, m_KeysProperty, type));
            }

            menu.ShowAsContext();
        }

        private void OnRemove(ReorderableList list)
        {
            AssetDatabaseUtility.RemoveInlinedObject(target, serializedObject, m_KeysProperty, list.index);
        }

        // check for cyclical dependency created within the template
        private static bool CheckForCyclicalDependency(Object a)
        {
            var processedObjects = new List<Object>();

            do
            {
                if (processedObjects.Any(o => o == a))
                {
                    return true;
                }

                processedObjects.Add(a);
                a = new SerializedObject(a).FindProperty(k_ParentProperty).objectReferenceValue;
            } while (a != null);

            return false;
        }

        // draw keys for a specific object (used to draw all keys for each parent)
        private static void DrawKeysFor(SerializedObject o)
        {
            o.Update();

            var parent = o.FindProperty(k_ParentProperty).objectReferenceValue;
            if (parent != null)
            {
                // draw the keys for the parent before drawing them for self
                // this way, the order is better maintained
                DrawKeysFor(new SerializedObject(parent));
            }

            var keysProperty = o.FindProperty(k_KeysProperty);

            var size = keysProperty.arraySize;

            for (var i = 0; i < size; i++)
            {
                EditorGUILayout.PropertyField(keysProperty.GetArrayElementAtIndex(i));
            }
        }
    }
}