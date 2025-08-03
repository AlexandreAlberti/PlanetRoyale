using Game.Damage;
using Game.Enabler;
using UnityEngine;

namespace Game.BaseUnit
{
    public abstract class UnitAttackTrigger : EnablerMonoBehaviour
    {
        protected int _damage;
        protected int _amountOfParticles;
        protected DamageEffect _damageEffect;

        public virtual void Initialize(DamageEffect damageEffect, int damage, int amountOfParticles = 1)
        {
            _damageEffect = damageEffect;
            _damage = damage;
            _amountOfParticles = amountOfParticles;
            Enable();
        }
        
        public void SetScale(float scale)
        {
            transform.localScale = Vector3.one * scale;
        }

        protected abstract void OnTriggerStay(Collider other);
    }
}