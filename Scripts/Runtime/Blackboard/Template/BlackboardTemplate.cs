using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// A blackboard template to make blackboard components out of.
    /// </summary>
    [CreateAssetMenu(fileName = "New Blackboard", menuName = "HiraBots/Blackboard")]
    internal partial class BlackboardTemplate : ScriptableObject
    {
        [SerializeField, HideInInspector] private BlackboardTemplate m_Parent = null;
        [SerializeField, HideInInspector] private BlackboardKey[] m_Keys = new BlackboardKey[0];
    }
}