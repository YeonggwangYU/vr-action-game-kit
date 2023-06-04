using _3DGamekitLite.Scripts.Runtime.Game.Weapon;
using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Enemies
{
    public class HumanoidRootController : MonoBehaviour
    {

        /// <Summary>
        /// この変数に対して敵の武器を設定することで、武器に付与されたスクリプトの関数を呼び出せるようになります
        /// </Summary>
        [SerializeField] private HumanoidWeaponController enemyWeaponController;

        /// <Summary>
        /// When executed enemy start attack or guard.
        /// It can be executed with private.
        /// </Summary>
        private void OnActionStart()
        {
            enemyWeaponController.EnableWeapon();
        }

        /// <Summary>
        /// When executed enemy end attack or guard.
        /// </Summary>
        private void OnActionEnd()
        {
            enemyWeaponController.DisableWeapon();
        }
    }
}