namespace _3DGamekitLite.Scripts.Runtime.Interactive.Senders
{

    public class SendOnBecameInvisible : SendGameCommand
    {
        void OnBecameInvisible()
        {
            Send();
        }
    }
}
