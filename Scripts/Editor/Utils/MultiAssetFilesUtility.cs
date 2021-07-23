using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HiraBots.Editor
{
    internal class MultiAssetFileHelper
    {
        internal MultiAssetFileHelper(Object target, SerializedObject serializedObject, SerializedProperty arrayProperty)
        {
            _target = target;
            _serializedObject = serializedObject;
            _arrayProperty = arrayProperty;
        }

        ~MultiAssetFileHelper()
        {
            _target = null;
            _serializedObject = null;
            _arrayProperty = null;
        }

        private Object _target;
        private SerializedObject _serializedObject;
        private SerializedProperty _arrayProperty;

        internal void AddNewObject(Type t)
        {
            _serializedObject.Update();

            var index = _arrayProperty.arraySize;

            var newObject = ScriptableObject.CreateInstance(t);
            newObject.name = t.Name;
            newObject.hideFlags |= HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            Undo.RegisterCreatedObjectUndo(newObject, $"Add {t.Name} Object");

            AssetDatabase.AddObjectToAsset(newObject, _target);

            _arrayProperty.arraySize++;
            var newObjectProperty = _arrayProperty.GetArrayElementAtIndex(index);
            newObjectProperty.objectReferenceValue = newObject;

            _serializedObject.ApplyModifiedProperties();

            EditorUtility.SetDirty(_target);
            AssetDatabase.SaveAssets();
        }

        internal void RemoveObject(int index)
        {
            _serializedObject.Update();

            var property = _arrayProperty.GetArrayElementAtIndex(index);
            var objectToRemove = property.objectReferenceValue;

            property.objectReferenceValue = null;
            _arrayProperty.DeleteArrayElementAtIndex(index);

            _serializedObject.ApplyModifiedProperties();

            Undo.SetCurrentGroupName($"Destroy {objectToRemove.name}");
            var group = Undo.GetCurrentGroup();
            AssetDatabase.RemoveObjectFromAsset(objectToRemove);
            Undo.DestroyObjectImmediate(objectToRemove);
            Undo.CollapseUndoOperations(group);

            EditorUtility.SetDirty(_target);
            AssetDatabase.SaveAssets();
        }

        internal void SynchronizeCollectionAndAsset()
        {
            _serializedObject.Update();
            var assetsInFile = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(_target))
                .Where(a => a != _target)
                .ToArray();

            var arraySize = _arrayProperty.arraySize;
            var assetsCount = assetsInFile.Length;

            var assetDirty = false;

            for (var i = assetsCount - 1; i >= 0; i--)
            {
                var currentAsset = assetsInFile[i];
                if (currentAsset == _target) continue;

                var found = false;
                for (var j = 0; j < arraySize; j++)
                {
                    if (_arrayProperty.GetArrayElementAtIndex(j).objectReferenceValue != currentAsset) continue;

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
                var currentKey = _arrayProperty.GetArrayElementAtIndex(i).objectReferenceValue;

                if (assetsInFile.Contains(currentKey)) continue;

                AssetDatabase.AddObjectToAsset(currentKey, _target);
                assetDirty = true;
            }

            if (assetDirty)
            {
                EditorUtility.SetDirty(_target);
                AssetDatabase.SaveAssets();
            }

            _serializedObject.ApplyModifiedProperties();
        }
    }
}