using System.Collections;
using Game.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Match
{
    public class StatusPanel : Panel
    {
        [SerializeField] private Animator _animator;
        [SerializeField] protected Image _icon;
        
        private static readonly int ShowTrigger = Animator.StringToHash("Show");
        private static readonly int HideTrigger = Animator.StringToHash("Hide");
        
        private Coroutine _hideCoroutine;

        private const float HideTime = 0.5f;

        protected void Show(float duration)
        {
            base.Show();
            _animator.SetTrigger(ShowTrigger);
            StartCoroutine(WaitAndHide(duration));
        }

        private IEnumerator WaitAndHide(float duration)
        {
            yield return new WaitForSeconds(duration);
            _animator.SetTrigger(HideTrigger);
            yield return new WaitForSeconds(HideTime);
            base.Hide();
        }

        public float GetHideDuration()
        {
            return HideTime;
        }
    }
}