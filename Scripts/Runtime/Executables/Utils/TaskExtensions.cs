﻿using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

namespace HiraBots
{
    internal static class TaskExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Message<T>(this IHiraBotsTask task, T message)
        {
            if (task is IMessageListener<T> listener)
            {
                try
                {
                    listener.OnMessageReceived(message);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void WrappedBegin(this IHiraBotsTask task)
        {
            try
            {
                task.Begin();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static HiraBotsTaskResult WrappedExecute(this IHiraBotsTask task, float deltaTime)
        {
            try
            {
                return task.Execute(deltaTime);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return HiraBotsTaskResult.InProgress;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void WrappedEnd(this IHiraBotsTask task, bool success)
        {
            try
            {
                task.End(success);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void WrappedAbort(this IHiraBotsTask task)
        {
            try
            {
                task.Abort();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}