using System.Collections;
using Application.Data;
using UnityEngine;

namespace Application.Sound
{
    public class MusicManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [Header("Splash Screen")]
        [SerializeField] private AudioClip _splashScreenMusic;
        [Header("Menu")]
        [SerializeField] private AudioClip _menuMusic;
        [Header("Game")]
        [SerializeField] private AudioClip _gameMusic;

        public static MusicManager Instance { get; private set; }

        private const float BackgroundMusicVolume = 0.05f;
        private const float FadeOutTimeInSeconds = 0.5f;

        private bool _isVolumeGoingDown;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                _isVolumeGoingDown = false;
                DontDestroyOnLoad(this);
            }
        }
        
        private void PlayMusic(AudioClip music)
        {
            if (!SettingsDataManager.Instance.IsMusicOn())
            {
                return;
            }
            
            if (!music)
            {
                return;
            }

            if (_isVolumeGoingDown)
            {
                StartCoroutine(WaitTillMusicFadeOutCompletely(music));
                return;
            }

            _audioSource.clip = music;
            _audioSource.volume = BackgroundMusicVolume;
            _audioSource.Play();
        }

        private IEnumerator WaitTillMusicFadeOutCompletely(AudioClip music)
        {
            yield return new WaitForSeconds(FadeOutTimeInSeconds);
            PlayMusic(music);
        }

        public void PlaySplashScreenMusic()
        {
            PlayMusic(_splashScreenMusic);
        }

        public void PlayMenuMusic()
        {
            PlayMusic(_menuMusic);
        }
        
        public void PlayGameMusic()
        {
            PlayMusic(_gameMusic);
        }
        
        public void StopMusic()
        {
            _audioSource.Stop();
        }

        public void FadeOutMusic()
        {
            _isVolumeGoingDown = true;
            StartCoroutine(VolumeDown());
        }

        private IEnumerator VolumeDown()
        {
            float startVolume = _audioSource.volume;
            float targetVolume = 0.0f;
            float timeElapsed = 0.0f;

            while (timeElapsed < FadeOutTimeInSeconds)
            {
                timeElapsed += Time.deltaTime;
                _audioSource.volume = Mathf.Lerp(startVolume, targetVolume, timeElapsed / FadeOutTimeInSeconds);
                yield return null;
            }

            _audioSource.volume = targetVolume;
            _isVolumeGoingDown = false;
        }

        public void PlayCurrentMusic()
        {
            PlayMusic(_audioSource.clip);
        }

        public void SetReproductionSpeed(float musicPlaySpeed)
        {
            if (!_audioSource)
            {
                return;
            }
            
            _audioSource.pitch = musicPlaySpeed;
        }
    }
}