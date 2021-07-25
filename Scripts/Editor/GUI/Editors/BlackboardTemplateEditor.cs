using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HiraBots.Editor
{
    [CustomEditor(typeof(BlackboardTemplate))]
    internal class BlackboardTemplateEditor : UnityEditor.Editor
    {
        private const string parent_property = "parent";
        private const string keys_property = "keys";

        [SerializeField] private bool dirty = false;
        private MultiAssetFileHelper _multiAssetFileHelper = null;
        private ReorderableList _reorderableList = null;

        private void OnEnable() => Undo.undoRedoPerformed += OnUndoPerformed;

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoPerformed;
            _multiAssetFileHelper = null;
            _reorderableList = null;
        }

        private void OnUndoPerformed() => dirty = true;

        public override void OnInspectorGUI()
        {
            var parentProperty = serializedObject.FindProperty(parent_property);
            if (parentProperty == null || parentProperty.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUILayout.HelpBox("Could not find parent property.", MessageType.Error);
                return;
            }

            var keysProperty = serializedObject.FindProperty(keys_property);
            if (keysProperty == null || !keysProperty.isArray)
            {
                EditorGUILayout.HelpBox("Could not find keys property.", MessageType.Error);
                return;
            }

            InitializeIfRequired(keysProperty);

            var editingDisabled = EditorApplication.isPlayingOrWillChangePlaymode;

            if (editingDisabled)
                EditorGUILayout.HelpBox("HiraBots components are read-only while in play mode.", MessageType.Warning);

            if (dirty)
            {
                _multiAssetFileHelper.SynchronizeCollectionAndAsset();
                dirty = false;
            }

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

                var parent = parentProperty.objectReferenceValue;
                if (parent != null)
                {
                    var inheritedKeysLabel = EditorGUILayout.GetControlRect();
                    if (Event.current.type == EventType.Repaint)
                        ReorderableList.defaultBehaviours.headerBackground.Draw(inheritedKeysLabel,
                            false, false, false, false);

                    inheritedKeysLabel.x += 25;
                    inheritedKeysLabel.width -= 25;
                    EditorGUI.LabelField(inheritedKeysLabel, GUIHelpers.ToGUIContent("Inherited Keys"), EditorStyles.boldLabel);

                    using (new GUIEnabledChanger(false))
                        DrawKeysFor(new SerializedObject(parent));
                }

                _reorderableList.DoLayoutList();
            }
        }

        private void InitializeIfRequired(SerializedProperty keysProperty)
        {
            if (_multiAssetFileHelper == null)
                _multiAssetFileHelper = new MultiAssetFileHelper(target, serializedObject, keysProperty);

            if (_reorderableList == null)
            {
                _reorderableList = new ReorderableList(serializedObject, keysProperty, true, true, true, true)
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
                var property = serializedObject.FindProperty(keys_property).GetArrayElementAtIndex(index);
                var value = property.objectReferenceValue;

                rect.y -= 2;
                rect.height -= 2;

                rect.x += 20f;
                rect.width -= 20f;
                ReorderableList.defaultBehaviours.DrawElementBackground(rect, index, isActive, true, true);

                rect.x -= 20f;
                rect.width = 20f;
                EditorGUI.DrawRect(rect, BlackboardGUIHelpers.GetBlackboardKeyColor(value));
            }
        }

        private float GetKeyHeight(int index)
        {
            var property = serializedObject.FindProperty(keys_property).GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(property) + 4;
        }

        private void DrawSelfKey(Rect rect, int index, bool isActive, bool isFocused)
        {
            var property = serializedObject.FindProperty(keys_property).GetArrayElementAtIndex(index);
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
                menu.AddItem(GUIHelpers.ToGUIContent(BlackboardGUIHelpers.FORMATTED_NAMES[type]), false,
                    () => _multiAssetFileHelper.AddNewObject(type));
            }

            menu.ShowAsContext();
        }

        private void OnRemove(ReorderableList list) =>
            _multiAssetFileHelper.RemoveObject(list.index);

        private static bool CheckForCyclicalDependency(Object a)
        {
            var processedObjects = new List<Object>();

            do
            {
                if (processedObjects.Any(o => o == a)) return true;

                processedObjects.Add(a);
                a = new SerializedObject(a).FindProperty(parent_property).objectReferenceValue;
            } while (a != null);

            return false;
        }

        private static void DrawKeysFor(SerializedObject o)
        {
            o.Update();

            var parent = o.FindProperty(parent_property).objectReferenceValue;
            if (parent != null)
                DrawKeysFor(new SerializedObject(parent));

            var keysProperty = o.FindProperty(keys_property);

            var size = keysProperty.arraySize;

            for (var i = 0; i < size; i++)
                EditorGUILayout.PropertyField(keysProperty.GetArrayElementAtIndex(i));
        }
    }
}