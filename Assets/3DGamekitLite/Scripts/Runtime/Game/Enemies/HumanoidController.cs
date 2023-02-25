using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//NavMeshAgentを使うためにインポートします
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Gamekit3D
{
    public class HumanoidController : MonoBehaviour
    {

        /// <Summary>
        /// どの音を再生するかを設定します
        /// </Summary>
        [SerializeField] private AudioClip _se_attack_hit;
        [SerializeField] private AudioClip _se_death;

        //音を再生するためのコンポーネントの情報を格納する変数です
        [SerializeField] private AudioSource _audioSource;

        /// <Summary>
        /// この変数に対してUnityの画面上でプレイヤーを設定することで、敵がプレイヤーに向かいます
        /// </Summary>
        [SerializeField] private Transform _target;

        /// <Summary>
        /// この変数に対して敵の武器を設定することで、武器に付与されたスクリプトの関数を呼び出せるようになります
        /// </Summary>
        [SerializeField] private HumanoidWeaponController _enemyWeaponController;

        /// <Summary>
        /// 敵のヒットポイントが0以下になることで敵が倒れてリスポーンします
        /// 敵のヒットポイントを半減させる処理などを想定して小数点を扱える型にします
        /// </Summary>
        [SerializeField] private float _enemyHitPoint;

        /// <Summary>
        /// この変数に対してターゲットとしてプレイヤーを指定することで敵がプレイヤーに向かいます
        /// </Summary>
        [SerializeField] private NavMeshAgent _navMeshAgent;
        
        /// <Summary>
        /// 敵がこちらに近づいてくるまでの距離を設定します
        /// </Summary>
        [SerializeField] private float noticeDistance = 10.0f;

        /// <Summary>
        /// この変数の中の値を変更することで対応したアニメーションが再生されます
        /// </Summary>
        [SerializeField] private Animator _animator;

        /// <Summary>
        /// スローの速さを調整します
        /// </Summary>
        [SerializeField] private float _timeScale;

        /// <Summary>
        /// この変数を実行することでエフェクトが実行されます
        /// </Summary>
        [SerializeField] private ParticleSystem _particleSystem;
        
        /// <Summary>
        /// 左右移動の速さを調整します
        /// </Summary>
        [SerializeField] private int leftRightMoveSpeed;
        
        //文字列をハッシュという数字に予め変換しておくことで、処理の度に文字列化を行ないでよいようにして負荷を軽減します
        //また、文字列の打ち間違いをしないようにします
        private static readonly int AnimationGotHitHash = Animator.StringToHash("GotHit");
        private static readonly int AnimationMovingHash = Animator.StringToHash("Moving");
        private static readonly int AnimationBattleRandomHash = Animator.StringToHash("BattleRandom");
        
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
                //敵に攻撃がヒットした音を鳴らします
                _audioSource.PlayOneShot(_se_attack_hit);

                //敵のヒットポイントを減らします
                _enemyHitPoint = Damage(_enemyHitPoint);

                //敵のヒットポイントが無くなったら倒れてリスポーンします
                if (_enemyHitPoint <= 0)
                {
                    //時間を一定時間遅くした後にもとに戻します
                    StartCoroutine(DelayCoroutine());

                    //ショックウェーブを発生させます
                    _particleSystem.Play();

                    //敵が倒れるモーションを再生します
                    _animator.SetTrigger(AnimationDeadHash);

                    //倒れるモーションを待ってから敵を消滅させます
                    Destroy(gameObject, _timeEnemyDead);
                }

                //敵の攻撃が当たったことを示すパラメーターをオンにします
                _animator.SetTrigger(AnimationGotHitHash);

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
        /// 攻撃モーションの途中で呼び出されて、当たり判定を無効化する関数を呼び出します
        /// privateでも呼び出されることは可能です
        /// </Summary>
        private void OnAttackStart()
        {
            _enemyWeaponController.EnableAttack();
        }

        /// <Summary>
        /// 攻撃モーションの途中で呼び出されて、Colliderを無効化することで当たり判定を消します
        /// </Summary>
        private void OnAttackEnd()
        {
            _enemyWeaponController.DisableAttack();
        }

        /// <Summary>
        /// 敵が倒れたときにアニメーションから呼び出される処理を定義します
        /// </Summary>
        private void OnDeath()
        {
            _audioSource.PlayOneShot(_se_death);
        }


        /// <Summary>
        /// ゲームの起動中継続して実行される処理です
        /// </Summary>
        private void Update()
        {
            if (Vector3.Distance(_target.position, _navMeshAgent.transform.position) <
                noticeDistance)
            {
               //プレイヤーの位置まで移動します
               _navMeshAgent.SetDestination(_target.position);
            }

            //敵が動いたら歩行アニメーションを再生します
            //NavMeshAgentの変数のパラメータであるvelocity.magnitudeが速度を表すので、それが少しでも動いたらというのを> 0.1fという形で表します
            if (_navMeshAgent.velocity.magnitude > 0.1f)
            {
                _animator.SetBool(AnimationMovingHash, true);
            }
            else
            {
                _animator.SetBool(AnimationMovingHash, false);
            }

            //プレイヤーと敵の距離がNavMeshAgentで設定している停止距離より少し近くなったら敵が攻撃を開始します
            if (Vector3.Distance(_target.position, _navMeshAgent.transform.position) < _navMeshAgent.stoppingDistance + 0.5f)
            {
                //プレイヤーの位置から自分の位置を引くことで、敵から見たプレイヤーの位置を算出します（★原理がよくわかっていない） https://gomafrontier.com/unity/2883
                //y軸を固定することで敵が上を向かないようにします
                var direction = _target.position - transform.position;
                direction.y = 0;

                //敵がプレイヤーの方向を向くようにする
                //振り向く速さはLerp()の第三引数で調整する
                var lookRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 0.1f);

                //ランダムに攻撃パターンを発生させます
                int attackPattern = Random.Range(1, 6);
                switch (attackPattern)
                {
                    case 1:
                    case 2:
                    case 3:
                        //ランダムに攻撃を行います
                        _animator.SetInteger(AnimationBattleRandomHash, attackPattern);
                        break;
                    case 4:
                        //プレイヤーからみて右に動きます
                        //参考：https://nekojara.city/unity-circular-motion
                        transform.RotateAround(_target.position, Vector3.up, 360 / leftRightMoveSpeed * Time.deltaTime);
                        break;
                    case 5:
                        //プレイヤーからみて左に動きます
                        transform.RotateAround(_target.position, Vector3.down, 360 / leftRightMoveSpeed * Time.deltaTime);
                        break;
                }

            }
        }

    }
}