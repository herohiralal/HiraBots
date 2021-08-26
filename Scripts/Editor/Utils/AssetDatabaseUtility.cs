using System;
using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

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
    }
}