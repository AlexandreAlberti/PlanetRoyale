using DG.Tweening;
using UnityEngine;

namespace UI.ScreenFader
{
    public class ScreenFader : MonoBehaviour
    {
        [SerializeField] private DOTweenAnimation _fadeInAnimation;
        [SerializeField] private DOTweenAnimation _fadeOutAnimation;
        [SerializeField] private Transform _fadeInBackground;
        [SerializeField] private Transform _fadeOutBackground;
        
        public static ScreenFader Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                ShowOrHideChildrenFromLoadingObjects();
                DontDestroyOnLoad(this);
            }
        }

        public void FadeIn(bool showLoadingObjects = false)
        {
            ShowOrHideChildrenFromLoadingObjects(showLoadingObjects);
            _fadeInAnimation.DOPlay();
        }

        private void ShowOrHideChildrenFromLoadingObjects(bool showLoadingObjects = false)
        {
            foreach (Transform child in _fadeInBackground)
            {
                child.gameObject.SetActive(showLoadingObjects);
            }

            foreach (Transform child in _fadeOutBackground)
            {
                child.gameObject.SetActive(showLoadingObjects);
            }
        }

        public void FadeOut()
        {
            _fadeOutAnimation.DORewind();
            _fadeOutAnimation.DOPlay();
        }
    }
}