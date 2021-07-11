using UnityEngine;

namespace HiraBots
{
    [CreateAssetMenu(fileName = "New Blackboard", menuName = "HiraBots/Blackboard")]
    internal partial class BlackboardTemplate : ScriptableObject
    {
        [SerializeField, HideInInspector] private BlackboardTemplate parent = null;
        [SerializeField, HideInInspector] private BlackboardKey[] keys = new BlackboardKey[0];
    }
}