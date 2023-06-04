using _3DGamekitLite.Scripts.Runtime.Game.Enemies.Grenadier;
using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Weapon
{
    public class Spit : GrenadierGrenade
    {
        protected override void OnCollisionEnter(Collision other)
        {
            base.OnCollisionEnter(other);

            if(explosionTimer < 0)
                Explosion();
        }
    }
}