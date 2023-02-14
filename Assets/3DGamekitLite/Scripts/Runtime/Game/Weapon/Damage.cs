using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
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
        /// �U���J�n�ʒu���猻�݂̈ʒu�̊ԂɌ������o���A
        /// �q�b�g����R���C�_�[�����邩�ǂ������m�F���܂�
        /// </Summary>
        public bool HitCheck(bool inAttack, AttackPoint[] attackPoints)
        {
            return !inAttack;
        }

        /// <Summary>
        /// �_���[�W��^���鑊�肩�𔻒肵����Ń_���[�W��^���A�G�t�F�N�g�𔭐������܂�
        /// �߂�l��false�̏ꍇ�͕��킪�e����鏈�������s�����悤�ȋL�q������܂����A
        /// ���̃v���W�F�N�g���ł͒e����鏈���͖����悤�ł��i3D Game Kit����̕��ɂ���Ƒz�肵�܂��j
        /// </Summary>
        private bool CheckDamage(Collider other, AttackPoint pts)
        {
            return true;
        }
        
    }
}