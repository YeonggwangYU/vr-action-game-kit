namespace _3DGamekitLite.Scripts.Runtime.Interactive.Senders
{

    public class SendOnBecameVisible : SendGameCommand
    {
        void OnBecameVisible()
        {
            Send();
        }
    }

}
