using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    public class Damage : MonoBehaviour
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

        public LayerMask targetLayers;

        public AttackPoint[] attackPoints = new AttackPoint[0];

        public TimeEffect[] effects;

        [Header("Audio")] public RandomAudioPlayer hitAudio;

        private GameObject _owner;

        private Vector3[] _previousPos = null;
        private Vector3 _direction;

        private int _currentParticle = 0;

        private AttackPoint _pts;

        
        /// <Summary>
        /// 攻撃を開始したことを受けて、攻撃がヒットした箇所の記録を開始します
        /// </Summary>
        public void RecordHitStart()
        {
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
        /// 攻撃を終了したことを受けて、攻撃がヒットした箇所の記録を終了します
        /// </Summary>
        public void RecordHitEnd()
        {
#if UNITY_EDITOR
            for (int i = 0; i < attackPoints.Length; ++i)
            {
                attackPoints[i].PreviousPositions.Clear();
            }
#endif
        }

        /// <Summary>
        /// 指定したオブジェクトにダメージを与えないフラグを立てます
        /// </Summary>
        public void AvoidDamage(GameObject owner)
        {
            _owner = owner;
        }

        /// <Summary>
        /// 攻撃開始地点から
        /// </Summary>
        public int HitCount(int i, ref RaycastHit[] raycastHitCache)
        {
            _pts = attackPoints[i];

            //武器の先にあるアタックポイントの現在の座標を取得します
            Vector3 worldPos = _pts.attackRoot.position + _pts.attackRoot.TransformVector(_pts.offset);
            
            //アタックポイントの現在の座標と過去の座標から攻撃の進度及び方向を取得します
            Vector3 attackVector = worldPos - _previousPos[i];

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
            int contacts = Physics.SphereCastNonAlloc(r, _pts.radius, raycastHitCache, attackVector.magnitude,
                ~0,
                QueryTriggerInteraction.Ignore);

            return contacts;
        }

        /// <Summary>
        /// 攻撃のヒットポジションを記録します
        /// </Summary>
        public void RecordPreviousPosition(int i)
        {
            _previousPos[i] = worldPos;

#if UNITY_EDITOR
            _pts.PreviousPositions.Add(_previousPos[i]);
#endif
        }

        /// <Summary>
        /// ダメージを与える相手かを判定した上でダメージを与え、エフェクトを発生させます
        /// 戻り値がfalseの場合は武器が弾かれる処理が実行されるような記述がありますが、
        /// このプロジェクト内では弾かれる処理は無いようです（3D Game Kit無印の方にあると想定します）
        /// </Summary>
        public bool CheckDamage(Collider other)
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
                _particlesPool[_currentParticle].transform.position = _pts.attackRoot.transform.position;
                _particlesPool[_currentParticle].time = 0;
                _particlesPool[_currentParticle].Play();
                _currentParticle = (_currentParticle + 1) % PARTICLE_COUNT;
            }

            return true;
        }
        public void OnDrawGizmosSelected()
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

    }
}