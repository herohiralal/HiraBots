using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HiraBots.Editor
{
    /// <summary>
    /// A utility class to help with multiple assets being present in the same file.
    /// </summary>
    internal class MultiAssetFileHelper
    {
        /// <summary>
        /// Create a MultiAssetFileHelper
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <param name="serializedObject">The target object as a SerializedObject.</param>
        /// <param name="arrayProperty">The array property to edit and add objects to.</param>
        internal MultiAssetFileHelper(Object target, SerializedObject serializedObject, SerializedProperty arrayProperty)
        {
            m_Target = target;
            m_SerializedObject = serializedObject;
            m_ArrayProperty = arrayProperty;
        }

        ~MultiAssetFileHelper()
        {
            m_Target = null;
            m_SerializedObject = null;
            m_ArrayProperty = null;
        }

        private Object m_Target;
        private SerializedObject m_SerializedObject;
        private SerializedProperty m_ArrayProperty;

        /// <summary>
        /// Synchronize the file to the collection by removing orphaned assets and adding absent assets.
        /// </summary>
        internal void SynchronizeFileToCollection()
        {
            m_SerializedObject.Update();
            var assetsInFile = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(m_Target))
                .Where(a => a != m_Target)
                .ToArray();

            var arraySize = m_ArrayProperty.arraySize;
            var assetsCount = assetsInFile.Length;

            var assetDirty = false;

            for (var i = assetsCount - 1; i >= 0; i--)
            {
                var currentAsset = assetsInFile[i];
                if (currentAsset == m_Target) continue;

                var found = false;
                for (var j = 0; j < arraySize; j++)
                {
                    if (m_ArrayProperty.GetArrayElementAtIndex(j).objectReferenceValue != currentAsset) continue;

                    found = true;
                    break;
                }

                if (found) continue;

                AssetDatabase.RemoveObjectFromAsset(currentAsset);
                assetsInFile[i] = null;
                assetDirty = true;
            }

            for (var i = 0; i < arraySize; i++)
            {
                var currentKey = m_ArrayProperty.GetArrayElementAtIndex(i).objectReferenceValue;

                if (assetsInFile.Contains(currentKey)) continue;

                AssetDatabase.AddObjectToAsset(currentKey, m_Target);
                assetDirty = true;
            }

            if (assetDirty)
            {
                EditorUtility.SetDirty(m_Target);
                AssetDatabase.SaveAssets();
            }

            m_SerializedObject.ApplyModifiedProperties();
        }
    }
}