namespace UnityEngine.AI
{
    public interface IMessageListener<in T> 
    {
        void OnMessageReceived(T message);
    }
}