using System;
using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    internal class GUIEnabledChanger : IDisposable
    {
        private readonly bool m_ExistingValue;

        internal GUIEnabledChanger(bool newValue) => (m_ExistingValue, GUI.enabled) = (GUI.enabled, newValue);

        public void Dispose() => GUI.enabled = m_ExistingValue;
    }

    internal class IndentNullifier : IDisposable
    {
        private readonly int m_ExistingValue;

        internal IndentNullifier() => (m_ExistingValue, EditorGUI.indentLevel) = (EditorGUI.indentLevel, 0);

        public void Dispose() => EditorGUI.indentLevel = m_ExistingValue;
    }

    internal class UndoMerger : IDisposable
    {
        private readonly int m_Group;

        internal UndoMerger(string name)
        {
            Undo.SetCurrentGroupName(name);
            m_Group = Undo.GetCurrentGroup();
        }

        public void Dispose() => Undo.CollapseUndoOperations(m_Group);
    }
}