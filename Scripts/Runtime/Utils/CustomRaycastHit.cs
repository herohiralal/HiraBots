using System.Reflection;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// Helper struct to expose RaycastHit properties.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct CustomRaycastHit
    {
        static unsafe CustomRaycastHit()
        {
            if (sizeof(CustomRaycastHit) != sizeof(RaycastHit))
            {
                throw new System.Exception("Size mismatch.");
            }

            void FieldCheck(string name)
            {
                var f1 = typeof(CustomRaycastHit).GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var f2 = typeof(RaycastHit).GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (f1 == null)
                {
                    throw new System.NullReferenceException($"Could not find the field {name} in self type.");
                }

                if (f2 == null)
                {
                    throw new System.NullReferenceException($"Could not find the field {name} in main type.");
                }

                if (UnsafeUtility.GetFieldOffset(f1) != UnsafeUtility.GetFieldOffset(f2))
                {
                    throw new System.Exception($"Field offset mismatch: {name}.");
                }
            }

            FieldCheck(nameof(m_Point));
            FieldCheck(nameof(m_Normal));
            FieldCheck(nameof(m_FaceID));
            FieldCheck(nameof(m_Distance));
            FieldCheck(nameof(m_UV));
            FieldCheck(nameof(m_Collider));
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initializer()
        {
            // this method is only present so that it triggers the type initializer
        }

        internal unsafe CustomRaycastHit(RaycastHit hit)
        {
            this = *(CustomRaycastHit*) (&hit);
        }

        internal readonly float3 m_Point;
        internal readonly float3 m_Normal;
        internal readonly uint m_FaceID;
        internal readonly float m_Distance;
        internal readonly float2 m_UV;
        internal readonly int m_Collider;
    }
}