using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    /// <summary>
    /// Custom editor for Blackboard Template.
    /// todo: implement this in UIElements (the current issue is that ListView has a predefined length for each element)
    /// </summary>
    [CustomEditor(typeof(HiraLGOAPBot))]
    internal class HiraLGOAPBotEditor : UnityEditor.Editor
    {
        private bool m_EditBlackboard;

        private HiraLGOAPBot.Serialized m_Bot;
        private HashSet<BlackboardKey> m_Keys;

        private void OnEnable()
        {
            m_Bot = new HiraLGOAPBot.Serialized((HiraLGOAPBot) target);
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

            m_Bot.Update();
            EditorGUILayout.PropertyField(m_Bot.archetypeProperty, true);
            EditorGUILayout.PropertyField(m_Bot.domainProperty, true);
            m_Bot.ApplyModifiedProperties();

            if (m_Bot.domain != null && m_Bot.domain.blackboard != null && m_Bot.domain.blackboard.isCompiled && m_Bot.blackboard != null)
            {
                var blackboardComponent = m_Bot.blackboard;
                var blackboard = m_Bot.domain.blackboard;

                if (m_Keys == null)
                {
                    m_Keys = new HashSet<BlackboardKey>();
                    blackboard.GetKeySet(m_Keys);
                }

                EditorGUILayout.Space();
                var blackboardHeadingRect = EditorGUILayout.GetControlRect();
                var editableButtonRect = EditorGUI.PrefixLabel( blackboardHeadingRect, GUIHelpers.TempContent("Blackboard"), EditorStyles.boldLabel);
                if (GUI.Button(editableButtonRect, m_EditBlackboard ? "Stop Editing" : "Edit"))
                {
                    m_EditBlackboard = !m_EditBlackboard;
                }

                using (new GUIEnabledChanger(m_EditBlackboard))
                {
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
                                    HiraBotsModule.DispatchOnMainThread(() => blackboardComponent.SetBooleanValue(keyName, newValue));
                                }

                                break;
                            }

                            case EnumBlackboardKey enumKey:
                            {
                                var currentValue = blackboardComponent.GetEnumValue(keyName);
                                byte newValue;

                                unsafe
                                {
                                    var newValueTemp = currentValue;

                                    GUIHelpers.DynamicEnumPopup(EditorGUILayout.GetControlRect(),
                                        GUIHelpers.TempContent(keyName),
                                        (System.IntPtr) (&newValueTemp),
                                        DynamicEnum.Helpers.identifierToType[enumKey.typeIdentifier]);

                                    newValue = newValueTemp;
                                }

                                if (currentValue != newValue)
                                {
                                    HiraBotsModule.DispatchOnMainThread(() => blackboardComponent.SetEnumValue(keyName, newValue));
                                }

                                break;
                            }

                            case FloatBlackboardKey _:
                            {
                                var currentValue = blackboardComponent.GetFloatValue(keyName);
                                var newValue = EditorGUILayout.DelayedFloatField(keyName, currentValue);

                                // ReSharper disable once CompareOfFloatsByEqualityOperator
                                if (currentValue != newValue)
                                {
                                    HiraBotsModule.DispatchOnMainThread(() => blackboardComponent.SetFloatValue(keyName, newValue));
                                }

                                break;
                            }

                            case IntegerBlackboardKey _:
                            {
                                var currentValue = blackboardComponent.GetIntegerValue(keyName);
                                var newValue = EditorGUILayout.DelayedIntField(keyName, currentValue);

                                if (currentValue != newValue)
                                {
                                    HiraBotsModule.DispatchOnMainThread(() => blackboardComponent.SetIntegerValue(keyName, newValue));
                                }

                                break;
                            }

                            case ObjectBlackboardKey _:
                            {
                                var currentValue = blackboardComponent.GetObjectValue(keyName);
                                var newValue = EditorGUILayout.ObjectField(keyName, currentValue, typeof(Object), true);

                                if (currentValue != newValue)
                                {
                                    HiraBotsModule.DispatchOnMainThread(() => blackboardComponent.SetObjectValue(keyName, newValue));
                                }

                                break;
                            }

                            case QuaternionBlackboardKey _:
                            {
                                var currentValue = ((Quaternion) blackboardComponent.GetQuaternionValue(keyName)).eulerAngles;
                                var newValue = EditorGUILayout.Vector3Field(keyName, currentValue);

                                if (currentValue != newValue)
                                {
                                    HiraBotsModule.DispatchOnMainThread(() =>
                                        blackboardComponent.SetQuaternionValue(keyName, Quaternion.Euler(newValue)));
                                }

                                break;
                            }

                            case VectorBlackboardKey _:
                            {
                                var currentValue = (Vector3) blackboardComponent.GetVectorValue(keyName);
                                var newValue = EditorGUILayout.Vector3Field(keyName, currentValue);

                                if (currentValue != newValue)
                                {
                                    HiraBotsModule.DispatchOnMainThread(() => blackboardComponent.SetVectorValue(keyName, newValue));
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
            else
            {
                m_Keys = null;
            }
        }
    }
}