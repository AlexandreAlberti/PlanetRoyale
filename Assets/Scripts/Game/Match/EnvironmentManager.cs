using System.Collections;
using Application.Sound;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Match
{
    public class EnvironmentManager : MonoBehaviour
    {
        [SerializeField] private Light _mainLight;
        [SerializeField] private Image _vignette;
        [SerializeField] private float _transitionsDuration;
        [Header("Match Stage 1 Config")]
        [SerializeField] private Color _skyBoxColorStage1;
        [SerializeField] private Color _fogColorStage1;
        [SerializeField] private Color _lightColorStage1;
        [SerializeField] private Color _vignetteColorStage1;
        [SerializeField] private float _musicPlaySpeedStage1;
        [Header("Match Stage 2 Config")]
        [SerializeField] private Color _skyBoxColorStage2;
        [SerializeField] private Color _fogColorStage2;
        [SerializeField] private Color _lightColorStage2;
        [SerializeField] private Color _vignetteColorStage2;
        [SerializeField] private float _musicPlaySpeedStage2;
        [Header("Match Stage 3 Config")]
        [SerializeField] private Color _skyBoxColorStage3;
        [SerializeField] private Color _fogColorStage3;
        [SerializeField] private Color _lightColorStage3;
        [SerializeField] private Color _vignetteColorStage3;
        [SerializeField] private Color _vignettePulseColorStage3;
        [SerializeField] private float _pulseDuration;
        [SerializeField] private float _musicPlaySpeedStage3;

        public static EnvironmentManager Instance { get; private set; }

        private static readonly int Tint = Shader.PropertyToID("_Tint");

        private void Awake()
        {
            Instance = this;
        }
        
        public void Initialize()
        {
            RenderSettings.fogColor = _fogColorStage1;
            RenderSettings.skybox.SetColor(Tint, _skyBoxColorStage1);
            MusicManager.Instance.SetReproductionSpeed(_musicPlaySpeedStage1);
            
            if (_vignette)
            {
                _vignette.color = _vignetteColorStage1;
            }
            
            if (_mainLight)
            {
                _mainLight.color = _lightColorStage1;
            }
        }

        public void ResetEnvironment()
        {
            _vignette.DOKill();
            Initialize();
        }
        
        public void TransitionToStage2()
        {
            StartCoroutine(TransitionToNextStage(_skyBoxColorStage1, _skyBoxColorStage2, 
                                                        _fogColorStage1, _fogColorStage2,
                                                        _lightColorStage1, _lightColorStage2, 
                                                        _vignetteColorStage1, _vignetteColorStage2, 
                                                        _musicPlaySpeedStage1, _musicPlaySpeedStage2));
        }

        public void TransitionToStage3()
        {
            StartCoroutine(TransitionToNextStage(_skyBoxColorStage2, _skyBoxColorStage3, 
                                                        _fogColorStage2, _fogColorStage3,
                                                        _lightColorStage2, _lightColorStage3, 
                                                        _vignetteColorStage2, _vignetteColorStage3, 
                                                        _musicPlaySpeedStage2, _musicPlaySpeedStage3));
            StartCoroutine(WaitAndStartPulsatingVignette());
        }

        private IEnumerator WaitAndStartPulsatingVignette()
        {
            yield return new WaitForSeconds(_transitionsDuration);
            ChangeVignetteToPulseColor();
        }

        private void ChangeVignetteToPulseColor()
        {
            _vignette.DOColor(_vignettePulseColorStage3, _pulseDuration).OnComplete(ChangeVignetteToNormalColor);
        }

        private void ChangeVignetteToNormalColor()
        {
            _vignette.DOColor(_vignetteColorStage3, _pulseDuration).OnComplete(ChangeVignetteToPulseColor);
        }

        private IEnumerator TransitionToNextStage(Color skyBoxColorStagePrevious, Color skyBoxColorStageNext, 
                                                  Color fogColorStagePrevious, Color fogColorStageNext, 
                                                  Color lightColorStagePrevious, Color lightColorStageNext,
                                                  Color vignetteColorStagePrevious, Color vignetteColorStageNext,
                                                  float musicPlaySpeedPrevious, float musicPlaySpeedNext)
        {
            float transitionTime = 0.0f;

            while (transitionTime < _transitionsDuration)
            {
                float transitionPercentage = transitionTime / _transitionsDuration;
                _mainLight.color = Color.Lerp(lightColorStagePrevious, lightColorStageNext, transitionPercentage);
                _vignette.color = Color.Lerp(vignetteColorStagePrevious, vignetteColorStageNext, transitionPercentage);
                MusicManager.Instance.SetReproductionSpeed(Mathf.Lerp(musicPlaySpeedPrevious, musicPlaySpeedNext, transitionPercentage));
                RenderSettings.fogColor = Color.Lerp(fogColorStagePrevious, fogColorStageNext, transitionPercentage);
                RenderSettings.skybox.SetColor(Tint, Color.Lerp(skyBoxColorStagePrevious, skyBoxColorStageNext, transitionPercentage));
                yield return null;
                transitionTime += Time.deltaTime;
            }
            
            _mainLight.color = lightColorStageNext;
            _vignette.color = vignetteColorStageNext;
            MusicManager.Instance.SetReproductionSpeed(musicPlaySpeedNext);
            RenderSettings.fogColor = fogColorStageNext;
            RenderSettings.skybox.SetColor(Tint, skyBoxColorStageNext);
        }
    }
}
