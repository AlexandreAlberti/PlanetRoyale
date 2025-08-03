using Application.Sound;
using UnityEngine;

namespace UI.Button
{
    public class NavigationBarButton : InteractableButton
    {
        [SerializeField] private Animator _animator;

        private static readonly int NormalTrigger = Animator.StringToHash("Normal");
        private static readonly int SelectedTrigger = Animator.StringToHash("Selected");
        private static readonly int PressedTrigger = Animator.StringToHash("Pressed");
        private static readonly int IdlePressedState = Animator.StringToHash("IdlePressed");
        
        public override void Enable()
        {
            if (_isEnabled)
            {
                return;
            }

            base.Enable();
            _pointerListener.OnButtonUp -= OnButtonUp;
        }
        
        public void ForceClick()
        {
            if (_animateClick)
            {
                ResetAnimationTriggers();
                _animator.Play(IdlePressedState);
            }
        }
        
        public override void OnButtonUp()
        {
            if (_animateClick)
            {
                ResetAnimationTriggers();
                _animator.SetTrigger(NormalTrigger);
            }
        }

        protected override void OnButtonDown()
        {
            if (!_isEnabled)
            {
                return;
            }

            if (_animateClick)
            {
                ResetAnimationTriggers();
                _animator.SetTrigger(SelectedTrigger);
            }
        }
        
        protected override void OnClick()
        {
            if (!_isEnabled)
            {
                return;
            }

            if (_playSounds)
            {
                SoundManager.Instance.PlayMenuPositiveButtonSound();
            }

            if (_animateClick)
            {
                ResetAnimationTriggers();
                _animator.SetTrigger(PressedTrigger);
            }

            OnClicked?.Invoke(this);
        }
        
        private void ResetAnimationTriggers()
        {
            _animator.ResetTrigger(NormalTrigger);
            _animator.ResetTrigger(SelectedTrigger);
            _animator.ResetTrigger(PressedTrigger);
        }
    }
}