using System;
using System.Linq.Expressions;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    internal static class DynamicEnumHelpers
    {
        internal static T DynamicEnumPopup<T>(Rect rect, T inValue, Type enumType)
        {
            var enumCast = RuntimeCastToEnum<T>(inValue, enumType);
            var output =
                enumType.GetCustomAttribute<FlagsAttribute>() == null
                    ? EditorGUI.EnumPopup(rect, GUIContent.none, enumCast)
                    : EditorGUI.EnumFlagsField(rect, GUIContent.none, enumCast);
            return RuntimeCastToUnderlyingType<T>(output);
        }
        
        internal static Enum RuntimeCastToEnum<T>(T value, Type enumType)
        {
            var dataParam = Expression.Parameter(typeof(T), "data");
            var body = Expression.Block(Expression.Convert(dataParam, enumType));
            var run = Expression.Lambda(body, dataParam).Compile();
            var ret = (Enum) run.DynamicInvoke(value);
            return ret;
        }

        internal static T RuntimeCastToUnderlyingType<T>(Enum input)
        {
            var dataParam = Expression.Parameter(input.GetType(), "data");
            var body = Expression.Block(Expression.Convert(dataParam, typeof(T)));
            var run = Expression.Lambda(body, dataParam).Compile();
            var ret = (T) run.DynamicInvoke(input);
            return ret;
        }
    }
}