using System;
using UnityEngine;

namespace HiraBots
{
    internal abstract class BlackboardKey : ScriptableObject
    {
        [SerializeField, HideInInspector] private bool instanceSynced = false;
        [SerializeField, HideInInspector] private bool essentialToDecisionMaking = false;

        internal abstract byte SizeInBytes { get; }
        internal ushort Identifier => throw new NotImplementedException();
        internal ushort Index => throw new NotImplementedException();

        internal abstract void Compile(IBlackboardKeyCompilerContext context);

#if UNITY_EDITOR // ideally validation is only needed within the editor (either when building, or when exiting play mode

        internal virtual void Validate(IBlackboardTemplateValidatorContext context)
        {
        }

#endif
    }
}