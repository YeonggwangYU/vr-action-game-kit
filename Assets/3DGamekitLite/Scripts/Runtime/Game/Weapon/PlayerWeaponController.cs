using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace Gamekit3D
{
    public class PlayerWeaponController : MonoBehaviour
    {
        /// <Summary>
        /// どの音を再生するかを設定します
        /// </Summary>
        [SerializeField] private AudioClip seSwordCollision;

        //音を再生するためのコンポーネントの情報を格納する変数です
        [SerializeField] private AudioSource audioSource;

        /// <Summary>
        /// この変数を実行することでエフェクトが実行されます
        /// </Summary>
        [SerializeField] private ParticleSystem weaponParticleSystem;

        /// <Summary>
        /// Colliderの操作を行うための変数です
        /// </Summary>
        [SerializeField] private Collider weaponCollider;

        /// <Summary>
        /// 武器の当たり判定の有効/無効を切り替えるための変数です
        /// </Summary>
        [SerializeField] private MeleeWeapon meleeWeapon;

        /// <Summary>
        /// time until enable player weapon when guarded with enemy weapon
        /// </Summary>
        [SerializeField] private float playerWeaponEnableTime;

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
            weaponCollider.isTrigger = true;
        }

        /// <Summary>
        /// isTriggerを解除することで落としても地面に乗るようにします
        /// </Summary>
        private void DisableIsTrigger()
        {
            weaponCollider.isTrigger = false;
        }

        /// <Summary>
        /// enable player weapon attack.
        /// </Summary>
        private void EnableAttack()
        {
            weaponCollider.enabled = true;
        }

        /// <Summary>
        /// disable player weapon attack.
        /// </Summary>
        private void DisableAttack()
        {
            weaponCollider.enabled = false;
        }

        /// <Summary>
        /// 左手のXR Ray Interactorから呼び出され、武器を持ったのが左手であると設定します
        /// </Summary>
        public void OnSelectEnteredLeftHand()
        {
            EnableIsTrigger();
            _inputDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            
            Debug.Log($"PlayerWeaponController:OnSelectEnteredLeftHand:_inputDevice:{_inputDevice.name}");
            
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
        /// play sound and particle when player weapon hit to enemy weapon.
        /// </Summary>
        private void OnHitEnemyWeapon()
        {
            //武器が衝突する音を鳴らします
            audioSource.PlayOneShot(seSwordCollision);

            //火花を散らせます
            weaponParticleSystem.Play();
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
                //Get animation from Collider other.
                Animator enemyAnimator = _humanoidWeaponControllerIdentification.GetHumanoidAnimator();

                // check enemy is playing animation
                if (enemyAnimator.GetCurrentAnimatorStateInfo(0).length != 0)
                {
                    string currentClipName = enemyAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

                    // if Enemy is attacking, play enemy's Repell animation
                    if((currentClipName == "AttackFromLeft") || (currentClipName == "AttackFromRight") ||
                       (currentClipName == "AttackFromUpper"))
                    {
                        OnHitEnemyWeapon();
                        
                        //敵の攻撃を弾かれたモーションをオンにします
                        enemyAnimator.SetTrigger(AnimationRepelledHash);
                    
                    }
                    // if Enemy is guarding,  disable player weopon (parameter) seconds 
                    else if ((currentClipName == "GuardLeft") || (currentClipName == "GuardRight") ||
                             (currentClipName == "GuardUpper"))
                    {
                        OnHitEnemyWeapon();

                        // Disable hit damage.
                        DisableAttack();
                        // Disable hit effect.
                        meleeWeapon.EndAttack();

                        Debug.Log($"PlayerWeaponController:OnTriggerEnter:DisableAttack");
                        StartCoroutine(DelayCoroutine());
                    }
                    else
                    {
                        // nothing to do
                    }

                }
                
            }

            //当たったのが敵のGuardLeftColliderかを判定します
            //PlayerWeaponから呼び出すのは、敵のコンポーネント側では自分がどのコンポーネントのコライダーかを判別できないためです
            //※GuardLeftColliderとHumanoidController両方に当たってしまうことがあるので、if else文にしてガードを優先します
            if (other.gameObject.TryGetComponent<GuardLeftCollider>(out GuardLeftCollider guardLeftCollider))
            {
                // GuardLeftColliderのSetTriggerGuard()を呼び出し、ガードモーションさせます
                guardLeftCollider.SetTriggerGuard(gameObject);
            }
            //当たったのが敵かどうかを判定します
            else if (other.gameObject.TryGetComponent<HumanoidController>(out HumanoidController _humanoidControllerIdentification))
            {
                Debug.Log($"PlayerWeaponController:OnTriggerEnter:_inputDevice:{_inputDevice.name}");
                
                //コントローラーを振動させます。3つ目の引数が振動させる時間です
                _inputDevice.SendHapticImpulse(0, 0.5f, 0.1f);

                // Disable hit effect.
                meleeWeapon.EndAttack();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            //当たったのが敵かどうかを判定します
            if (other.gameObject.TryGetComponent<HumanoidController>(out HumanoidController _humanoidControllerIdentification))
            {
                // Enable hit effect
                meleeWeapon.BeginAttack(true);
            }
        }
        
        /// <Summary>
        /// Enable player weopon after (parameter) seconds
        /// </Summary>
        private IEnumerator DelayCoroutine()
        {
            // wait (parameter) second
            yield return new WaitForSecondsRealtime(playerWeaponEnableTime);

            // Enable hit damage.
            EnableAttack();
            // Enable hit effect.
            meleeWeapon.BeginAttack(true);
            
            Debug.Log($"PlayerWeaponController:OnTriggerEnter:EnableAttack");

        }

    }
}