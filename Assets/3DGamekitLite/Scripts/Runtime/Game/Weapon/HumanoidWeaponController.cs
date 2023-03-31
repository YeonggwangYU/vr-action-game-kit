using System;
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

        /// <Summary>
        /// attack point of the enemy weapon
        /// </Summary>
        [SerializeField] private int attackPoint = 1;

        private Vector3 _direction;
        private bool _isThrowingHit = false;

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

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"HumanoidWeaponController:OnCollisionEnter:collision.gameObject.name {other.gameObject.name}");

            // if not collision to player, end method
            if (other.gameObject.GetComponentInChildren<PlayerController>() == null)
            {
                return;
            }
            // if collision to player, apply damage
            else if (other.gameObject.GetComponentInChildren<PlayerController>().TryGetComponent<PlayerController>(out PlayerController _playerControllerIdentification))
            {
                //select damage target
                Damageable d = other.gameObject.GetComponentInChildren<Damageable>();

                //与えるダメージの量や方向などを格納します
                Damageable.DamageMessage data;

                data.amount = attackPoint;
                data.damager = this;
                data.direction = _direction.normalized;
                data.damageSource = this.transform.position;
                data.throwing = _isThrowingHit;
                data.stopCamera = false;

                //ダメージを与えます
                d.ApplyDamage(data);
            }
        }
    }
}