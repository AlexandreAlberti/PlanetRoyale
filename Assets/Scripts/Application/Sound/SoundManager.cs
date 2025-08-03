using Application.Data;
using UnityEngine;

namespace Application.Sound
{
    public class SoundManager : MonoBehaviour
    {
        [Range(0.0f, 2.0f)] [SerializeField] private float _soundsVolume = 1.0f;
        [Header("UI Sounds")] [SerializeField] private AudioClip[] _uiButtonPositiveSounds;
        [SerializeField] private AudioClip[] _uiButtonNegativeSounds;
        [SerializeField] private AudioClip[] _uiPopupSounds;
        [SerializeField] private AudioClip[] _uiButtonStartGameSounds;
        [SerializeField] private AudioClip[] _uiRouletteSounds;
        [SerializeField] private AudioClip[] _uiItemCollectedSounds;
        [SerializeField] private AudioClip[] _uiRewardSounds;

        [Header("Player Game Sounds")] 
        [SerializeField] private AudioClip[] _gamePlayerDeathSounds;
        [SerializeField] private AudioClip[] _gamePlayerLevelUpSounds;
        [SerializeField] private AudioClip[] _gamePlayerShootSounds;
        [SerializeField] private AudioClip[] _gamePlayerPickXpSounds;
        [SerializeField] private AudioClip[] _gamePlayerHitSounds;

        [Header("Enemy Game Sounds")]
        [SerializeField] private AudioClip[] _gameEnemyHitSounds;
        [SerializeField] private AudioClip[] _gameEnemyDeathSounds;
        
        public static SoundManager Instance { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
        }

        private AudioClip RandomAudioClip(AudioClip[] audioClips)
        {
            if (audioClips.Length == 0)
            {
                return null;
            }

            return audioClips[Random.Range(0, audioClips.Length)];
        }

        private void PlaySound(AudioClip sound, Vector3 position)
        {
            if (!SettingsDataManager.Instance.AreSoundsOn())
            {
                return;
            }

            if (!sound)
            {
                return;
            }

            AudioSource.PlayClipAtPoint(sound, Vector3.zero, _soundsVolume);
        }

        public void PlayMenuPositiveButtonSound()
        {
            PlaySound(RandomAudioClip(_uiButtonPositiveSounds), Vector3.zero);
        }

        public void PlayMenuNegativeButtonSound2()
        {
            PlaySound(RandomAudioClip(_uiButtonNegativeSounds), Vector3.zero);
        }

        public void PlayMenuPopupButtonSound()
        {
            PlaySound(RandomAudioClip(_uiPopupSounds), Vector3.zero);
        }

        public void PlayMenuPlayGameButtonSound()
        {
            PlaySound(RandomAudioClip(_uiButtonStartGameSounds), Vector3.zero);
        }

        public void PlayUpgradeEquipmentPieceSound()
        {
            PlaySound(RandomAudioClip(_uiItemCollectedSounds), Vector3.zero);
        }

        public void PlayUpgradeTalentButtonSound()
        {
            PlaySound(RandomAudioClip(_uiRouletteSounds), Vector3.zero);
        }

        public void PlayUpgradedTalentScreenSound()
        {
            PlaySound(RandomAudioClip(_uiItemCollectedSounds), Vector3.zero);
        }

        public void PlayRewardSound()
        {
            PlaySound(RandomAudioClip(_uiRewardSounds), Vector3.zero);
        }

        public void PlayPlayerLevelUpSound()
        {
            PlaySound(RandomAudioClip(_gamePlayerLevelUpSounds), Vector3.zero);
        }

        public void PlayPlayerDeathSound()
        {
            PlaySound(RandomAudioClip(_gamePlayerDeathSounds), Vector3.zero);
        }

        public void PlayPlayerHitSound()
        {
            PlaySound(RandomAudioClip(_gamePlayerHitSounds), Vector3.zero);
        }

        public void PlayPlayerShootSound()
        {
            PlaySound(RandomAudioClip(_gamePlayerShootSounds), Vector3.zero);
        }

        public void PlayPlayerPickXpSound()
        {
            PlaySound(RandomAudioClip(_gamePlayerPickXpSounds), Vector3.zero);
        }
        
        public void PlayEnemyHitSound()
        {
            PlaySound(RandomAudioClip(_gameEnemyHitSounds), Vector3.zero);
        }

        public void PlayEnemyDeathSound()
        {
            PlaySound(RandomAudioClip(_gameEnemyDeathSounds), Vector3.zero);
        }
    }
}