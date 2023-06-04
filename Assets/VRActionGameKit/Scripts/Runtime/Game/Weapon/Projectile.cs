using _3DGamekitLite.Scripts.Runtime.Game.Helpers;
using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Weapon
{
    public abstract class Projectile : MonoBehaviour, IPooled<Projectile>
    {
        public int poolID { get; set; }
        public ObjectPooler<Projectile> pool { get; set; }

        public abstract void Shot(Vector3 target, RangeWeapon shooter);
    }
}