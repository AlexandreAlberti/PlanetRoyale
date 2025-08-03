using Application.Data.Progression;

namespace UI.Counter
{
    public class EnergyCounter : BaseCounter
    {
        private void Start()
        {
            EnergyDataManager.Instance.OnEnergySpent += OnEnergySpent;
        }

        private void OnDestroy()
        {
            EnergyDataManager.Instance.OnEnergySpent -= OnEnergySpent;
        }
        
        private void OnEnergySpent(int energySpent)
        {
            SubtractAnimationParticle(-energySpent);
        }
    }
}