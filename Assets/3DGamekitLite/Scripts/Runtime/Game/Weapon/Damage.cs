using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    public class Damage : MonoBehaviour
    {

        
        /// <Summary>
        /// 敵に攻撃がヒットしたかをチェックします
        /// </Summary>
        public bool HitCheck(bool inAttack, MeleeWeapon.AttackPoint[] attackPoints, Vector3[] previousPos,RaycastHit[] RaycastHitCache)
        {
            bool hitCheckResult = false;
            
            //攻撃中かどうかを判定しています
            if (inAttack)
            {
                for (int i = 0; i < attackPoints.Length; ++i)
                {
                    MeleeWeapon.AttackPoint pts = attackPoints[i];

                    //武器の先にあるアタックポイントの現在の座標を取得します
                    Vector3 worldPos = pts.attackRoot.position + pts.attackRoot.TransformVector(pts.offset);
                    
                    //アタックポイントの現在の座標と過去の座標から攻撃の進度及び方向を取得します
                    Vector3 attackVector = worldPos - previousPos[i];

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
                    int contacts = Physics.SphereCastNonAlloc(r, pts.radius, RaycastHitCache, attackVector.magnitude,
                        ~0,
                        QueryTriggerInteraction.Ignore);

                    //光線に衝突したものの数だけ処理を繰り返します
                    for (int k = 0; k < contacts; ++k)
                    {
                        //光線に衝突したもののコライダーを取得します
                        Collider col = RaycastHitCache[k].collider;

                        //コライダーが存在していれば、ダメージ処理を実行します
                        if (col != null)
                        {
                            CheckDamage(col, pts);
                            hitCheckResult = true;
                        }
                    }

                    previousPos[i] = worldPos;

#if UNITY_EDITOR
                    pts.PreviousPositions.Add(previousPos[i]);
#endif
                }
            }

            return hitCheckResult;
        }

        /// <Summary>
        /// ダメージを与える相手かを判定した上でダメージを与え、エフェクトを発生させます
        /// 戻り値がfalseの場合は武器が弾かれる処理が実行されるような記述がありますが、
        /// このプロジェクト内では弾かれる処理は無いようです（3D Game Kit無印の方にあると想定します）
        /// </Summary>
        private bool CheckDamage(Collider other, MeleeWeapon.AttackPoint pts)
        {
            return true;
        }
        
    }
}