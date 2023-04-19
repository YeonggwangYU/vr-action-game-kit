using System;
using System.Collections.Generic;
using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Weapon
{
    public class Damage : MonoBehaviour
    {
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

        public AttackPoint[] attackPoints = new AttackPoint[0];

        /// <Summary>
        /// 敵に攻撃がヒットしたかをチェックします
        /// </Summary>
        public bool HitCheck(bool inAttack, AttackPoint[] attackPoints)
        {
            return !inAttack;
        }

        /// <Summary>
        /// ダメージを与える相手かを判定した上でダメージを与え、エフェクトを発生させます
        /// 戻り値がfalseの場合は武器が弾かれる処理が実行されるような記述がありますが、
        /// このプロジェクト内では弾かれる処理は無いようです（3D Game Kit無印の方にあると想定します）
        /// </Summary>
        private bool CheckDamage(Collider other, AttackPoint pts)
        {
            return true;
        }
        
    }
}