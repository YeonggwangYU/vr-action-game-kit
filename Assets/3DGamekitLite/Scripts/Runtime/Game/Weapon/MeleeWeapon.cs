using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    public class MeleeWeapon : MonoBehaviour
    {
        private bool _isThrowingHit = false;
        private bool _inAttack = false;

        public bool throwingHit
        {
            get { return _isThrowingHit; }
            set { _isThrowingHit = value; }
        }

        public ParticleSystem hitParticlePrefab;
        
        public RandomAudioPlayer attackAudio;

        protected static RaycastHit[] RaycastHitCache = new RaycastHit[32];
        protected static Collider[] ColliderCache = new Collider[32];
        
        const int PARTICLE_COUNT = 10;
        private ParticleSystem[] _particlesPool = new ParticleSystem[PARTICLE_COUNT];

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
        /// whoever own the weapon is responsible for calling that. Allow to avoid "self harm"
        /// 自分の武器で自分を攻撃しないようにします
        /// </Summary>
        public void SetOwner(GameObject owner)
        {
            _damage.AvoidDamage(owner);
        }

        /// <Summary>
        /// 武器の攻撃を開始します
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
            
            _damage.RecordHitStart();
        }

        /// <Summary>
        /// 攻撃を終了したフラグを立てます
        /// </Summary>
        public void EndAttack()
        {
            //攻撃が終わったことを示します
            _inAttack = false;
            
            _damage.RecordHitEnd();
        }

        /// <Summary>
        /// 端末の性能などに依存せずに一定時間ごとに実行されるメソッドです
        /// Project Setting→Time→Fixed Timestepに実行間隔があります
        /// </Summary>
        private void FixedUpdate()
        {
            //攻撃中かどうかを判定しています
            if (_inAttack)
            {
                for (int i = 0; i < _damage.attackPoints.Length; ++i)
                {
                    int contacts = _damage.HitCount(i, ref RaycastHitCache);
                    
                    //光線に衝突したものの数だけ処理を繰り返します
                    for (int k = 0; k < contacts; ++k)
                    {
                        //光線に衝突したもののコライダーを取得します
                        Collider col = RaycastHitCache[k].collider;

                        //コライダーが存在していれば、ダメージ処理を実行します
                        if (col != null)
                        {
                            _damage.CheckDamage(col);
                            
                            //☆ここにパーティクル関連のメソッドを切り出す
                        }
                    }

                    _damage.RecordPreviousPosition(i);
                }
            }

        }


#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            _damage.OnDrawGizmosSelected();
        }

#endif
    }
}