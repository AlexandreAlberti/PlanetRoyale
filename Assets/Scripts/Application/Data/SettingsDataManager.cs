using UnityEngine;

namespace Application.Data
{
    public class SettingsDataManager : MonoBehaviour
    {
        private const string SoundsOn = "SoundsOn";
        private const string MusicOn = "MusicOn";
        private const string Enabled = "1";
        private const string Disabled = "0";
        private const string Empty = "";

        public static SettingsDataManager Instance { get; set; }

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
                SetInitialState();
            }
        }

        private void SetInitialState()
        {
            if (PlayerPrefs.GetString(SoundsOn, Empty) == Empty)
            {
                SaveSoundsOn(true);
            }

            if (PlayerPrefs.GetString(MusicOn, Empty) == Empty)
            {
                SaveMusicOn(true);
            }
        }

        public bool AreSoundsOn()
        {
            return PlayerPrefs.GetString(SoundsOn) == Enabled;
        }

        public bool IsMusicOn()
        {
            return PlayerPrefs.GetString(MusicOn) == Enabled;
        }

        public void SaveSoundsOn(bool isEnabled)
        {
            PlayerPrefs.SetString(SoundsOn, isEnabled ? Enabled : Disabled);
        }

        public void SaveMusicOn(bool isEnabled)
        {
            PlayerPrefs.SetString(MusicOn, isEnabled ? Enabled : Disabled);
        }
    }
}