using System;
using UnityEngine;

namespace HiraBots
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    internal class CookedDataSingletonAttribute : Attribute
    {
        internal CookedDataSingletonAttribute(string name) => m_Name = name;
        internal readonly string m_Name;
    }

    internal abstract class CookedDataSingleton<T> : ScriptableObject where T : CookedDataSingleton<T>
    {
        private static T s_Instance;

        internal static T instance
        {
            get
            {
                if (s_Instance != null) return s_Instance;

                s_Instance = SerializationUtility.LoadCookedData<T>(fileName);

                return s_Instance == null
                    ? throw new NullReferenceException($"{typeof(T).FullName} instance was not cooked.")
                    : s_Instance;
            }
        }

        internal static string fileName
        {
            get
            {
                foreach (var attribute in typeof(T).GetCustomAttributes(true))
                {
                    if (attribute is CookedDataSingletonAttribute properties)
                        return properties.m_Name;
                }

                return typeof(T).FullName;
            }
        }
    }
}