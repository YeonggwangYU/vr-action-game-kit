using _3DGamekitLite.Scripts.Runtime.Game.Weapon;
using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Enemies
{
    public class GuardRightCollider : MonoBehaviour
    {
        /// <Summary>
        /// Controll enemy from this variable
        /// </Summary>
        [SerializeField] private HumanoidController humanoidController;
        
        private static readonly int AnimationGuardRightHash = Animator.StringToHash("GuardRight");

        /// <Summary>
        /// This method is used to make guard motions from the outside.
        /// 外部からガードモーションをさせるためのメソッドです
        /// </Summary>
        public void SetTriggerGuard(GameObject playerObject)
        {
            if (playerObject.TryGetComponent<PlayerWeaponController>(
                    out PlayerWeaponController _playerWeaponControllerIdentification))
            {
                humanoidController.animator.SetTrigger(AnimationGuardRightHash);
            }
        }
    }
}