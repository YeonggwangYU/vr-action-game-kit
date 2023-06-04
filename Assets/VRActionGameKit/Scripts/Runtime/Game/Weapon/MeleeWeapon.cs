using System;
using System.Collections.Generic;
using _3DGamekitLite.Scripts.Runtime.Game.Audio;
using _3DGamekitLite.Scripts.Runtime.Game.DamageSystem;
using _3DGamekitLite.Scripts.Runtime.Game.Enemies;
using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Weapon
{
    public class MeleeWeapon : MonoBehaviour
    {
        public int damage = 1;

        //MeleeWeapon以外からは現状呼ばれていないので、分割の仕方によってはprivateで良さそうです
        [System.Serializable]
        public class AttackPoint
        {
            public float radius;
            public Vector3 offset;
            public Transform attackRoot;

#if UNITY_EDITOR
            //editor only as it's only used in editor to display the path of the attack that is used by the raycast
            [NonSerialized] public List<Vector3> previousPositions = new List<Vector3>();
#endif

        }

        public ParticleSystem hitParticlePrefab;
        public LayerMask targetLayers;

        public AttackPoint[] attackPoints = new AttackPoint[0];

        public TimeEffect[] effects;

        [Header("Audio")] public RandomAudioPlayer hitAudio;
        public RandomAudioPlayer attackAudio;

        public bool throwingHit
        {
            get { return m_IsThrowingHit; }
            set { m_IsThrowingHit = value; }
        }

        protected GameObject m_Owner;

        protected Vector3[] m_PreviousPos = null;
        protected Vector3 m_Direction;

        protected bool m_IsThrowingHit = false;
        protected bool inAttack = false;

        const int PARTICLE_COUNT = 10;
        protected ParticleSystem[] m_ParticlesPool = new ParticleSystem[PARTICLE_COUNT];
        protected int m_CurrentParticle = 0;

        protected static RaycastHit[] s_RaycastHitCache = new RaycastHit[32];
        protected static Collider[] s_ColliderCache = new Collider[32];

        /// <Summary>
        /// 所定の数だけパーティクルを予め作成して、動きを止めておきます
        /// </Summary>
        private void Awake()
        {
            if (hitParticlePrefab != null)
            {
                for (int i = 0; i < PARTICLE_COUNT; ++i)
                {
                    m_ParticlesPool[i] = Instantiate(hitParticlePrefab);
                    m_ParticlesPool[i].Stop();
                }
            }
        }

        /// <Summary>
        /// 使っていないので、消しても問題無さそうです
        /// </Summary>
        private void OnEnable()
        {

        }

        /// <Summary>
        /// whoever own the weapon is responsible for calling that. Allow to avoid "self harm"
        /// 自分の武器で自分を攻撃しないようにします
        /// </Summary>
        public void SetOwner(GameObject owner)
        {
            m_Owner = owner;
        }

        /// <Summary>
        /// 攻撃を開始したフラグを立て、攻撃開始位置を記憶します
        /// </Summary>
        public void BeginAttack(bool thowingAttack)
        {
            Debug.Log($"MeleeWeapon:BeginAttack");
            
            //武器を振る際の音を流します
            if (attackAudio != null)
                attackAudio.PlayRandomClip();
            
            //特に攻撃に関連していなさそうな変数に見えます
            throwingHit = thowingAttack;

            //攻撃中であることを示し、武器が敵に当たった際にダメージやエフェクトが発生するようになります
            inAttack = true;

            //攻撃開始地点の座標を取得するための変数の値を初期化しています
            m_PreviousPos = new Vector3[attackPoints.Length];

            //アタックポイント毎に攻撃開始地点の座標を取得します
            for (int i = 0; i < attackPoints.Length; ++i)
            {
                Vector3 worldPos = attackPoints[i].attackRoot.position +
                                   attackPoints[i].attackRoot.TransformVector(attackPoints[i].offset);
                m_PreviousPos[i] = worldPos;

#if UNITY_EDITOR
                attackPoints[i].previousPositions.Clear();
                attackPoints[i].previousPositions.Add(m_PreviousPos[i]);
#endif
            }
        }

        /// <Summary>
        /// 攻撃を終了したフラグを立てます
        /// </Summary>
        public void EndAttack()
        {
            Debug.Log($"MeleeWeapon:EndAttack");
            
            //攻撃中が終わったことを示します
            inAttack = false;


#if UNITY_EDITOR
            for (int i = 0; i < attackPoints.Length; ++i)
            {
                attackPoints[i].previousPositions.Clear();
            }
#endif
        }

        /// <Summary>
        /// 端末の性能などに依存せずに一定時間ごとに実行されるメソッドです
        /// Project Setting→Time→Fixed Timestepに実行間隔があります
        /// </Summary>
        private void FixedUpdate()
        {
            //攻撃中かどうかを判定しています
            if (inAttack)
            {
                for (int i = 0; i < attackPoints.Length; ++i)
                {
                    AttackPoint pts = attackPoints[i];

                    //武器の先にあるアタックポイントの現在の座標を取得します
                    Vector3 worldPos = pts.attackRoot.position + pts.attackRoot.TransformVector(pts.offset);
                    
                    //アタックポイントの現在の座標と過去の座標から攻撃の進度及び方向を取得します
                    Vector3 attackVector = worldPos - m_PreviousPos[i];

                    //攻撃の進度が極端に少ない場合に、小さい値を格納します
                    if (attackVector.magnitude < 0.001f)
                    {
                        // A zero vector for the sphere cast don't yield any result, even if a collider overlap the "sphere" created by radius. 
                        // so we set a very tiny microscopic forward cast to be sure it will catch anything overlaping that "stationary" sphere cast
                        attackVector = Vector3.forward * 0.0001f;
                    }

                    //攻撃の方向に見えない光線Rayを発生させます
                    Ray r = new Ray(worldPos, attackVector.normalized);

                    //発生した光線に衝突したものを変数に格納します
                    int contacts = Physics.SphereCastNonAlloc(r, pts.radius, s_RaycastHitCache, attackVector.magnitude,
                        ~0,
                        QueryTriggerInteraction.Ignore);

                    //光線に衝突したものの数だけ処理を繰り返します
                    for (int k = 0; k < contacts; ++k)
                    {
                        //光線に衝突したもののコライダーを取得します
                        Collider col = s_RaycastHitCache[k].collider;

                        //コライダーが存在していれば、ダメージ処理を実行します
                        if (col != null)
                            CheckDamage(col, pts);
                    }

                    m_PreviousPos[i] = worldPos;

#if UNITY_EDITOR
                    pts.previousPositions.Add(m_PreviousPos[i]);
#endif
                }
            }
        }

        /// <Summary>
        /// ダメージを与える相手かを判定した上でダメージを与え、エフェクトを発生させます
        /// 戻り値がfalseの場合は武器が弾かれる処理が実行されるような記述がありますが、
        /// このプロジェクト内では弾かれる処理は無いようです（3D Game Kit無印の方にあると想定します）
        /// </Summary>
        private bool CheckDamage(Collider other, AttackPoint pts)
        {
            //光線がぶつかった相手にDamageableがアタッチされていなければ処理を終了します
            Damageable d = other.GetComponent<Damageable>();
            if (d == null)
            {
                return false;
            }

            //衝突したのが武器を持っている者であれば処理を終了します
            if (d.gameObject == m_Owner)
                return true; //ignore self harm, but do not end the attack (we don't "bounce" off ourselves)

            //Inspectorで設定したターゲットレイヤーのオブジェクトであれば処理を終了します
            if ((targetLayers.value & (1 << other.gameObject.layer)) == 0)
            {
                //hit an object that is not in our layer, this end the attack. we "bounce" off it
                return false;
            }

            //When hit to guard collider, don't enable particle.
            if (other.gameObject.TryGetComponent<GuardLeftCollider>(out GuardLeftCollider guardLeftCollider) ||
                other.gameObject.TryGetComponent<GuardRightCollider>(out GuardRightCollider guardRightCollider) ||
                other.gameObject.TryGetComponent<GuardUpperCollider>(out GuardUpperCollider guardUpperCollider))
            {
                return false;
            }

            //攻撃を当てた際に音を発生させます
            if (hitAudio != null)
            {
                var renderer = other.GetComponent<Renderer>();
                if (!renderer)
                    renderer = other.GetComponentInChildren<Renderer> ();
                if (renderer)
                    hitAudio.PlayRandomClip (renderer.sharedMaterial);
                else
                    hitAudio.PlayRandomClip ();
            }

            //与えるダメージの量や方向などを格納します
            Damageable.DamageMessage data;

            data.amount = damage;
            data.damager = this;
            data.direction = m_Direction.normalized;
            data.damageSource = m_Owner.transform.position;
            data.throwing = m_IsThrowingHit;
            data.stopCamera = false;

            //ダメージを与えます
            d.ApplyDamage(data);

            //攻撃がヒットした際のパーティクルを発生させます
            if (hitParticlePrefab != null)
            {
                m_ParticlesPool[m_CurrentParticle].transform.position = pts.attackRoot.transform.position;
                m_ParticlesPool[m_CurrentParticle].time = 0;
                m_ParticlesPool[m_CurrentParticle].Play();
                m_CurrentParticle = (m_CurrentParticle + 1) % PARTICLE_COUNT;
            }

            return true;
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            for (int i = 0; i < attackPoints.Length; ++i)
            {
                AttackPoint pts = attackPoints[i];

                if (pts.attackRoot != null)
                {
                    Vector3 worldPos = pts.attackRoot.TransformVector(pts.offset);
                    Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.4f);
                    Gizmos.DrawSphere(pts.attackRoot.position + worldPos, pts.radius);
                }

                if (pts.previousPositions.Count > 1)
                {
                    UnityEditor.Handles.DrawAAPolyLine(10, pts.previousPositions.ToArray());
                }
            }
        }

#endif
    }
}