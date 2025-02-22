using System;
using UnityEditor;
using UnityEngine;

// This file contains certain temporary overrides to GUI that are removed when Dispose() gets called.
namespace HiraBots.Editor
{
    /// <summary>
    /// Change GUI.enabled and reset it on Dispose().
    /// </summary>
    internal readonly struct GUIEnabledChanger : IDisposable
    {
        private readonly bool m_ExistingValue;

        internal GUIEnabledChanger(bool newValue)
        {
            (m_ExistingValue, GUI.enabled) = (GUI.enabled, newValue);
        }

        public void Dispose()
        {
            GUI.enabled = m_ExistingValue;
        }
    }

    /// <summary>
    /// Change EditorGUI.indentLevel and reset it on Dispose().
    /// </summary>
    internal readonly struct IndentNullifier : IDisposable
    {
        private readonly int m_ExistingValue;

        internal IndentNullifier(int newValue)
        {
            (m_ExistingValue, EditorGUI.indentLevel) = (EditorGUI.indentLevel, newValue);
        }

        public void Dispose()
        {
            EditorGUI.indentLevel = m_ExistingValue;
        }
    }

    /// <summary>
    /// Merge multiple undo functions within the scope under a single command name.
    /// </summary>
    internal readonly struct UndoMerger : IDisposable
    {
        private readonly int m_Group;

        internal UndoMerger(string name)
        {
            Undo.SetCurrentGroupName(name);
            m_Group = Undo.GetCurrentGroup();
        }

        public void Dispose()
        {
            Undo.CollapseUndoOperations(m_Group);
        }
    }
}