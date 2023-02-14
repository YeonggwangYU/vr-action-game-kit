using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Gamekit3D
{
    public class DamageTest
    {
        private Damage _damage;

        // 攻撃中ではない場合に、攻撃がヒットしないことををテストします
        [Test]
        public void HitCheck_FailTest()
        {
            bool inAttack = false;
            bool result = _damage.HitCheck(inAttack, _damage.attackPoints);
            Assert.That(result == false);
        }

        // 攻撃中の場合に、攻撃がヒットしたことをテストします
        // ★ダメージを与えたこともテストしないといけないのでは？
        // ★戻り値を構造体にしておくか？
        [Test]
        public void HitCheck_SuccessTest()
        {
            bool inAttack = true;
            bool result = _damage.HitCheck(inAttack, _damage.attackPoints);
            Assert.That(result == true);
        }

    }
}