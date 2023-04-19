using _3DGamekitLite.Scripts.Runtime.Interactive;

namespace _3DGamekitLite.Scripts.Runtime.Game.Player
{
    public class RespawnPlayer : GameCommandHandler
    {
        public PlayerController player;

        public override void PerformInteraction()
        {
            player.Respawn();
        }
    }
}
