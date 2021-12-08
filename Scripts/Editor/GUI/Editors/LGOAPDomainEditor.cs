using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    /**
     * todo: decide on a theme
     * 
     * Unreal:
     * decorator 003ea0
     * task 7934a7
     * service 0e997e
     *
     * Custom:
     * 203, 232, 150 decorator
     * 238, 150, 75 score calculator
     * 239, 71, 111 effector
     * 83, 58, 113 task
     * 200, 198, 215 service
     * 80, 61, 66 reset - used for bb header
     */

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

        private void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndoPerformed;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoPerformed;
            m_SerializedObject = null;
            m_Dirty = false;
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

                var editingAllowed = !EditorApplication.isPlayingOrWillChangePlaymode;

                if (!editingAllowed)
                {
                    EditorGUILayout.HelpBox("Blackboard Templates are read-only while in play mode.", MessageType.Warning);
                }

                if (m_Dirty)
                {
                    m_SerializedObject.Update();
                    // todo: synchronize file to compound object here
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

                    // blackboard property field
                    m_SerializedObject.Update();
                    EditorGUILayout.PropertyField(m_SerializedObject.blackboard);
                    if (m_SerializedObject.ApplyModifiedProperties())
                    {
                        m_SerializedObject.OnBlackboardUpdate();
                    }

                    m_SerializedObject.Update();
                    m_SerializedObject.intermediateLayersCount = EditorGUILayout.IntSlider(m_SerializedObject.intermediateLayersCount, 0, 2);
                    if (m_SerializedObject.ApplyModifiedProperties())
                    {
                        m_Dirty = true;
                    }

                    var currentBlackboard = m_SerializedObject.blackboard.objectReferenceValue as BlackboardTemplate;
                    if (currentBlackboard == null)
                    {
                        EditorGUILayout.HelpBox("Assign a blackboard template to modify AI behaviour.", MessageType.Info);
                        return;
                    }

                    // intermediate layer count field
                    // m_SerializedObject.Update();
                    // var intermediateLayerCountGUIContent = GUIHelpers.TempContent("Intermediate Layer Count", "The number of intermediate layers.");
                    // var originalIntermediateLayerCount = m_SerializedObject.intermediateLayers.arraySize;
                    // var newIntermediateLayerCount =
                    //     EditorGUILayout.IntSlider(intermediateLayerCountGUIContent, m_SerializedObject.intermediateLayers.arraySize, 0, 5);
                    // if (originalIntermediateLayerCount < newIntermediateLayerCount)
                    // {
                    //     for (var i = originalIntermediateLayerCount; i < newIntermediateLayerCount; i++)
                    //     {
                    //         
                    //     }
                    // }

                    EditorGUILayout.Space();
                }
            }
        }

        private void LogAssetWithADuplicateReference() => Debug.LogFormat(LogType.Exception, LogOption.None, target,
            $"{target.name} contains duplicate references to the same asset, which must not happen in a compound object.");

        private HashSet<Object> assetsThatMustBeInFile
        {
            get
            {
                var hs = new HashSet<Object>();

                foreach (var goal in m_SerializedObject.topLayer.ToSerializedArrayProperty().Select(p => (LGOAPGoal) p.objectReferenceValue))
                {
                    hs.Add(goal);

                    foreach (var d in goal.insistence.m_Insistence)
                    {
                        hs.Add(d);
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

                        foreach (var d in task.action.m_Cost)
                        {
                            hs.Add(d);
                        }

                        foreach (var d in task.action.m_Effect)
                        {
                            hs.Add(d);
                        }

                        foreach (var d in task.target.m_Target)
                        {
                            hs.Add(d);
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

                    foreach (var d in task.action.m_Cost)
                    {
                        hs.Add(d);
                    }

                    foreach (var d in task.action.m_Effect)
                    {
                        hs.Add(d);
                    }

                    foreach (var d in task.target.m_Target)
                    {
                        hs.Add(d);
                    }
                }

                return hs;
            }
        }
    }
}