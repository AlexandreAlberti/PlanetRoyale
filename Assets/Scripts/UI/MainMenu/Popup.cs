using System;
using System.Collections;
using UI.Button;
using UnityEngine;

namespace UI.MainMenu
{
    public class Popup : MonoBehaviour
    {
        [SerializeField] protected Animator _animator;
        [SerializeField] protected InteractableButton _closeButton;
        [SerializeField] protected InteractableButton _clickableBackground;

        protected static readonly int ShowTrigger = Animator.StringToHash("Show");
        private static readonly int HideTrigger = Animator.StringToHash("Hide");

        protected const float AnimationDuration = 0.2f;

        private Coroutine _enableButtonsCoroutine;
        public Action OnShown;
        public Action OnClosed;

        protected virtual void CloseButton_OnClick(InteractableButton button)
        {
            UnsubscribeButtons();
            Hide();
        }

        public virtual void Show()
        {
            _animator.SetTrigger(ShowTrigger);

            EnableButtons();
        }

        public virtual void Hide()
        {
            _animator.SetTrigger(HideTrigger);
            
            OnClosed?.Invoke();
        }
        
        public void ForceHide()
        {
            _animator.SetTrigger(HideTrigger);
        }

        protected void EnableButtons()
        {
            if (_enableButtonsCoroutine != null)
            {
                StopCoroutine(_enableButtonsCoroutine);
            }

            _enableButtonsCoroutine = StartCoroutine(WaitAndSubscribeButtons());
        }

        protected virtual IEnumerator WaitAndSubscribeButtons()
        {
            yield return new WaitForSeconds(AnimationDuration / _animator.speed);

            _closeButton.OnClicked += CloseButton_OnClick;
            _closeButton.Enable();
            _clickableBackground.OnClicked += CloseButton_OnClick;
            _clickableBackground.Enable();
            OnShown?.Invoke();
        }

        protected virtual void UnsubscribeButtons()
        {
            if (_enableButtonsCoroutine != null)
            {
                StopCoroutine(_enableButtonsCoroutine);
            }
            
            _closeButton.OnClicked -= CloseButton_OnClick;
            _clickableBackground.OnClicked -= CloseButton_OnClick;
        }

        public void ChangeAnimatorSpeed(float animatorSpeed)
        {
            _animator.speed = animatorSpeed;
        }
    }
}