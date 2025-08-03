using Game.BaseUnit;
using Game.Damage;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class HealthBar : Bar
    {
        [SerializeField] private UnitHealth _unitHealth;
        [SerializeField] private TextMeshProUGUI _currentHealth;

        public void Initialize(int currentHealth)
        {
            base.Initialize();
            _unitHealth.OnDamaged += PlayerHealth_OnDamaged;
            _unitHealth.OnHealed += PlayerHealth_OnHealed;
            _unitHealth.OnDead += PlayerHealth_OnDead;
            _currentHealth.text = $"{currentHealth}";
        }

        private void PlayerHealth_OnDamaged(int damage, int currentHealth, int maxHealth, DamageType damageType, int damagerIndex, int amountOfParticles)
        {
            EmptyBarPercentage((float)currentHealth / maxHealth);
            _currentHealth.text = $"{currentHealth}";
        }
        
        private void PlayerHealth_OnHealed(int amount, int currentHealth, int maxHealth)
        {
            FillBarPercentage((float)currentHealth / maxHealth);
            _currentHealth.text = $"{currentHealth}";
        }

        private void PlayerHealth_OnDead()
        {
            _bar.SetActive(false);
        }
    }
}
