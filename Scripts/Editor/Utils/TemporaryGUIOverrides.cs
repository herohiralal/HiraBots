using System;
using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    internal class GUIEnabledChanger : IDisposable
    {
        private readonly bool _existingValue;

        internal GUIEnabledChanger(bool newValue) => (_existingValue, GUI.enabled) = (GUI.enabled, newValue);

        public void Dispose() => GUI.enabled = _existingValue;
    }

    internal class IndentNullifier : IDisposable
    {
        private readonly int _existingValue;

        internal IndentNullifier() => (_existingValue, EditorGUI.indentLevel) = (EditorGUI.indentLevel, 0);

        public void Dispose() => EditorGUI.indentLevel = _existingValue;
    }
}