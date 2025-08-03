using System.Collections;
using System.Collections.Generic;
using Application.Data;
using Application.Data.Equipment;
using Application.Sound;
using DG.Tweening;
using Game.Equipment;
using Game.Ftue;
using Game.PlayerUnit;
using TMPro;
using UI.Button;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace UI.SearchingForPlayers
{
    public class SearchForPlayersScreen : MonoBehaviour
    {
        [SerializeField] private InteractableButton _backToMainMenuButton;
        [SerializeField] private TextMeshProUGUI _tipText;
        [SerializeField] private float _nextPlayersSearchTimeMin;
        [SerializeField] private float _nextPlayersSearchTimeMax;
        [SerializeField] private float _allPlayersFoundPostTime;
        [SerializeField] private float _playerFoundFadeImageBlockerOutTime;
        [SerializeField] private string[] _tips;
        [SerializeField] private CanvasGroup _playerBlockImage1;
        [SerializeField] private CanvasGroup _playerBlockImage2;
        [SerializeField] private CanvasGroup _playerBlockImage3;
        [SerializeField] private TextMeshProUGUI _playerNameText;
        [SerializeField] private TextMeshProUGUI _npc1NameText;
        [SerializeField] private TextMeshProUGUI _npc2NameText;
        [SerializeField] private TextMeshProUGUI _npc3NameText;
        [SerializeField] private Animator _playerAnimator;
        [SerializeField] private Animator _npc1Animator;
        [SerializeField] private Animator _npc2Animator;
        [SerializeField] private Animator _npc3Animator;
        [SerializeField] private HeroPrefabManager _humanHeroPrefabManager;

        private const float GameSceneLoadDuration = 1.0f;
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");

        private Coroutine _searchingForPlayersCoroutine;

        private void OnEnable()
        {
            _backToMainMenuButton.OnClicked -= BackToMainMenuButton_OnClicked;
            _backToMainMenuButton.OnClicked += BackToMainMenuButton_OnClicked;
        }

        private void Start()
        {
            QualitySettings.vSyncCount = 0;
            UnityEngine.Application.targetFrameRate = 60;

            MusicManager.Instance.PlayMenuMusic();
            ScreenFader.ScreenFader.Instance.FadeOut();

            _searchingForPlayersCoroutine = StartCoroutine(WaitAndIncrementPlayersFound());
            _tipText.text = _tips[Random.Range(0, _tips.Length)];
            _playerNameText.text = NameDataManager.Instance.GetPlayerName();
            List<string> npcNames = NameDataManager.Instance.GetGeneratedPlayerNpcNames(3, true);
            _npc1NameText.text = npcNames[2];
            _npc2NameText.text = npcNames[1];
            _npc3NameText.text = npcNames[0];
            _playerBlockImage1.alpha = 0.0f;
            _playerBlockImage2.alpha = 0.0f;
            _playerBlockImage3.alpha = 0.0f;
            
            WeaponType weaponType = EquipmentDataManager.Instance.GetEquippedWeaponType();
            GameObject playerPrefab = _humanHeroPrefabManager.GetHeroPrefabByWeaponType(weaponType);
            playerPrefab.SetActive(true);
            _backToMainMenuButton.gameObject.SetActive(FtueManager.Instance.HasFinishedFtue());
        }

        private IEnumerator WaitAndIncrementPlayersFound()
        {
            _playerAnimator.SetTrigger(AttackTrigger);
            yield return new WaitForSeconds(Random.Range(_nextPlayersSearchTimeMin, _nextPlayersSearchTimeMax));
            _playerBlockImage1.DOFade(1.0f, _playerFoundFadeImageBlockerOutTime).OnComplete(() => _npc1Animator.SetTrigger(AttackTrigger));
            yield return new WaitForSeconds(Random.Range(_nextPlayersSearchTimeMin, _nextPlayersSearchTimeMax));
            _playerBlockImage2.DOFade(1.0f, _playerFoundFadeImageBlockerOutTime).OnComplete(() => _npc2Animator.SetTrigger(AttackTrigger));
            yield return new WaitForSeconds(Random.Range(_nextPlayersSearchTimeMin, _nextPlayersSearchTimeMax));
            _playerBlockImage3.DOFade(1.0f, _playerFoundFadeImageBlockerOutTime).OnComplete(() => _npc3Animator.SetTrigger(AttackTrigger));
            _backToMainMenuButton.OnClicked -= BackToMainMenuButton_OnClicked;
            _backToMainMenuButton.Disable();
            _backToMainMenuButton.gameObject.SetActive(false);
            yield return new WaitForSeconds(_allPlayersFoundPostTime);
            ScreenFader.ScreenFader.Instance.FadeIn(true);
            MusicManager.Instance.FadeOutMusic();
            yield return new WaitForSeconds(GameSceneLoadDuration);
            SceneManager.LoadScene((int)SceneEnum.Game);
        }

        private void BackToMainMenuButton_OnClicked(InteractableButton button)
        {
            _backToMainMenuButton.OnClicked -= BackToMainMenuButton_OnClicked;
            _backToMainMenuButton.Disable();
            StopCoroutine(_searchingForPlayersCoroutine);
            ScreenFader.ScreenFader.Instance.FadeIn();
            SoundManager.Instance.PlayMenuPlayGameButtonSound();
            SceneManager.LoadScene((int)SceneEnum.MainMenu);
        }
    }
}