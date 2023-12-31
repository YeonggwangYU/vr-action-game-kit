﻿using System.Collections;
using _3DGamekitLite.Scripts.Runtime.Game.Enemies;
using UnityEngine;
using UnityEngine.XR;
using Vector3 = UnityEngine.Vector3;

namespace _3DGamekitLite.Scripts.Runtime.Game.Weapon
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
        /// Collider for make enemy guard and guard enemy attack.
        /// </Summary>
        [SerializeField] private Collider weaponCollider;

        /// <Summary>
        /// Collider for attack.
        /// </Summary>
        [SerializeField] private Collider weaponAttackCollider;

        /// <Summary>
        /// Use to get speed of player weapon.
        /// </Summary>
        [SerializeField] private Rigidbody rigidbody;

        /// <Summary>
        /// Use to display weapon locus.
        /// </Summary>
        [SerializeField] private TrailRenderer trailRenderer;

        /// <Summary>
        /// 武器の当たり判定の有効/無効を切り替えるための変数です
        /// </Summary>
        [SerializeField] private MeleeWeapon meleeWeapon;

        /// <Summary>
        /// time until enable player weapon when guarded with enemy weapon
        /// </Summary>
        [SerializeField] private float attackEnableTime;

        /// <Summary>
        /// speed that enable weapon attack.
        /// </Summary>
        [SerializeField] private float attackEnableSpeed;

        /// <Summary>
        /// acceleration that enable weapon attack.
        /// </Summary>
        [SerializeField] private float attackEnableAcceleration;

        /// <Summary>
        /// time to disable Trail Renderer.
        /// </Summary>
        [SerializeField] private float disableTrailRendererTime;

        //コントローラーを振動させる際に使用する変数です
        private InputDevice _inputDevice;
        
        /// <Summary>
        /// posititon before 1 frame.
        /// </Summary>
        private Vector3 _prevPosition;

        /// <Summary>
        /// velocity before 1 frame.
        /// </Summary>
        private Vector3 _prevVelocity;

        /// <Summary>
        /// Used to determine if acceleration exceeds a threshold over multiple frames
        /// </Summary>
        private int _frameCounter;
        [SerializeField] private int frameThreshold;
        
        /// <Summary>
        /// check attack is enable.
        /// </Summary>
        private bool isAttackEnable = false;
        
        /// <Summary>
        /// check attack is disabled by enemy guard.
        /// </Summary>
        private bool isAttackDisabledByEnemyGuard = false;

        /// <Summary>
        /// check weapon is grabbed.
        /// </Summary>
        private bool isGrabbed = false;

        
        /// <Summary>
        /// アニメーションのパラメータの打ち間違いを防ぐため、変数に格納してSetTriggerに渡します
        /// </Summary>
        private static readonly int AnimationRepelledHash = Animator.StringToHash("Repelled");


        private bool hapticResult = false;

        
        private void Start()
        {
            // hold first position.
            _prevPosition = transform.position;
        }
        
        /// <Summary>
        /// Set isTrigger to prevent the player from moving on his own when holding the weapon, and to prevent the weapon from shaking when he moves.
        /// Also set isGrabbed true and enable swing sound and particle.
        /// </Summary>
        private void OnGrabbed()
        {
            weaponCollider.isTrigger = true;
            weaponAttackCollider.isTrigger = true;
            isGrabbed = true;
        }

        /// <Summary>
        /// Disable isTrigger so that the weapon rides on the ground even if it is dropped.
        /// Also set isGrabbed false and disable swing sound and particle.
        /// </Summary>
        private void OnReleased()
        {
            weaponCollider.isTrigger = false;
            weaponAttackCollider.isTrigger = false;
            isGrabbed = false;
        }

        /// <Summary>
        /// enable player weapon attack.
        /// </Summary>
        private void EnableAttack()
        {
            isAttackEnable = true;
            
            weaponAttackCollider.enabled = true;
            meleeWeapon.BeginAttack(true);
            trailRenderer.enabled = true;
        }

        /// <Summary>
        /// disable player weapon attack.
        /// </Summary>
        private void DisableAttack()
        {
            isAttackEnable = false;

            weaponAttackCollider.enabled = false;
            meleeWeapon.EndAttack();
            StartCoroutine(DisableTrailRendererCoroutine());
        }

        /// <Summary>
        /// Enable player weopon after (parameter) seconds
        /// </Summary>
        private IEnumerator DisableTrailRendererCoroutine()
        {
            // wait (parameter) second
            yield return new WaitForSecondsRealtime(disableTrailRendererTime);

            // Enable hit damage.
            trailRenderer.enabled = false;
        }

        /// <Summary>
        /// Checks if acceleration thresholds are exceeded over multiple frames.
        /// </Summary>
        private bool CheckAccelerationThresholds()
        {
            if (_frameCounter >= 0)
            {
                _frameCounter++;
            }

            return (_frameCounter > frameThreshold);
        }
        
        /// <Summary>
        /// 左手のXR Ray Interactorから呼び出され、武器を持ったのが左手であると設定します
        /// </Summary>
        public void OnSelectEnteredLeftHand()
        {
            OnGrabbed();
            _inputDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        }

        /// <Summary>
        /// 右手のXR Ray Interactorから呼び出され、武器を持ったのが右手であると設定します
        /// </Summary>
        public void OnSelectEnteredRightHand()
        {
            OnGrabbed();
            _inputDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        }

        /// <Summary>
        /// 両手のXR Ray InteractorのHover Exitedから呼び出され、武器を離したと設定します
        /// </Summary>
        public void OnSelectExited()
        {
            OnReleased();

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

                        StartCoroutine(DisableAttackCoroutine());
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
            else if (other.gameObject.TryGetComponent<GuardRightCollider>(out GuardRightCollider guardRightCollider))
            {
                // Execute guard motion.
                guardRightCollider.SetTriggerGuard(gameObject);
            }
            else if (other.gameObject.TryGetComponent<GuardUpperCollider>(out GuardUpperCollider guardUpperCollider))
            {
                // Execute guard motion.
                guardUpperCollider.SetTriggerGuard(gameObject);
            }
            //当たったのが敵かどうかを判定します
            else if (other.gameObject.TryGetComponent<HumanoidTriggerController>(out HumanoidTriggerController humanoidTriggerControllerIdentification))
            {
                if (isAttackEnable)
                {
                    //コントローラーを振動させます。3つ目の引数が振動させる時間です
                    hapticResult = _inputDevice.SendHapticImpulse(0, 0.5f, 0.1f);
                }
            }
        }

        /// <Summary>
        /// Enable player weopon after (parameter) seconds
        /// </Summary>
        private IEnumerator DisableAttackCoroutine()
        {
            // Disable hit damage.
            isAttackDisabledByEnemyGuard = true;
            
            Debug.Log($"PlayerWeaponController:OnTriggerEnter:isAttackDisabledByEnemyGuard = true");
            
            // wait (parameter) second
            yield return new WaitForSecondsRealtime(attackEnableTime);

            // Enable hit damage.
            isAttackDisabledByEnemyGuard = false;
            
            Debug.Log($"PlayerWeaponController:OnTriggerEnter:isAttackDisabledByEnemyGuard = false");

        }

        /// <Summary>
        /// Enable player weapon attack when swing weapon fast.
        /// </Summary>
        private void Update()
        {
            if (!isGrabbed)
            {
                return;
            }
            
            // do nothing while deltaTime is 0.
            if (Mathf.Approximately(Time.deltaTime, 0))
            {
                return;
            }

            // get current position.
            Vector3 position = transform.position;

            // calculate current speed.
            Vector3 velocity = (position - _prevPosition) / Time.deltaTime;
            
            // calculate acceleration.
            Vector3 acceleration = velocity - _prevVelocity;

            if (velocity.magnitude > attackEnableSpeed)
            {
                Debug.Log($"PlayerWeaponController:Update():velocity.magnitude {velocity.magnitude}");
                Debug.Log($"PlayerWeaponController:Update():acceleration.magnitude {acceleration.magnitude}");
                if (acceleration.magnitude > attackEnableAcceleration)
                {
                    Debug.Log($"PlayerWeaponController:Update():_frameCounter {_frameCounter}");
                    if (CheckAccelerationThresholds())
                    {
                        if (!isAttackEnable)
                        {
                            if (isAttackDisabledByEnemyGuard)
                            {
                                return;
                            }
                            EnableAttack();
                        }

                        _frameCounter = 0;
                    }
                }
            }
            else if((velocity.magnitude <= attackEnableSpeed))
            {
                if (isAttackEnable)
                {
                    DisableAttack();
                }
            }
            
            // update previous frame position.
            _prevPosition = position;
            _prevVelocity = velocity;
        }
    }
}