using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gamekit3D
{
    public class GuardUpperCollider : MonoBehaviour
    {
        /// <Summary>
        /// Controll enemy from this variable
        /// </Summary>
        [SerializeField] private HumanoidController humanoidController;
        
        private static readonly int AnimationGuardUpperHash = Animator.StringToHash("GuardUpper");

        /// <Summary>
        /// This method is used to make guard motions from the outside.
        /// 外部からガードモーションをさせるためのメソッドです
        /// </Summary>
        public void SetTriggerGuard(GameObject playerObject)
        {
            if (playerObject.TryGetComponent<PlayerWeaponController>(
                    out PlayerWeaponController _playerWeaponControllerIdentification))
            {
                humanoidController.animator.SetTrigger(AnimationGuardUpperHash);
            }
        }
    }
}