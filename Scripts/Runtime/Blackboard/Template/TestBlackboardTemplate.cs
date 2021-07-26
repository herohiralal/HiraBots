// test blackboard template does not show up in object pickers unless testing
#if HIRA_BOTS_TESTS
using UnityEngine;

namespace HiraBots.Editor.Tests
{
    [CreateAssetMenu(fileName = "New Test Blackboard", menuName = "HiraBots/Blackboard (Test)")]
    internal class TestBlackboardTemplate : BlackboardTemplate
    {
    }
}
#endif