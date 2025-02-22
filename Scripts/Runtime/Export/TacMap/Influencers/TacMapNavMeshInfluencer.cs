namespace UnityEngine.AI
{
    [AddComponentMenu("AI/TacMap Influencer (NavMesh)")]
    public class TacMapNavMeshInfluencer : MonoBehaviour
    {
        [ContextMenu("Synchronize To NavMesh")]
        public void SynchronizeToNavMesh()
        {
            var map = GetComponent<TacMap>();

            if (map != null)
            {
                SynchronizeToNavMesh(map);
            }
        }

        public static void SynchronizeToNavMesh(TacMap map)
        {
            if (map == null)
            {
                throw new System.NullReferenceException();
            }

            HiraBots.TacMapNavMeshSynchronizer.Run(map.component);
        }
    }
}