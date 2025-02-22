using System;
using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// Attribute to use in conjunction with <see cref="CookedDataSingleton{T}"/>.
    /// Lets you choose a custom filename.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    internal class CookedDataSingletonAttribute : Attribute
    {
        internal CookedDataSingletonAttribute(string name)
        {
            this.name = name;
        }

        internal string name { get; }
    }

    /// <summary>
    /// Base class to create cooked data. These can be used as config files, but are not automatically built.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class CookedDataSingleton<T> : ScriptableObject where T : CookedDataSingleton<T>
    {
        private static T s_Instance;

        /// <summary>
        /// Get the instance of the object.
        /// </summary>
        internal static T instance
        {
            get
            {
                if (s_Instance != null)
                {
                    return s_Instance;
                }

                s_Instance = SerializationUtility.LoadCookedData<T>();

                return s_Instance == null
                    ? throw new NullReferenceException($"{typeof(T).FullName} instance was not cooked.")
                    : s_Instance;
            }
        }

        /// <summary>
        /// Clear the static instance.
        /// This will force the instance to be re-read from wherever it was serialized, the next time it is accessed.
        /// </summary>
        internal static void ClearInstance()
        {
            // set field to null first, in case OnDestroy throws an exception
            
            var i = s_Instance;
            s_Instance = null;
            SerializationUtility.UnloadCookedData(i);
        }

        /// <summary>
        /// Get the filename to store this singleton at.
        /// </summary>
        internal static string fileName
        {
            get
            {
                foreach (var attribute in typeof(T).GetCustomAttributes(true))
                {
                    if (attribute is CookedDataSingletonAttribute properties)
                    {
                        return properties.name;
                    }
                }

                return typeof(T).FullName;
            }
        }
    }
}