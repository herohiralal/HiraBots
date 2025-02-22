using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace HiraBots.Editor
{
    /// <summary>
    /// Helper class to ease iteration over SerializedProperty that is an array.
    /// </summary>
    internal readonly struct SerializedArrayProperty : IEnumerable<SerializedProperty>
    {
        private readonly SerializedProperty m_Property;

        internal SerializedArrayProperty(SerializedProperty property)
        {
            UnityEngine.Debug.Assert(property.isArray, "Property is not array.");
            m_Property = property;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(m_Property);
        }

        IEnumerator<SerializedProperty> IEnumerable<SerializedProperty>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Basic enumerator that iterates over the elements of the array.
        /// </summary>
        internal struct Enumerator : IEnumerator<SerializedProperty>
        {
            private readonly SerializedProperty m_Property;
            private readonly int m_Size;
            private int m_Index;

            internal Enumerator(SerializedProperty property)
            {
                m_Property = property;
                m_Size = property.arraySize;
                m_Index = -1;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                ++m_Index;
                return m_Index < m_Size;
            }

            public void Reset()
            {
                m_Index = -1;
            }

            public SerializedProperty Current => m_Property.GetArrayElementAtIndex(m_Index);

            object IEnumerator.Current => Current;
        }
    }

    internal static class SerializedArrayPropertyUtility
    {
        /// <summary>
        /// Extension method to convert a serialized property (array into an IEnumerable SerializedArrayProperty.
        /// </summary>
        internal static SerializedArrayProperty ToSerializedArrayProperty(this SerializedProperty property)
        {
            return new SerializedArrayProperty(property);
        }
    }
}