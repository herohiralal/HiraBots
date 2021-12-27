using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HiraBots.Editor
{
    /// <summary>
    /// Custom editor for an LGOAP Domain.
    /// todo: implement this in UIElements (the current issue is that ListView has a predefined length for each element)
    /// </summary>
    [CustomEditor(typeof(LGOAPDomain), true)]
    internal class LGOAPDomainEditor : UnityEditor.Editor
    {
        [MenuItem("Assets/Create/HiraBots/LGOAP Domain", false)]
        private static void CreateLGOAPDomain()
        {
            AssetDatabaseUtility.CreateNewObject<LGOAPDomain>("NewDomain");
        }

        [MenuItem("Assets/Create/HiraBots/LGOAP Domain", true)]
        private static bool CanCreateLGOAPDomain()
        {
            // only allow domain creation in edit mode
            return !EditorApplication.isPlayingOrWillChangePlaymode;
        }

        // undo helper
        [SerializeField] private bool m_Dirty = false;
        private LGOAPDomain.Serialized m_SerializedObject = null;
        private ReorderableList m_TopLayer = null;
        private ReorderableList m_TopLayerFallback = null;
        private ReorderableList[] m_IntermediateLayers = null;
        private ReorderableList[] m_IntermediateLayersFallbacks = null;
        private ReorderableList m_BottomLayer = null;
        private ReorderableList m_BottomLayerFallback = null;

        private void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndoPerformed;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoPerformed;

            LGOAPGoalROLDrawer.Unbind(m_TopLayer);
            FallbackPlanDrawer.Unbind(m_TopLayerFallback);

            if (m_IntermediateLayers != null)
            {
                foreach (var l in m_IntermediateLayers)
                {
                    LGOAPAbstractTaskROLDrawer.Unbind(l);
                }
            }

            if (m_IntermediateLayersFallbacks != null)
            {
                foreach (var l in m_IntermediateLayersFallbacks)
                {
                    FallbackPlanDrawer.Unbind(l);
                }
            }

            LGOAPTaskROLDrawer.Unbind(m_BottomLayer);
            FallbackPlanDrawer.Unbind(m_BottomLayerFallback);

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
            InlinedObjectReferencesHelper.Expand((LGOAPDomain) target, out var cso);

            if (cso is LGOAPDomain.Serialized domain)
            {
                m_SerializedObject = domain;

                if (m_SerializedObject.hasError)
                {
                    EditorGUILayout.HelpBox(m_SerializedObject.error, MessageType.Error);
                    return;
                }

                if (m_TopLayer == null)
                {
                    m_TopLayer = LGOAPGoalROLDrawer.Bind(domain);
                }

                if (m_TopLayerFallback == null)
                {
                    m_TopLayerFallback = FallbackPlanDrawer.BindTopLayer(domain);
                }

                if (m_IntermediateLayers == null)
                {
                    m_IntermediateLayers = LGOAPAbstractTaskROLDrawer.Bind(domain);
                }

                if (m_IntermediateLayersFallbacks == null)
                {
                    m_IntermediateLayersFallbacks = FallbackPlanDrawer.BindIntermediateLayers(domain);
                }

                if (m_BottomLayer == null)
                {
                    m_BottomLayer = LGOAPTaskROLDrawer.Bind(domain);
                }

                if (m_BottomLayerFallback == null)
                {
                    m_BottomLayerFallback = FallbackPlanDrawer.BindBottomLayer(domain);
                }

                var editingAllowed = !EditorApplication.isPlayingOrWillChangePlaymode;

                if (!editingAllowed)
                {
                    EditorGUILayout.HelpBox("LGOAP Domains are read-only while in play mode.", MessageType.Warning);
                }

                if (m_Dirty)
                {
                    m_SerializedObject.Update();
                    AssetDatabaseUtility.SynchronizeFileToCompoundObject(target, subAssetsThatMustBeInFile);
                    m_Dirty = false;
                    m_SerializedObject.ApplyModifiedProperties();
                    m_SerializedObject.OnBlackboardUpdate();
                }

                using (new GUIEnabledChanger(editingAllowed))
                {
                    // backends property field
                    m_SerializedObject.Update();
                    EditorGUILayout.PropertyField(m_SerializedObject.backends);
                    m_SerializedObject.ApplyModifiedProperties();

                    // intermediate layers count field
                    m_SerializedObject.Update();
                    m_SerializedObject.intermediateLayersCount = EditorGUILayout.IntSlider(
                        GUIHelpers.TempContent("Intermediate Layers Count", "The number of intermediate layers in this domain."),
                        m_SerializedObject.intermediateLayersCount, 0, 2);
                    if (m_SerializedObject.ApplyModifiedProperties())
                    {
                        m_Dirty = true;
                        LGOAPAbstractTaskROLDrawer.Rebind(ref m_IntermediateLayers, domain);
                        FallbackPlanDrawer.RebindIntermediateLayers(ref m_IntermediateLayersFallbacks, domain);
                    }

                    // blackboard property field
                    m_SerializedObject.Update();
                    EditorGUILayout.PropertyField(m_SerializedObject.blackboard);
                    if (m_SerializedObject.ApplyModifiedProperties())
                    {
                        m_SerializedObject.OnBlackboardUpdate();
                    }

                    var currentBlackboard = m_SerializedObject.blackboard.objectReferenceValue as BlackboardTemplate;
                    if (currentBlackboard == null)
                    {
                        EditorGUILayout.HelpBox("Assign a blackboard template to modify AI behaviour.", MessageType.Info);
                        return;
                    }

                    var selfBackends = (int) m_SerializedObject.target.backends;
                    var blackboardBackends = (int) ((BlackboardTemplate) m_SerializedObject.blackboard.objectReferenceValue).backends;

                    if ((blackboardBackends & selfBackends) != selfBackends)
                    {
                        EditorGUILayout.HelpBox("The assigned blackboard template must contain" +
                                                " all the backends that this LGOAP domain requires.", MessageType.Error);
                    }

                    EditorGUILayout.Space();

                    using (new GUIEnabledChanger(false))
                    {
                        DrawPlanSizeProperty(m_SerializedObject.topLayerMaxPlanSize);
                    }

                    m_TopLayer.DoLayoutList();

                    m_SerializedObject.Update();
                    m_TopLayerFallback.DoLayoutList();
                    m_SerializedObject.ApplyModifiedProperties();

                    EditorGUILayout.Space();

                    for (var i = 0; i < m_IntermediateLayers.Length; i++)
                    {
                        DrawPlanSizeProperty(m_SerializedObject.intermediateLayerMaxPlanSizes.GetArrayElementAtIndex(i));

                        m_IntermediateLayers[i].DoLayoutList();

                        m_SerializedObject.Update();
                        m_IntermediateLayersFallbacks[i].DoLayoutList();
                        m_SerializedObject.ApplyModifiedProperties();

                        EditorGUILayout.Space();
                    }

                    DrawPlanSizeProperty(m_SerializedObject.bottomLayerMaxPlanSize);

                    m_BottomLayer.DoLayoutList();

                    m_SerializedObject.Update();
                    m_BottomLayerFallback.DoLayoutList();
                    m_SerializedObject.ApplyModifiedProperties();

                    EditorGUILayout.Space();
                }
            }
        }

        private void DrawPlanSizeProperty(SerializedProperty maxPlanSizeProperty)
        {
            m_SerializedObject.Update();

            EditorGUILayout.IntSlider(maxPlanSizeProperty, 1, 10, "Max Plan Size");

            m_SerializedObject.ApplyModifiedProperties();
        }

        private HashSet<Object> subAssetsThatMustBeInFile
        {
            get
            {
                var hs = new HashSet<Object>();

                foreach (var goal in m_SerializedObject.topLayer.ToSerializedArrayProperty().Select(p => (LGOAPGoal) p.objectReferenceValue))
                {
                    hs.Add(goal);

                    foreach (var s in goal.insistence.m_Insistence)
                    {
                        hs.Add(s);
                    }

                    foreach (var d in goal.target.m_Target)
                    {
                        hs.Add(d);
                    }
                }

                foreach (var intermediateLayer in m_SerializedObject.intermediateLayers)
                {
                    foreach (var task in intermediateLayer.ToSerializedArrayProperty().Select(p => (LGOAPTask) p.objectReferenceValue))
                    {
                        hs.Add(task);

                        foreach (var d in task.action.m_Precondition)
                        {
                            hs.Add(d);
                        }

                        foreach (var s in task.action.m_Cost)
                        {
                            hs.Add(s);
                        }

                        foreach (var e in task.action.m_Effect)
                        {
                            hs.Add(e);
                        }

                        foreach (var d in task.target.m_Target)
                        {
                            hs.Add(d);
                        }

                        foreach (var t in task.taskProviders)
                        {
                            hs.Add(t);
                        }

                        foreach (var s in task.serviceProviders)
                        {
                            hs.Add(s);
                        }
                    }
                }

                foreach (var task in m_SerializedObject.bottomLayer.ToSerializedArrayProperty().Select(p => (LGOAPTask) p.objectReferenceValue))
                {
                    hs.Add(task);

                    foreach (var d in task.action.m_Precondition)
                    {
                        hs.Add(d);
                    }

                    foreach (var s in task.action.m_Cost)
                    {
                        hs.Add(s);
                    }

                    foreach (var e in task.action.m_Effect)
                    {
                        hs.Add(e);
                    }

                    foreach (var d in task.target.m_Target)
                    {
                        hs.Add(d);
                    }

                    foreach (var t in task.taskProviders)
                    {
                        hs.Add(t);
                    }

                    foreach (var s in task.serviceProviders)
                    {
                        hs.Add(s);
                    }
                }

                return hs;
            }
        }

        private static class FallbackPlanDrawer
        {
            private static void Bind(ReorderableList rol, SerializedProperty planProperty, SerializedProperty containerListProperty)
            {
                rol.drawHeaderCallback = r =>
                {
                    r.x += 20f;
                    EditorGUI.LabelField(r, "Fallback Plan");
                };

                rol.elementHeight = 23f;

                rol.drawElementBackgroundCallback = (r, i, a, f) =>
                {
                    if (i < 0)
                    {
                        return;
                    }

                    // default background
                    ReorderableList.defaultBehaviours.DrawElementBackground(r, i, a, true, true);
                };

                rol.drawElementCallback = (r, i, a, f) =>
                {
                    r.y += 2;
                    r.height -= 4;
                    r.x += 2;
                    r.width -= 4;

                    var currentIndex = planProperty.GetArrayElementAtIndex(i).intValue;

                    string label;
                    if (currentIndex < 0 || currentIndex >= containerListProperty.arraySize)
                    {
                        var width = r.width;
                        r.width = width * 0.78f;

                        var r2 = r;
                        r2.x += width * 0.82f;
                        r2.width = width * 0.18f;
                        EditorGUI.HelpBox(r2, "Invalid.", MessageType.Error);

                        label = $"UNKNOWN [{currentIndex}]";
                    }
                    else
                    {
                        var currentContainer = containerListProperty.GetArrayElementAtIndex(currentIndex).objectReferenceValue;
                        label = currentContainer == null ? $"NULL [{currentIndex}]" : $"{currentContainer.name} [{currentIndex}]";
                    }

                    if (EditorGUI.DropdownButton(r, GUIHelpers.TempContent(label), FocusType.Keyboard, EditorStyles.popup))
                    {
                        var menu = new GenericMenu();

                        for (var j = 0; j < containerListProperty.arraySize; j++)
                        {
                            var currentContainerIndex = j;
                            var currentContainer = containerListProperty.GetArrayElementAtIndex(j).objectReferenceValue;

                            var guiContent = new GUIContent(currentContainer == null ? $"NULL [{currentIndex}]" : $"{currentContainer.name} [{currentIndex}]");
                            menu.AddItem(guiContent, currentIndex == j,
                                () =>
                                {
                                    planProperty.serializedObject.Update();
                                    planProperty.GetArrayElementAtIndex(i).intValue = currentContainerIndex;
                                    planProperty.serializedObject.ApplyModifiedProperties();
                                });
                        }

                        menu.DropDown(r);
                    }
                };

                rol.onCanRemoveCallback = r => planProperty.arraySize <= 1;
            }

            internal static void Unbind(ReorderableList rol)
            {
                rol.drawHeaderCallback = null;
                rol.drawElementBackgroundCallback = null;
                rol.drawElementCallback = null;
                rol.onCanRemoveCallback = null;
            }

            internal static ReorderableList BindTopLayer(LGOAPDomain.Serialized serializedObject)
            {
                var rol = new ReorderableList((SerializedObject) serializedObject, serializedObject.topLayerFallback,
                    true, true, true, true);

                Bind(rol, serializedObject.topLayerFallback, serializedObject.topLayer);
                return rol;
            }

            internal static ReorderableList BindBottomLayer(LGOAPDomain.Serialized serializedObject)
            {
                var rol = new ReorderableList((SerializedObject) serializedObject, serializedObject.bottomLayerFallback,
                    true, true, true, true);

                Bind(rol, serializedObject.bottomLayerFallback, serializedObject.bottomLayer);
                return rol;
            }

            private static ReorderableList BindIntermediateLayer(LGOAPDomain.Serialized serializedObject, int index)
            {
                var rol = new ReorderableList((SerializedObject) serializedObject, serializedObject.intermediateLayersFallbacks[index],
                    true, true, true, true);
                Bind(rol, serializedObject.intermediateLayersFallbacks[index], serializedObject.intermediateLayers[index]);
                return rol;
            }

            internal static ReorderableList[] BindIntermediateLayers(LGOAPDomain.Serialized serializedObject)
            {
                var count = serializedObject.intermediateLayersCount;
                var lists = new ReorderableList[count];

                for (var i = 0; i < count; i++)
                {
                    lists[i] = BindIntermediateLayer(serializedObject, i);
                }

                return lists;
            }

            internal static void RebindIntermediateLayers(ref ReorderableList[] lists, LGOAPDomain.Serialized serializedObject)
            {
                var updatedCount = serializedObject.intermediateLayersCount;
                var originalCount = lists.Length;

                System.Array.Resize(ref lists, updatedCount);
                if (lists == null) lists = new ReorderableList[0];

                var difference = updatedCount - originalCount;

                for (var i = 0; i < difference; i++)
                {
                    lists[originalCount + i] = BindIntermediateLayer(serializedObject, originalCount + i);
                }
            }
        }
    }
}