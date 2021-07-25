using System;
using UnityEngine;

namespace HiraBots
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    internal class CookedDataSingletonAttribute : Attribute
    {
        internal CookedDataSingletonAttribute(string name) => Name = name;
        internal readonly string Name;
    }

    internal abstract class CookedDataSingleton<T> : ScriptableObject where T : CookedDataSingleton<T>
    {
        private static T _instance;

        internal static T Instance
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = SerializationUtility.LoadCookedData<T>(FileName);

                return _instance == null
                    ? throw new NullReferenceException($"{typeof(T).FullName} instance was not cooked.")
                    : _instance;
            }
        }

        internal static string FileName
        {
            get
            {
                foreach (var attribute in typeof(T).GetCustomAttributes(true))
                {
                    if (attribute is CookedDataSingletonAttribute properties)
                        return properties.Name;
                }

                return typeof(T).FullName;
            }
        }
    }
}