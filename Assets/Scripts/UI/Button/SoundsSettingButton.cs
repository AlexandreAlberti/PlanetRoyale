using Application.Data;
using Application.Sound;

namespace UI.Button
{
    public class SoundsSettingButton : SettingButton
    {
        protected override void Start()
        {
            _isOn = SettingsDataManager.Instance.AreSoundsOn();
            base.Start();
        }

        protected override void OnClick()
        {
            base.OnClick();
            SettingsDataManager.Instance.SaveSoundsOn(_isOn);

            if (_isOn)
            {
                SoundManager.Instance.PlayMenuPositiveButtonSound();
            }
        }
    }
}