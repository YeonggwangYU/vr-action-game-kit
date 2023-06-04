using System.Collections.Generic;
using NUnit.Framework;
using _3DGamekitLite.Scripts.Runtime.Game.DamageSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Gamekit3D
{
    public class DamageableTest
    {
        private GameObject _damageableGameObject = new GameObject("_damageableGameObject");
        private Damageable.DamageMessage data;

        private void setHitpoint(Damageable damageable)
        {
            damageable.maxHitPoints = 1;
            
            //this need to avoid null reference exception.
            damageable.OnResetDamage = new UnityEvent();
            damageable.OnDeath = new UnityEvent();
            damageable.OnReceiveDamage = new UnityEvent();
            damageable.OnHitWhileInvulnerable = new UnityEvent();
            damageable.onDamageMessageReceivers = new List<MonoBehaviour>();

            damageable.ResetDamage();
        }
        private Damageable.DamageMessage SetDamageMessage(Damageable.DamageMessage data)
        {
            //there are annother parameter. but it have nothing to do with test so set nothing.
            data.amount = 1;

            return data;
        }
        
        //don't coll setHitpoint method. so fail.
        [Test]
        public void ApplyDamage_hitpointMinusFailTest()
        {
            Damageable damageable = _damageableGameObject.AddComponent<Damageable>();
            
            data = SetDamageMessage(data);
            
            bool result = damageable.ApplyDamage(data);
            Assert.That(result == false);
        }

        // Fails by setting an invalid argument.
        [Test]
        public void ApplyDamage_isVulnerableFailTest()
        {
            Damageable damageable = _damageableGameObject.AddComponent<Damageable>();
            
            setHitpoint(damageable);
            damageable.isInvulnerable = true;
            data = SetDamageMessage(data);
            
            bool result = damageable.ApplyDamage(data);
            Assert.That(result == false);
        }

        // success pattern.
        [Test]
        public void ApplyDamage_SuccessTest()
        {
            Damageable damageable = _damageableGameObject.AddComponent<Damageable>();

            setHitpoint(damageable);
            data = SetDamageMessage(data);
            
            bool result = damageable.ApplyDamage(data);
            Assert.That(result == true);
        }

    }
}