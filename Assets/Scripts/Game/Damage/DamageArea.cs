using Game.Enabler;
using UnityEngine;

namespace Game.Damage
{
    [RequireComponent(typeof(Collider))]
    public abstract class DamageArea : EnablerMonoBehaviour
    {
        [SerializeField] private float _damageCoolDown;
        [SerializeField] protected DamageType _damageType;
        [SerializeField] protected DamageEffect _damageEffect;

        private Collider[] _colliders;
        private bool _canDamage;
        private float _currentDamageCoolDown;
        protected int _damage;

        public override void Enable()
        {
            base.Enable();

            _colliders = GetComponents<Collider>();
            
            foreach (Collider currentCollider in _colliders)
            {
                currentCollider.enabled = true;
            }
        }

        public override void Disable()
        {
            base.Disable();
            
            foreach (Collider currentCollider in _colliders)
            {
                currentCollider.enabled = false;
            }
            
            _canDamage = false;
        }

        public virtual void Initialize(int damage)
        {
            _damage = damage;
            _currentDamageCoolDown = _damageCoolDown;
            Enable();
        }

        private void Update()
        {
            if (!_isEnabled)
            {
                return;
            }

            _currentDamageCoolDown += Time.deltaTime;

            if (!_canDamage && _currentDamageCoolDown >= _damageCoolDown)
            {
                _currentDamageCoolDown = 0.0f;
                _canDamage = true;
            }
        }
        
        private void OnTriggerStay(Collider other)
        {
            if (!_isEnabled)
            {
                return;
            }
            
            if (!_canDamage)
            {
                return;
            }
            
            _canDamage = false;
            _currentDamageCoolDown = 0.0f;
            DamageTarget(other);
        }

        protected abstract void DamageTarget(Collider other);
    }
}