using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//NavMeshAgentを使うためにインポートします
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Gamekit3D
{
    public class HumanoidTriggerController : MonoBehaviour
    {

        /// <Summary>
        /// どの音を再生するかを設定します
        /// </Summary>
        [SerializeField] private AudioClip _se_attack_hit;

        //音を再生するためのコンポーネントの情報を格納する変数です
        [SerializeField] private AudioSource _audioSource;

        /// <Summary>
        /// 敵のヒットポイントが0以下になることで敵が倒れてリスポーンします
        /// 敵のヒットポイントを半減させる処理などを想定して小数点を扱える型にします
        /// </Summary>
        [SerializeField] private float _enemyHitPoint;

        /// <Summary>
        /// この変数の中の値を変更することで対応したアニメーションが再生されます
        /// </Summary>
        [SerializeField] public Animator animator;

        /// <Summary>
        /// スローの速さを調整します
        /// </Summary>
        [SerializeField] private float _timeScale;

        /// <Summary>
        /// この変数を実行することでエフェクトが実行されます
        /// </Summary>
        [SerializeField] private ParticleSystem _particleSystem;
        
        /// <Summary>
        /// use for knockback enemy
        /// </Summary>
        [SerializeField] private NavMeshAgent _navMeshAgent;

        /// <Summary>
        /// knockback distance
        /// </Summary>
        [SerializeField] private float knockbackFrame;

        /// <Summary>
        /// how long to knockback
        /// </Summary>
        [SerializeField] private float knockbackSeconds;

        //文字列をハッシュという数字に予め変換しておくことで、処理の度に文字列化を行ないでよいようにして負荷を軽減します
        //また、文字列の打ち間違いをしないようにします
        private static readonly int AnimationGotHitHash = Animator.StringToHash("GotHit");
        private static readonly int AnimationDeadHash = Animator.StringToHash("Dead");


        /// <Summary>
        /// 敵が倒れるまでにかかる時間です
        /// </Summary>
        private readonly float _timeEnemyDead = 1.3f;

        /// <Summary>
        /// 敵を倒したときのスローを解除するまでの時間です
        /// </Summary>
        private readonly float _delayTime = 2.3f;

        /// <Summary>
        /// 敵の攻撃パターンを設定します
        /// </Summary>
        private int _attackPattern;
        
        /// <Summary>
        /// set velocity of knockback when attacked by player weapon.
        /// </Summary>
        private Vector3 _knockbackVelocity = Vector3.zero;
        
        /// <Summary>
        /// 敵にダメージを与えてヒットポイントを減らします
        /// 将来的にステータス異常などプレイヤーの武器以外からのダメージを想定してパブリックにします
        /// </Summary>
        public float Damage(float inputEnemyHitPoint)
        {
            
            inputEnemyHitPoint--;
            return inputEnemyHitPoint;
        }


        /// <Summary>
        /// プレイヤーの武器が敵本体に設定したColliderに触れると実行される処理を書きます
        /// </Summary>
        private void OnTriggerEnter(Collider other)
        {
            //当たったのがプレイヤーの武器かどうかを判定します
            if (other.gameObject.TryGetComponent<PlayerWeaponController>(out PlayerWeaponController _playerWeaponControllerIdentification))
            {
                Debug.Log($"HumanoidController:OnTriggerEnter:AttackHit");
                
                //敵に攻撃がヒットした音を鳴らします
                _audioSource.PlayOneShot(_se_attack_hit);

                //敵のヒットポイントを減らします
                _enemyHitPoint = Damage(_enemyHitPoint);

                if (_enemyHitPoint > 0)
                {
                    //knockback enemy.
                    StartCoroutine(KnockbackCoroutine());
                }
                //敵のヒットポイントが無くなったら倒れてリスポーンします
                else if (_enemyHitPoint <= 0)
                {
                    //時間を一定時間遅くした後にもとに戻します
                    StartCoroutine(DelayCoroutine());

                    //ショックウェーブを発生させます
                    _particleSystem.Play();

                    //敵が倒れるモーションを再生します
                    animator.SetTrigger(AnimationDeadHash);

                    //倒れるモーションを待ってから敵を消滅させます
                    Destroy(gameObject, _timeEnemyDead);
                }

                //敵の攻撃が当たったことを示すパラメーターをオンにします
                animator.SetTrigger(AnimationGotHitHash);

            }
        }

        /// <Summary>
        /// 敵を倒した際にスローにしてから戻します
        /// </Summary>
        private IEnumerator DelayCoroutine()
        {
            //時間の流れを遅くします
            Time.timeScale = _timeScale;

            // 敵が倒れるまで待ちます
            yield return new WaitForSecondsRealtime(_delayTime);

            //時間の流れを戻します
            Time.timeScale = 1.0f;
        }

        /// <Summary>
        /// knockback few seconds.
        /// </Summary>
        private IEnumerator KnockbackCoroutine()
        {
            //set knockback velocity
            _knockbackVelocity = (-transform.forward * knockbackFrame);

            //wait for knockback time.
            yield return new WaitForSecondsRealtime(knockbackSeconds);

            //clear knockback velocity
            _knockbackVelocity = Vector3.zero;
        }

        private void Update()
        {
            //When attacked by player weapon, knockback.
            if (_knockbackVelocity != Vector3.zero) {
                _navMeshAgent.Move(_knockbackVelocity * Time.deltaTime);
            }
        }
    }
}