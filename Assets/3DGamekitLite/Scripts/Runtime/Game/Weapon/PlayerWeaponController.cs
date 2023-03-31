using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace Gamekit3D
{
    public class PlayerWeaponController : MonoBehaviour
    {
        /// <Summary>
        /// どの音を再生するかを設定します
        /// </Summary>
        [SerializeField] private AudioClip _se_sword_collision;

        //音を再生するためのコンポーネントの情報を格納する変数です
        [SerializeField] private AudioSource _audioSource;

        /// <Summary>
        /// この変数を実行することでエフェクトが実行されます
        /// </Summary>
        [SerializeField] private ParticleSystem _particleSystem;

        /// <Summary>
        /// Colliderの操作を行うための変数です
        /// </Summary>
        [SerializeField] private Collider _collider;

        /// <Summary>
        /// 武器の当たり判定の有効/無効を切り替えるための変数です
        /// </Summary>
        [SerializeField] private MeleeWeapon _meleeWeapon;

        /// <Summary>
        /// time until enable player weapon when guarded with enemy weapon
        /// </Summary>
        [SerializeField] private float playerWeaponEnableTime = 1.0f;

        //コントローラーを振動させる際に使用する変数です
        private InputDevice _inputDevice;

        /// <Summary>
        /// アニメーションのパラメータの打ち間違いを防ぐため、変数に格納してSetTriggerに渡します
        /// </Summary>
        private static readonly int AnimationRepelledHash = Animator.StringToHash("Repelled");


        /// <Summary>
        /// isTriggerを設定することで武器を持ったときにプレイヤーが勝手に移動しないようにし、移動したときに武器がブレないようにします
        /// </Summary>
        private void EnableIsTrigger()
        {
            _collider.isTrigger = true;
        }

        /// <Summary>
        /// isTriggerを解除することで落としても地面に乗るようにします
        /// </Summary>
        private void DisableIsTrigger()
        {
            _collider.isTrigger = false;
        }

        /// <Summary>
        /// enable player weapon attack.
        /// </Summary>
        private void EnableAttack()
        {
            _collider.enabled = true;
        }

        /// <Summary>
        /// disable player weapon attack.
        /// </Summary>
        private void DisableAttack()
        {
            _collider.enabled = false;
        }

        /// <Summary>
        /// 左手のXR Ray Interactorから呼び出され、武器を持ったのが左手であると設定します
        /// </Summary>
        public void OnSelectEnteredLeftHand()
        {
            EnableIsTrigger();
            _inputDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        }

        /// <Summary>
        /// 右手のXR Ray Interactorから呼び出され、武器を持ったのが右手であると設定します
        /// </Summary>
        public void OnSelectEnteredRightHand()
        {
            EnableIsTrigger();
            _inputDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        }

        /// <Summary>
        /// 両手のXR Ray InteractorのHover Exitedから呼び出され、武器を離したと設定します
        /// </Summary>
        public void OnSelectExited()
        {
            DisableIsTrigger();

            //LeftEyeがXRNode変数の初期値なので初期値に戻します
            _inputDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftEye);
        }

        /// <Summary>
        /// プレイヤーの武器が敵の武器に設定したColliderに触れると火花を散らせて衝突音を鳴らします
        /// プレイヤーの武器が敵本体に設定したColliderに触れるとコントローラーを振動させます
        /// 移動時にも武器がブレないようにIs Triggerをオンにしているため、衝突判定を取るにはOnTriggerEnterを使用します
        /// </Summary>
        private void OnTriggerEnter(Collider other)
        {
            // Log to verify operation 
            Debug.Log($"PlayerWeaponController:OnTriggerEnter {other.gameObject.name}");
            
            //当たったのが敵の武器かどうかを判定します
            if (other.gameObject.TryGetComponent<HumanoidWeaponController>(out HumanoidWeaponController _humanoidWeaponControllerIdentification))
            {
                //武器が衝突する音を鳴らします
                _audioSource.PlayOneShot(_se_sword_collision);

                //火花を散らせます
                _particleSystem.Play();
                
                //Get animation from Collider other.
                Animator enemyAnimator = other.gameObject.GetComponentInParent<Animator>();
                Debug.Log($"PlayerWeaponController:OnTriggerEnter:enemyAnimator {enemyAnimator}");
                Debug.Log($"PlayerWeaponController:OnTriggerEnter:enemyAnimator.GetCurrentAnimatorStateInfo(0).length {enemyAnimator.GetCurrentAnimatorStateInfo(0).length}");

                // check enemy is playing animation
                if (enemyAnimator.GetCurrentAnimatorStateInfo(0).length != 0)
                {
                    string currentClipName = enemyAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                    Debug.Log($"PlayerWeaponController:OnTriggerEnter:enemyAnimator.GetCurrentAnimatorClipInfo(0)[0] {enemyAnimator.GetCurrentAnimatorClipInfo(0)[0]}");

                    // if Enemy is attacking, play enemy's Repell animation
                    if((currentClipName == "AttackFromLeft") || (currentClipName == "AttackFromRight") ||
                       (currentClipName == "AttackFromUpper"))
                    {
                        //敵の攻撃が当たったことを示すパラメーターをオンにします
                        //Triggerの場合は自動でオフになるため、Boolのようにfalseにする処理は必要ありません
                        enemyAnimator.SetTrigger(AnimationRepelledHash);
                    
                    }
                    // if Enemy is guarding,  disable player weopon (parameter) seconds 
                    else if ((currentClipName == "GuardLeft") || (currentClipName == "GuardRight") ||
                             (currentClipName == "GuardUpper"))
                    {
                        DisableAttack();
                        StartCoroutine(DelayCoroutine());
                    }
                    else
                    {
                        // nothing to do
                    }

                }
                
            }

            //当たったのが敵のGuardLeftColliderかを判定します
            //※GuardLeftColliderとHumanoidController両方に当たってしまうことがあるので、if else文にしてガードを優先します
            if (other.gameObject.TryGetComponent<GuardLeftCollider>(out GuardLeftCollider guardLeftCollider))
            {
                // GuardLeftColliderのSetTriggerGuard()を呼び出し、ガードモーションさせます
                guardLeftCollider.SetTriggerGuard(gameObject);
            }
            //当たったのが敵かどうかを判定します
            else if (other.gameObject.TryGetComponent<HumanoidController>(out HumanoidController _humanoidControllerIdentification))
            {
                //コントローラーを振動させます。3つ目の引数が振動させる時間です
                _inputDevice.SendHapticImpulse(0, 0.5f, 0.1f);

                //当たり判定を無効化します
                _meleeWeapon.EndAttack();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            //当たったのが敵かどうかを判定します
            if (other.gameObject.TryGetComponent<HumanoidController>(out HumanoidController _humanoidControllerIdentification))
            {
                //当たり判定を無効化します
                _meleeWeapon.BeginAttack(true);
            }
        }
        
        /// <Summary>
        /// Enable player weopon after (parameter) seconds
        /// </Summary>
        private IEnumerator DelayCoroutine()
        {
            // wait (parameter) second
            yield return new WaitForSecondsRealtime(playerWeaponEnableTime);

            EnableAttack();
        }

    }
}