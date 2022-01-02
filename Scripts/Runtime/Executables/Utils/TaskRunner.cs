﻿using UnityEngine;

namespace HiraBots
{
    internal static class TaskRunner
    {
        internal interface IInterface
        {
            void Add(ExecutorComponent executor, IHiraBotsTask task, float tickInterval);
            void Remove(ExecutorComponent executor);
        }

        internal static IInterface instance { get; set; }
    }
}