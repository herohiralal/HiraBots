using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HiraBots.Editor
{
    /// <summary>
    /// Custom editor for Blackboard Template.
    /// todo: implement this in UIElements (the current issue is that ListView has a predefined length for each element)
    /// </summary>
    [CustomEditor(typeof(HiraLGOAPRealtimeBot))]
    internal class HiraLGOAPBotEditor : UnityEditor.Editor
    {
        [System.Serializable]
        private enum EditType
        {
            Unexpected,
            Expected
        }

        private bool m_EditBlackboard;
        private EditType m_EditType = EditType.Unexpected;

        private HiraLGOAPRealtimeBot.Serialized m_Bot;
        private HashSet<BlackboardKey> m_Keys;

        private void OnEnable()
        {
            m_Bot = new HiraLGOAPRealtimeBot.Serialized((HiraLGOAPRealtimeBot) target);
        }

        private void OnDisable()
        {
            m_Bot?.Dispose();
            m_Bot = null;
        }

        public override void OnInspectorGUI()
        {
            if (m_Bot == null)
            {
                return;
            }

            if (m_Bot.hasError)
            {
                EditorGUILayout.HelpBox(m_Bot.error, MessageType.Error);
                return;
            }

            m_Bot.Update();
            EditorGUILayout.PropertyField(m_Bot.archetypeProperty, true);
            EditorGUILayout.PropertyField(m_Bot.domainProperty, true);
            EditorGUILayout.PropertyField(m_Bot.tickIntervalProperty, true);
            EditorGUILayout.PropertyField(m_Bot.executableTickIntervalMultiplierProperty, true);
            EditorGUILayout.PropertyField(m_Bot.runPlannerSynchronouslyProperty, true);
            m_Bot.ApplyModifiedProperties();

            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return; // nothing is gonna be compiled unless in play mode
            }

            if (m_Bot.domain == null || m_Bot.domain.blackboard == null || !m_Bot.domain.blackboard.isCompiled || m_Bot.blackboard == null || m_Bot.planner == null)
            {
                m_Keys = null;
                return;
            }

            EditorGUILayout.Space(12f, true);
            var blackboardHeadingRect = EditorGUILayout.GetControlRect();
            var editableButtonRect = EditorGUI.PrefixLabel(blackboardHeadingRect, GUIHelpers.TempContent("Blackboard"), EditorStyles.boldLabel);
            if (GUI.Button(editableButtonRect, m_EditBlackboard ? "Stop Editing" : "Edit (NO UNDO/REDO)"))
            {
                m_EditBlackboard = !m_EditBlackboard;
            }

            var currentEventType = Event.current.type;
            var currentEventTypeIsLayoutOrRepaint = currentEventType == EventType.Layout || currentEventType == EventType.Repaint;

            if (!m_EditBlackboard && !currentEventTypeIsLayoutOrRepaint)
            {
                return;
            }

            DrawCurrentBlackboardState();

            if (!currentEventTypeIsLayoutOrRepaint)
            {
                return;
            }

            DrawCurrentlyRunningTasks();
        }

        private unsafe void DrawCurrentBlackboardState()
        {
            var blackboardComponent = m_Bot.blackboard;
            var blackboard = m_Bot.domain.blackboard;

            if (m_Keys == null)
            {
                m_Keys = new HashSet<BlackboardKey>();
                blackboard.GetKeySet(m_Keys);
            }

            using (new GUIEnabledChanger(m_EditBlackboard))
            {
                m_EditType = (EditType) EditorGUILayout.EnumPopup(
                    GUIHelpers.TempContent("Edit Type", "The type of edit to perform on the blackboard."),
                    m_EditType);

                foreach (var key in m_Keys)
                {
                    var keyName = key.name;

                    switch (key)
                    {
                        case BooleanBlackboardKey _:
                        {
                            var currentValue = blackboardComponent.GetBooleanValue(keyName);
                            var newValue = EditorGUILayout.Toggle(keyName, currentValue);

                            if (currentValue != newValue)
                            {
                                blackboardComponent.SetBooleanValue(keyName,
                                    newValue,
                                    m_EditType == EditType.Expected);
                            }

                            break;
                        }

                        case EnumBlackboardKey enumKey:
                        {
                            var currentValue = blackboardComponent.GetEnumValue(keyName);

                            var newValueTemp = currentValue;

                            GUIHelpers.DynamicEnumPopup(EditorGUILayout.GetControlRect(),
                                GUIHelpers.TempContent(keyName),
                                (System.IntPtr) (&newValueTemp),
                                DynamicEnum.Helpers.identifierToType[enumKey.typeIdentifier]);

                            var newValue = newValueTemp;

                            if (currentValue != newValue)
                            {
                                blackboardComponent.SetEnumValue(keyName,
                                    newValue,
                                    m_EditType == EditType.Expected);
                            }

                            break;
                        }

                        case FloatBlackboardKey _:
                        {
                            var currentValue = blackboardComponent.GetFloatValue(keyName);
                            var newValue = EditorGUILayout.DelayedFloatField(keyName, currentValue);

                            if (Mathf.Abs(newValue - currentValue) >= 0.0001f)
                            {
                                blackboardComponent.SetFloatValue(keyName,
                                    newValue,
                                    m_EditType == EditType.Expected);
                            }

                            break;
                        }

                        case IntegerBlackboardKey _:
                        {
                            var currentValue = blackboardComponent.GetIntegerValue(keyName);
                            var newValue = EditorGUILayout.DelayedIntField(keyName, currentValue);

                            if (currentValue != newValue)
                            {
                                blackboardComponent.SetIntegerValue(keyName,
                                    newValue,
                                    m_EditType == EditType.Expected);
                            }

                            break;
                        }

                        case ObjectBlackboardKey _:
                        {
                            var currentValue = blackboardComponent.GetObjectValue(keyName);
                            var newValue = EditorGUILayout.ObjectField(keyName, currentValue, typeof(Object), true);

                            if (currentValue != newValue)
                            {
                                blackboardComponent.SetObjectValue(keyName,
                                    newValue,
                                    m_EditType == EditType.Expected);
                            }

                            break;
                        }

                        case QuaternionBlackboardKey _:
                        {
                            var currentValue = ((Quaternion) blackboardComponent.GetQuaternionValue(keyName)).eulerAngles;
                            var newValue = EditorGUILayout.Vector3Field(keyName, currentValue);

                            if (currentValue != newValue)
                            {
                                blackboardComponent.SetQuaternionValue(keyName,
                                    Quaternion.Euler(newValue),
                                    m_EditType == EditType.Expected);
                            }

                            break;
                        }

                        case VectorBlackboardKey _:
                        {
                            var currentValue = (Vector3) blackboardComponent.GetVectorValue(keyName);
                            var newValue = EditorGUILayout.Vector3Field(keyName, currentValue);

                            if (currentValue != newValue)
                            {
                                blackboardComponent.SetVectorValue(keyName,
                                    newValue,
                                    m_EditType == EditType.Expected);
                            }

                            break;
                        }

                        default:
                        {
                            EditorGUILayout.LabelField(keyName, "Unknown type.");
                            break;
                        }
                    }
                }
            }
        }

        private void DrawCurrentlyRunningTasks()
        {
            var planSet = new LGOAPPlannerComponent.Serialized(m_Bot.planner).executionSet;
            var domain = m_Bot.domain.compiledData;

            var layerCount = domain.layerCount;

            for (var i = 0; i < layerCount; i++)
            {
                var planAtCurrentLayer = planSet[i];
                var length = planAtCurrentLayer.length;
                var currentExecutionIndex = planAtCurrentLayer.currentIndex;

                EditorGUILayout.Space(12f, true);
                var lengthRect = EditorGUI.PrefixLabel(EditorGUILayout.GetControlRect(),
                    GUIHelpers.TempContent(i == 0 ? "GOALS [Layer Index: 0]" : $"TASKS [Layer Index: {i}]"),
                    EditorStyles.boldLabel);

                EditorGUI.LabelField(lengthRect,
                    GUIHelpers.TempContent($"[EXECUTED: {currentExecutionIndex}/{length}] [LENGTH: {length}/{planAtCurrentLayer.maxLength}]"),
                    EditorStyles.miniLabel);

                for (short j = 0; j < length; j++)
                {
                    var currentContainerIndex = planAtCurrentLayer[j];

                    var containerName = domain.GetContainerName(i, currentContainerIndex);

                    var progressBarRect = EditorGUI.PrefixLabel(EditorGUILayout.GetControlRect(), GUIHelpers.TempContent(containerName));

                    float progress;
                    string progressBarText;
                    if (j != currentExecutionIndex)
                    {
                        progress = j < currentExecutionIndex ? 1 : 0;
                        progressBarText = "";
                    }
                    else
                    {
                        if (i == 0 || domain.IsTaskAbstract(i, currentContainerIndex))
                        {
                            // if this task is abstract, there's gonna be a next layer
                            var nextLayerPlan = planSet[i + 1];
                            progress = (float) nextLayerPlan.currentIndex / nextLayerPlan.length;
                        }
                        else
                        {
                            domain.GetTaskProviders(i, currentContainerIndex, out var tp);
                            progress = 1 - ((m_Bot.currentTaskProvidersQueueLength + 1f) / tp.count);
                        }

                        progressBarText = ">> Executing <<";

                        if (i != 0)
                        {
                            domain.GetServiceProviders(i, currentContainerIndex, out var sp);

                            if (sp.count > 0)
                            {
                                using (new IndentNullifier(EditorGUI.indentLevel + 1))
                                {
                                    EditorGUILayout.LabelField("SERVICES", EditorStyles.miniBoldLabel);
                                    foreach (var serviceProvider in sp)
                                    {
                                        EditorGUILayout.LabelField($"{serviceProvider.name} every {serviceProvider.tickInterval} second(s)",
                                            EditorStyles.miniLabel);
                                    }
                                }
                            }
                        }
                    }

                    EditorGUI.ProgressBar(progressBarRect, progress, progressBarText);
                }
            }
        }
    }
}