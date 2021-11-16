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
            // only allow blackboard creation in edit mode
            return !EditorApplication.isPlayingOrWillChangePlaymode;
        }

        // undo helper
        [SerializeField] private bool m_Dirty = false;
        private BlackboardTemplate.Serialized m_SerializedObject = null;
        private ReorderableList m_ReorderableList = null;

        private void OnEnable()
        {
            m_ReorderableList = new ReorderableList(serializedObject, null,
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
            m_SerializedObject = null;
            m_Dirty = false;
            InlinedObjectReferencesHelper.Collapse(target);
        }

        private void OnUndoPerformed()
        {
            m_Dirty = true;
        }

        public override void OnInspectorGUI()
        {
            InlinedObjectReferencesHelper.Expand((BlackboardTemplate) target, out var cso);

            if (cso is BlackboardTemplate.Serialized sbt)
            {
                m_SerializedObject = sbt;

                if (m_SerializedObject.hasError)
                {
                    EditorGUILayout.HelpBox(m_SerializedObject.error, MessageType.Error);
                    return;
                }

                m_ReorderableList.serializedProperty = m_SerializedObject.keys;

                var editingAllowed = !EditorApplication.isPlayingOrWillChangePlaymode;

                if (!editingAllowed)
                {
                    EditorGUILayout.HelpBox("Blackboard Templates are read-only while in play mode.", MessageType.Warning);
                }

                if (m_Dirty)
                {
                    m_SerializedObject.Update();
                    var hs = new HashSet<Object>(
                        m_SerializedObject.keys
                            .ToSerializedArrayProperty()
                            .Select(p => p.objectReferenceValue));

                    AssetDatabaseUtility.SynchronizeFileToCompoundObject(target, hs);
                    m_Dirty = false;
                    m_SerializedObject.ApplyModifiedProperties();
                }

                using (new GUIEnabledChanger(editingAllowed))
                {
                    // backends property field
                    m_SerializedObject.Update();
                    EditorGUILayout.PropertyField(m_SerializedObject.backends);
                    m_SerializedObject.ApplyModifiedProperties();

                    // parent property field
                    m_SerializedObject.Update();
                    var newParent = EditorGUILayout.ObjectField(
                        GUIHelpers.TempContent(m_SerializedObject.parent.displayName,
                            m_SerializedObject.parent.tooltip),
                        m_SerializedObject.parent.objectReferenceValue,
                        typeof(BlackboardTemplate),
                        false);
                    if (!ReferenceEquals(newParent, m_SerializedObject.parent.objectReferenceValue))
                    {
                        if (m_SerializedObject.CanBeAssignedParent((BlackboardTemplate) newParent))
                        {
                            m_SerializedObject.parent.objectReferenceValue = newParent;
                        }
                        else
                        {
                            Debug.LogError($"Cyclical dependency created in blackboard {target.name}. Removing parent.");
                        }

                        m_SerializedObject.ApplyModifiedProperties();
                        m_SerializedObject.UpdateParentSerializedObjects();
                    }

                    var hasParent = m_SerializedObject.parent.objectReferenceValue != null;

                    // backends check
                    if (hasParent)
                    {
                        var selfBackends = (int) m_SerializedObject.target.backends;
                        var parentBackends = (int) m_SerializedObject.hierarchy[0].backends;

                        if ((parentBackends & selfBackends) != selfBackends)
                        {
                            EditorGUILayout.HelpBox("The parent must contain all the backends" +
                                                    " that this blackboard template requires.", MessageType.Error);
                        }
                    }

                    // parent keys list
                    if (hasParent)
                    {
                        EditorGUILayout.Space();

                        var inheritedKeysLabel = EditorGUILayout.GetControlRect();
                        if (Event.current.type == EventType.Repaint)
                        {
                            ReorderableList.defaultBehaviours.headerBackground.Draw(inheritedKeysLabel,
                                false, false, false, false);
                        }

                        inheritedKeysLabel.x += 25;
                        inheritedKeysLabel.width -= 25;
                        EditorGUI.LabelField(inheritedKeysLabel, GUIHelpers.TempContent("Inherited Keys"), EditorStyles.boldLabel);

                        using (new IndentNullifier(EditorGUI.indentLevel + 1))
                        {
                            DrawReadOnlyHierarchyFor(m_SerializedObject, false);
                        }
                    }

                    // self keys list
                    EditorGUILayout.Space();
                    m_ReorderableList.DoLayoutList();
                }
            }
        }

        internal static void DrawReadOnlyHierarchyFor(BlackboardTemplate.Serialized sbt, bool includeSelf)
        {
            var hierarchy = sbt.hierarchy;
            for (var i = hierarchy.count - 1; i >= 0; i--)
            {
                DrawHeaderAndReadOnlyKeysFor(hierarchy.count - i - 1, hierarchy[i]);
            }

            if (includeSelf)
            {
                DrawHeaderAndReadOnlyKeysFor(hierarchy.count, sbt.target);
            }
        }

        private static void DrawHeaderAndReadOnlyKeysFor(int index, BlackboardTemplate bt)
        {
            if (InlinedObjectReferencesHelper
                .DrawHeader(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect()),
                    bt,
                    BlackboardGUIHelpers.blackboardHeaderColor,
                    index.ToString(),
                    out var cso) && cso is BlackboardTemplate.Serialized sbt)
            {
                using (new GUIEnabledChanger(false))
                {
                    foreach (var key in sbt.keys.ToSerializedArrayProperty())
                    {
                        EditorGUILayout.PropertyField(key);
                    }
                }
            }
        }

        private void DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index >= 0)
            {
                var value = m_SerializedObject.keys.GetArrayElementAtIndex(index).objectReferenceValue as BlackboardKey;

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
            return EditorGUI.GetPropertyHeight(m_SerializedObject.keys.GetArrayElementAtIndex(index)) + 4;
        }

        private void DrawSelfKey(Rect rect, int index, bool isActive, bool isFocused)
        {
            EditorGUI.PropertyField(rect, m_SerializedObject.keys.GetArrayElementAtIndex(index), GUIContent.none, true);
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
                    () => AssetDatabaseUtility
                        .AddInlinedObject(m_SerializedObject.target,
                            (SerializedObject) m_SerializedObject,
                            m_SerializedObject.keys,
                            type));
            }

            menu.ShowAsContext();
        }

        private void OnRemove(ReorderableList list)
        {
            AssetDatabaseUtility
                .RemoveInlinedObject(m_SerializedObject.target,
                    (SerializedObject) m_SerializedObject,
                    m_SerializedObject.keys,
                    list.index);
        }
    }
}