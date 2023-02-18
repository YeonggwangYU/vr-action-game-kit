using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
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
            [NonSerialized] public List<Vector3> PreviousPositions = new List<Vector3>();
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
            get { return _isThrowingHit; }
            set { _isThrowingHit = value; }
        }

        private GameObject _owner;

        private Vector3[] _previousPos = null;
        private Vector3 _direction;

        private bool _isThrowingHit = false;
        private bool _inAttack = false;

        const int PARTICLE_COUNT = 10;
        private ParticleSystem[] _particlesPool = new ParticleSystem[PARTICLE_COUNT];
        private int _currentParticle = 0;

        protected static RaycastHit[] RaycastHitCache = new RaycastHit[32];
        protected static Collider[] ColliderCache = new Collider[32];

        private Damage _damage = new Damage();

        /// <Summary>
        /// 所定の数だけパーティクルを予め作成して、動きを止めておきます
        /// </Summary>
        private void Awake()
        {
            if (hitParticlePrefab != null)
            {
                for (int i = 0; i < PARTICLE_COUNT; ++i)
                {
                    _particlesPool[i] = Instantiate(hitParticlePrefab);
                    _particlesPool[i].Stop();
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
            _owner = owner;
        }

        /// <Summary>
        /// 攻撃を開始したフラグを立て、攻撃開始位置を記憶します
        /// </Summary>
        public void BeginAttack(bool thowingAttack)
        {
            //武器を振る際の音を流します
            if (attackAudio != null)
                attackAudio.PlayRandomClip();
            
            //特に攻撃に関連していなさそうな変数に見えます
            throwingHit = thowingAttack;

            //攻撃中であることを示し、武器が敵に当たった際にダメージやエフェクトが発生するようになります
            _inAttack = true;

            //攻撃開始地点の座標を取得するための変数の値を初期化しています
            _previousPos = new Vector3[attackPoints.Length];

            //アタックポイント毎に攻撃開始地点の座標を取得します
            for (int i = 0; i < attackPoints.Length; ++i)
            {
                Vector3 worldPos = attackPoints[i].attackRoot.position +
                                   attackPoints[i].attackRoot.TransformVector(attackPoints[i].offset);
                _previousPos[i] = worldPos;

#if UNITY_EDITOR
                attackPoints[i].PreviousPositions.Clear();
                attackPoints[i].PreviousPositions.Add(_previousPos[i]);
#endif
            }
        }

        /// <Summary>
        /// 攻撃を終了したフラグを立てます
        /// </Summary>
        public void EndAttack()
        {
            //攻撃中が終わったことを示します
            _inAttack = false;


#if UNITY_EDITOR
            for (int i = 0; i < attackPoints.Length; ++i)
            {
                attackPoints[i].PreviousPositions.Clear();
            }
#endif
        }

        /// <Summary>
        /// 端末の性能などに依存せずに一定時間ごとに実行されるメソッドです
        /// Project Setting→Time→Fixed Timestepに実行間隔があります
        /// </Summary>
        private void FixedUpdate()
        {
            _damage.HitCheck(_inAttack, attackPoints, _previousPos, RaycastHitCache);
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
            if (d.gameObject == _owner)
                return true; //ignore self harm, but do not end the attack (we don't "bounce" off ourselves)

            //Inspectorで設定したターゲットレイヤーのオブジェクトであれば処理を終了します
            if ((targetLayers.value & (1 << other.gameObject.layer)) == 0)
            {
                //hit an object that is not in our layer, this end the attack. we "bounce" off it
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
            data.direction = _direction.normalized;
            data.damageSource = _owner.transform.position;
            data.throwing = _isThrowingHit;
            data.stopCamera = false;

            //ダメージを与えます
            d.ApplyDamage(data);

            //攻撃がヒットした際のパーティクルを発生させます
            if (hitParticlePrefab != null)
            {
                _particlesPool[_currentParticle].transform.position = pts.attackRoot.transform.position;
                _particlesPool[_currentParticle].time = 0;
                _particlesPool[_currentParticle].Play();
                _currentParticle = (_currentParticle + 1) % PARTICLE_COUNT;
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

                if (pts.PreviousPositions.Count > 1)
                {
                    UnityEditor.Handles.DrawAAPolyLine(10, pts.PreviousPositions.ToArray());
                }
            }
        }

#endif
    }
}