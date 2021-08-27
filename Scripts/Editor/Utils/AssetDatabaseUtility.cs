using System;
using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HiraBots.Editor
{
    /// <summary>
    /// Helper class to set up an object after it has been created.
    /// </summary>
    internal class NewObjectAction : EndNameEditAction
    {
        internal Type m_T = null;

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            if (m_T == null)
            {
                return;
            }

            var obj = CreateInstance(m_T);
            obj.name = Path.GetFileName(pathName);
            AssetDatabase.CreateAsset(obj, AssetDatabase.GenerateUniqueAssetPath(pathName));

            ProjectWindowUtil.ShowCreatedAsset(obj);
        }
    }

    /// <summary>
    /// Provides extra functionality for asset database.
    /// </summary>
    internal static class AssetDatabaseUtility
    {
        private const string k_InlinedObjectModificationInvalidPropertyError = "Property must be an object reference or its array.";

        /// <summary>
        /// An alternate to using [CreateAssetMenu].
        /// Main use case - allowing custom validation functions in conjunction with MenuItems.
        /// </summary>
        internal static void CreateNewObject<T>(string name) where T : ScriptableObject
        {
            name = (name ?? $"New{typeof(T).Name}") + ".asset";
            if (typeof(T).IsAbstract)
            {
                throw new ArgumentException($"{typeof(T).FullName} is abstract.", nameof(T));
            }

            var action = ScriptableObject.CreateInstance<NewObjectAction>();
            action.m_T = typeof(T);

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, action, name, null, null);
        }

        // does not perform any serialized object update/apply

        internal static void AddInlinedObject(
            Object target,
            SerializedObject serializedObject,
            SerializedProperty property,
            Type t,
            string name = null,
            HideFlags hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector)
        {
            var isArray = property.isArray;

            // validate
            if (!isArray && property.propertyType != SerializedPropertyType.ObjectReference)
            {
                throw new ArgumentException(k_InlinedObjectModificationInvalidPropertyError, nameof(property));
            }

            // update
            serializedObject.Update();

            SerializedProperty newObjectProperty;

            if (isArray) // increment array index
            {
                var index = property.arraySize;
                property.arraySize++;
                newObjectProperty = property.GetArrayElementAtIndex(index);
                if (newObjectProperty.propertyType != SerializedPropertyType.ObjectReference)
                {
                    throw new ArgumentException(k_InlinedObjectModificationInvalidPropertyError, nameof(property));
                }
            }
            else // do nothing
            {
                newObjectProperty = property;
            }

            // create
            var newObject = ScriptableObject.CreateInstance(t);
            newObject.name = name ?? t.Name;
            newObject.hideFlags = hideFlags;
            Undo.RegisterCreatedObjectUndo(newObject, $"Create {newObject.name}");

            // add to asset
            AssetDatabase.AddObjectToAsset(newObject, target);

            // set value
            newObjectProperty.objectReferenceValue = newObject;

            // apply
            serializedObject.ApplyModifiedProperties();

            // update editor
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }

        internal static void RemoveInlinedObject(
            Object target,
            SerializedObject serializedObject,
            SerializedProperty property,
            int arrayIndex = -1)
        {
            var isArray = property.isArray && arrayIndex >= 0 && arrayIndex < property.arraySize;

            // validate
            if (!isArray && property.propertyType != SerializedPropertyType.ObjectReference)
            {
                throw new ArgumentException(k_InlinedObjectModificationInvalidPropertyError, nameof(property));
            }

            // update
            serializedObject.Update();

            SerializedProperty objectToRemoveProperty;

            if (isArray) // get proper index
            {
                objectToRemoveProperty = property.GetArrayElementAtIndex(arrayIndex);
                if (objectToRemoveProperty.propertyType != SerializedPropertyType.ObjectReference)
                {
                    throw new ArgumentException(k_InlinedObjectModificationInvalidPropertyError, nameof(property));
                }
            }
            else // do nothing
            {
                objectToRemoveProperty = property;
            }

            var objectToDestroy = objectToRemoveProperty.objectReferenceValue;

            // clear
            objectToRemoveProperty.objectReferenceValue = null;
            if (isArray)
            {
                property.DeleteArrayElementAtIndex(arrayIndex);
            }

            // apply
            serializedObject.ApplyModifiedProperties();

            // destroy
            Undo.DestroyObjectImmediate(objectToDestroy);

            // update editor
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }
    }
}