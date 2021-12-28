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
            Undo.undoRedoPerformed += OnUndoPerformed;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoPerformed;
            BlackboardKeyInlinedObjectReferenceROLDrawer.Unbind(m_ReorderableList);
            m_ReorderableList = null;
            m_SerializedObject = null;
            m_Dirty = false;

            if (target != null)
            {
                InlinedObjectReferencesHelper.Collapse(target);
            }
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

                if (m_ReorderableList == null)
                {
                    m_ReorderableList = BlackboardKeyInlinedObjectReferenceROLDrawer.Bind(sbt);
                }

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
                    var oldParent = m_SerializedObject.parent.objectReferenceValue;
                    EditorGUILayout.PropertyField(m_SerializedObject.parent);
                    var newParent = m_SerializedObject.parent.objectReferenceValue;
                    m_SerializedObject.parent.objectReferenceValue = oldParent;
                    if (!ReferenceEquals(newParent, oldParent))
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
                            DrawReadOnlyHierarchyFor(m_SerializedObject);
                        }
                    }

                    // self keys list
                    EditorGUILayout.Space();
                    m_ReorderableList.DoLayoutList();
                }
            }
        }

        private static void DrawReadOnlyHierarchyFor(BlackboardTemplate.Serialized sbt)
        {
            var hierarchy = sbt.hierarchy;
            for (var i = hierarchy.count - 1; i >= 0; i--)
            {
                DrawHeaderAndReadOnlyKeysFor(hierarchy.count - i - 1, hierarchy[i]);
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
    }
}