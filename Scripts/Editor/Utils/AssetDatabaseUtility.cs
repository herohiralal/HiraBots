using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        /// <summary>
        /// Set the serialized property to a new object, and add it to the same file as the target.
        /// </summary>
        /// <param name="target">The target object to add the file to.</param>
        /// <param name="serializedObject">Serialized object.</param>
        /// <param name="property">The object reference (or array of object references) property to add the object to.</param>
        /// <param name="t">The type of object to create.</param>
        /// <param name="name">Name of the newly created object.</param>
        /// <param name="hideFlags">HideFlags for the newly created object.</param>
        /// <returns>Newly created object.</returns>
        internal static Object AddInlinedObject(
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

            return newObject;
        }

        /// <summary>
        /// Remove an inlined object reference.
        /// </summary>
        /// <param name="target">The target object to remove the file from.</param>
        /// <param name="serializedObject">The serialized object.</param>
        /// <param name="property">The object reference (or array of object references) property to remove the object from.</param>
        /// <param name="arrayIndex">If the property is an array, the index at which to remove the object.</param>
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
            if (objectToDestroy != null)
            {
                Undo.DestroyObjectImmediate(objectToDestroy);
            }

            // update editor
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Add missing objects to the file and remove orphaned objects from the file.
        /// </summary>
        /// <param name="target">The main object.</param>
        /// <param name="objectsThatMustBeInFile">The objects that must be in the file.</param>
        /// <param name="allowRemove">Whether to allow removing of objects.</param>
        internal static void SynchronizeFileToCompoundObject(Object target, HashSet<Object> objectsThatMustBeInFile, bool allowRemove)
        {
            var path = AssetDatabase.GetAssetPath(target);
            var subAssets = new HashSet<Object>(AssetDatabase.LoadAllAssetsAtPath(path));
            subAssets.Remove(target); // don't remove the fucking main object from the file again

            var assetDirty = false;

            if (allowRemove)
            {
                foreach (var subAsset in subAssets.Where(subAsset => subAsset != null && !objectsThatMustBeInFile.Contains(subAsset)))
                {
                    Debug.Log($"Object clean-up: removed {subAsset.name} from {path}.");
                    AssetDatabase.RemoveObjectFromAsset(subAsset);
                    assetDirty = true;
                }
            }

            foreach (var requiredObject in objectsThatMustBeInFile.Where(rObj => rObj != null && !subAssets.Contains(rObj)))
            {
                if (AssetDatabase.Contains(requiredObject))
                {
                    Debug.LogError($"Object clean-up: could not add {requiredObject.name} to {path}. It's already an asset.");
                }
                else
                {
                    Debug.Log($"Object clean-up: added {requiredObject.name} to {path}.");
                    AssetDatabase.AddObjectToAsset(requiredObject, path);
                    assetDirty = true;
                }
            }

            if (assetDirty)
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
        }
    }
}