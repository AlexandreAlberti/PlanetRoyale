using System;
using Application.Data;
using Application.Sound;

namespace UI.Button
{
    public class MusicSettingButton : SettingButton
    {
        public Action<bool> OnMusicButtonClicked { get; set; }
        
        protected override void Start()
        {
            _isOn = SettingsDataManager.Instance.IsMusicOn();
            base.Start();
        }

        protected override void OnClick()
        {
            base.OnClick();
            SettingsDataManager.Instance.SaveMusicOn(_isOn);
            
            OnMusicButtonClicked?.Invoke(_isOn);
            
            if (_isOn)
            {
                MusicManager.Instance.PlayCurrentMusic();
            }
            else
            {
                MusicManager.Instance.StopMusic();
            }
        }
    }
}
