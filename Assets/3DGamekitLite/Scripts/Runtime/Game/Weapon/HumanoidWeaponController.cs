using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    public class HumanoidWeaponController : MonoBehaviour
    {

        /// <Summary>
        /// この変数の中の値を変更することでこの武器を持つ敵の対応したアニメーションが再生されます
        /// </Summary>
        [SerializeField] private Animator _animator;

        /// <Summary>
        /// この変数の中の値を変更することで武器の当たり判定の有無を操作します
        /// </Summary>
        [SerializeField] private BoxCollider _boxCollider;

        private void Start()
        {
            //攻撃モーションが始まるまでは当たり判定を無効化します
            DisableWeapon();
        }

        /// <Summary>
        /// 武器のColliderを有効にします。
        /// 色々なシチュエーションで使えるように他のスクリプトから呼び出せるようにpublicにします。
        /// </Summary>
        public void EnableWeapon()
        {
            _boxCollider.enabled = true;
        }

        /// <Summary>
        /// 武器のColliderを無効にします。
        /// 色々なシチュエーションで使えるように他のスクリプトから呼び出せるようにpublicにします。
        /// </Summary>
        public void DisableWeapon()
        {
            _boxCollider.enabled = false;
        }

    }
}