#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// A custom version of serialized object that caches all found properties.
    /// Eliminates the need for re-lookups.
    /// </summary>
    internal class CustomSerializedObject : System.IDisposable
    {
        private readonly SerializedObject m_SerializedObject;
        protected string m_Error;

        internal CustomSerializedObject(SerializedObject serializedObject)
        {
            m_SerializedObject = serializedObject;
            serializedObject.Update();
            m_Error = "";
        }

        public void Dispose()
        {
            m_SerializedObject.Dispose();
        }

        /// <summary>
        /// Whether the serialized object has errors.
        /// </summary>
        internal bool hasError => m_Error != "";

        /// <summary>
        /// The error string in the serialized object.
        /// </summary>
        internal string error => m_Error;

        /// <summary>
        /// Cache a non-primitive serialized property.
        /// </summary>
        /// <param name="propertyPath">The path of the property.</param>
        /// <param name="isCollection">Whether the property is supposed to be a collection.</param>
        /// <param name="essential">Whether the absence of property is acceptable.</param>
        /// <typeparam name="T">The type of complex object.</typeparam>
        /// <returns>The queried property. If the search/validation fails, then null.</returns>
        internal SerializedProperty GetProperty<T>(string propertyPath, bool? isCollection = null, bool essential = false)
        {
            var p = m_SerializedObject.FindProperty(propertyPath);

            if (p != null)
            {
                if (!isCollection.HasValue)
                {
                    return p;
                }

                SerializedPropertyType? elementPropTypeIfArray;
                string elementTypeIfArray;
                if (!p.isArray)
                {
                    elementPropTypeIfArray = null;
                    elementTypeIfArray = null;
                }
                else
                {
                    var lastIndex = p.arraySize++;
                    var elementProperty = p.GetArrayElementAtIndex(lastIndex);
                    elementPropTypeIfArray = elementProperty.propertyType;
                    elementTypeIfArray = elementProperty.type;
                    p.arraySize--;
                }

                SerializedPropertyType? propTypeToCompare;
                string typeToCompare;

                switch (isCollection)
                {
                    // ReSharper disable HeuristicUnreachableCode
                    case null:
                        propTypeToCompare = null;
                        typeToCompare = null;
                        break;
                    // ReSharper restore HeuristicUnreachableCode
                    case true:
                        propTypeToCompare = elementPropTypeIfArray;
                        typeToCompare = elementTypeIfArray;
                        break;
                    case false:
                        propTypeToCompare = p.propertyType;
                        typeToCompare = p.type;
                        break;
                }

                switch (propTypeToCompare)
                {
                    case null:
                    case SerializedPropertyType.Generic when typeToCompare == nameof(T):
                    case SerializedPropertyType.ObjectReference when typeToCompare == $"PPtr<${nameof(T)}>":
                        return p;
                }
            }

            if (essential)
            {
                m_Error += $"Could not find property: {propertyPath}";
            }

            return null;
        }

        /// <summary>
        /// Cache a non-primitive SerializedProperty.
        /// </summary>
        /// <param name="propertyPath">The path of the property.</param>
        /// <param name="type">The type of property.</param>
        /// <param name="isCollection">Whether the property is supposed to be a collection.</param>
        /// <param name="essential">Whether the absence of property is acceptable.</param>
        /// <returns>The queried property. If the search/validation fails, then null.</returns>
        internal SerializedProperty GetProperty(string propertyPath,
            SerializedPropertyType? type = null, bool? isCollection = null, bool essential = false)
        {
            var p = m_SerializedObject.FindProperty(propertyPath);

            if (p != null)
            {
                if (!type.HasValue && !isCollection.HasValue)
                {
                    return p;
                }

                SerializedPropertyType? elementTypeIfArray;
                if (!p.isArray)
                {
                    elementTypeIfArray = null;
                }
                else
                {
                    var lastIndex = p.arraySize++;
                    elementTypeIfArray = p.GetArrayElementAtIndex(lastIndex).propertyType;
                    p.arraySize--;
                }

                bool collectionValidity;
                bool typeValidity;

                switch (isCollection)
                {
                    case null:
                        collectionValidity = true;
                        break;
                    case true:
                        collectionValidity = type == SerializedPropertyType.String
                            ? elementTypeIfArray == SerializedPropertyType.String
                            : p.isArray;
                        break;
                    case false:
                        collectionValidity = type == SerializedPropertyType.String
                            ? elementTypeIfArray != SerializedPropertyType.String
                            : !p.isArray;
                        break;
                }

                switch (type)
                {
                    case null:
                        typeValidity = true;
                        break;
                    default:
                        switch (isCollection)
                        {
                            case null:
                                typeValidity = p.propertyType == type || elementTypeIfArray == type;
                                break;
                            case true:
                                typeValidity = elementTypeIfArray == type;
                                break;
                            case false:
                                typeValidity = p.propertyType == type;
                                break;
                        }

                        break;
                }

                if (collectionValidity && typeValidity)
                {
                    return p;
                }
            }

            if (essential)
            {
                m_Error += $"Could not find property: {propertyPath}";
            }

            return null;
        }

        /// <summary>
        /// Update serialized object.
        /// </summary>
        internal void Update()
        {
            m_SerializedObject.Update();
        }

        /// <summary>
        /// Apply modified properties. Return true, if there was a change.
        /// </summary>
        internal bool ApplyModifiedProperties()
        {
            return m_SerializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Apply modified properties without undo. Returns true, if there was a change.
        /// </summary>
        internal bool ApplyModifiedPropertiesWithoutUndo()
        {
            return m_SerializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }

    internal class CustomSerializedObject<T> : CustomSerializedObject
        where T : Object
    {
        private readonly T m_Object;

        internal CustomSerializedObject(T obj) : base(new SerializedObject(obj))
        {
            m_Object = obj;
        }

        /// <summary>
        /// The target serialized object.
        /// </summary>
        internal T target => m_Object;
    }
}
#endif