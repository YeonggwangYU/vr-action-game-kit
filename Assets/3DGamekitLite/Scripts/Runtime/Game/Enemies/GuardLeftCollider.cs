using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    public class GuardLeftCollider : MonoBehaviour
    {
        /// <Summary>
        /// この変数の中の値を変更することで対応したアニメーションが再生されます
        /// </Summary>
        [SerializeField] private Animator _animator;

        private static readonly int AnimationGuardLeftHash = Animator.StringToHash("GuardLeft");

        /// <Summary>
        /// This method is used to make guard motions from the outside.
        /// 外部からガードモーションをさせるためのメソッドです
        /// </Summary>
        public void SetTriggerGuard(GameObject playerObject)
        {
            // Log to verify operation 
            Debug.Log($"SetTriggerGuard:{playerObject}");

            if (playerObject.TryGetComponent<PlayerWeaponController>(
                    out PlayerWeaponController _playerWeaponControllerIdentification))
            {
                _animator.SetTrigger(AnimationGuardLeftHash);
            }
        }

    }
}