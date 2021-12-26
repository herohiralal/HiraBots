using UnityEngine;

namespace HiraBots
{
    internal sealed partial class WaitBlackboardTimeTaskProvider : HiraBotsTaskProvider
    {
        [SerializeField] private UnityEngine.BlackboardTemplate.KeySelector m_Key;

        public override IHiraBotsTask GetTask(UnityEngine.BlackboardComponent blackboard)
        {
            switch (m_Key.selectedKey.keyType)
            {
                case UnityEngine.BlackboardKeyType.Float:
                    return WaitTask.Get(blackboard.GetFloatValue(m_Key.selectedKey.name));
                case UnityEngine.BlackboardKeyType.Integer:
                    return WaitTask.Get(blackboard.GetIntegerValue(m_Key.selectedKey.name));
                default:
                    return ErrorExecutable.Get($"Can not execute {name} because a correct key is not selected.");
            }
        }
    }
}