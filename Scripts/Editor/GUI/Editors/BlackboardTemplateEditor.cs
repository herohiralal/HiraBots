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
        private const string k_BackendProperty = "m_Backends";

        // undo helper
        [SerializeField] private bool m_Dirty = false;
        private MultiAssetFileHelper m_MultiAssetFileHelper = null;
        private ReorderableList m_ReorderableList = null;

        private void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndoPerformed;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoPerformed;
            m_MultiAssetFileHelper = null;
            m_ReorderableList = null;
        }

        private void OnUndoPerformed()
        {
            m_Dirty = true;
        }

        public override void OnInspectorGUI()
        {
            // try find parent property
            var parentProperty = serializedObject.FindProperty(k_ParentProperty);
            if (parentProperty == null || parentProperty.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUILayout.HelpBox("Could not find parent property.", MessageType.Error);
                return;
            }

            // try find keys property
            var keysProperty = serializedObject.FindProperty(k_KeysProperty);
            if (keysProperty == null || !keysProperty.isArray)
            {
                EditorGUILayout.HelpBox("Could not find keys property.", MessageType.Error);
                return;
            }

            var backendsProperty = serializedObject.FindProperty(k_BackendProperty);
            if (backendsProperty == null || backendsProperty.propertyType != SerializedPropertyType.Enum)
            {
                EditorGUILayout.HelpBox("Could not find backends property.", MessageType.Error);
                return;
            }

            InitializeIfRequired(keysProperty);

            var editingDisabled = EditorApplication.isPlayingOrWillChangePlaymode;

            if (editingDisabled)
            {
                EditorGUILayout.HelpBox("Blackboard Templates are read-only while in play mode.", MessageType.Warning);
            }

            if (m_Dirty)
            {
                m_MultiAssetFileHelper.SynchronizeFileToCollection();
                m_Dirty = false;
            }

            // parent property field
            using (new GUIEnabledChanger(!editingDisabled))
            {
                serializedObject.Update();

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(parentProperty);
                if (EditorGUI.EndChangeCheck())
                {
                    using (new UndoMerger("Changed Blackboard Parent"))
                    {
                        serializedObject.ApplyModifiedProperties();
                        var newParent = parentProperty.objectReferenceValue;
                        if (CheckForCyclicalDependency(target))
                        {
                            parentProperty.objectReferenceValue = null;
                            Debug.LogError($"Cyclical dependency created in blackboard {target.name}. Removing parent.");
                        }
                        else if (newParent != null && CheckForCyclicalDependency(target))
                        {
                            parentProperty.objectReferenceValue = null;
                            Debug.LogError($"Cyclical dependency created in blackboard {newParent.name}. Removing parent.");
                        }

                        serializedObject.ApplyModifiedProperties();
                    }
                }

                EditorGUILayout.PropertyField(backendsProperty);
                serializedObject.ApplyModifiedProperties();

                // parent keys list
                var parent = parentProperty.objectReferenceValue;

                if (parent != null)
                {
                    var selfBackends = backendsProperty.intValue;
                    var parentSerializedObject = new SerializedObject(parent);
                    parentSerializedObject.Update();
                    var parentBackends = parentSerializedObject.FindProperty(k_BackendProperty).intValue;
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

        private void InitializeIfRequired(SerializedProperty keysProperty)
        {
            if (m_MultiAssetFileHelper == null)
            {
                m_MultiAssetFileHelper = new MultiAssetFileHelper(target, serializedObject, keysProperty);
            }

            if (m_ReorderableList == null)
            {
                m_ReorderableList = new ReorderableList(serializedObject, keysProperty, true, true, true, true)
                {
                    drawHeaderCallback = DrawKeyListHeader,
                    onAddDropdownCallback = OnAddDropdown,
                    onRemoveCallback = OnRemove,
                    elementHeightCallback = GetKeyHeight,
                    drawElementCallback = DrawSelfKey,
                    drawElementBackgroundCallback = DrawElementBackground
                };
            }
        }

        private void DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index >= 0)
            {
                var property = serializedObject.FindProperty(k_KeysProperty).GetArrayElementAtIndex(index);
                var value = property.objectReferenceValue;

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
            var property = serializedObject.FindProperty(k_KeysProperty).GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(property) + 4;
        }

        private void DrawSelfKey(Rect rect, int index, bool isActive, bool isFocused)
        {
            var property = serializedObject.FindProperty(k_KeysProperty).GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, property, GUIContent.none, true);
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
                    () => m_MultiAssetFileHelper.AddNewObject(type));
            }

            menu.ShowAsContext();
        }

        private void OnRemove(ReorderableList list)
        {
            m_MultiAssetFileHelper.RemoveObject(list.index);
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