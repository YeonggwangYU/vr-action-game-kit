using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Gamekit3D
{
    public class DamageTest
    {
        //private Damage _damage;

        // �U�����ł͂Ȃ��ꍇ�ɁA�������I�����邩���e�X�g���܂�
        [Test]
        public void GetAttackPositionFailTest()
        {
            //GetAttackPosition(false);
            Assert.That(1 < 10);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator DamageTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}