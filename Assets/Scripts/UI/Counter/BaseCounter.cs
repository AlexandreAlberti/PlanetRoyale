using TMPro;
using UnityEngine;

namespace UI.Counter
{
    public class BaseCounter : MonoBehaviour
    {
        [SerializeField] private Animator _particleAnimator;
        [SerializeField] private TextMeshProUGUI _particleText;
        
        private static readonly int SubtractAmount = Animator.StringToHash("SubtractAmount");
        
        protected void SubtractAnimationParticle(int amountSubtracted)
        {
            _particleText.text = $"{amountSubtracted}";
            _particleAnimator.SetTrigger(SubtractAmount);
        }
    }
}