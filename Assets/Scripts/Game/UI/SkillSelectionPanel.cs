using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.Skills;
using UnityEngine;

namespace Game.UI
{
    public class SkillSelectionPanel : Panel
    {
        [SerializeField] private RectTransform _skill1Holder;
        [SerializeField] private RectTransform _skill2Holder;
        [SerializeField] private SkillSelectionButton _skill1;
        [SerializeField] private SkillSelectionButton _skill2;
        [SerializeField] private float _appearAnimationDuration;
        [SerializeField] private float _appearSkillAnimationDuration;
        [SerializeField] private float _appearSkillDelayDuration;
        [SerializeField] private float _hideAnimationDuration;

        private static readonly Vector3 GrowMaxScale = new(1.25f, 1.25f, 1.25f);
        private static readonly Vector3 GrowMaxScaleForSkills = new(1.05f, 1.05f, 1.05f);

        private const int AmountOfSkillsToChoose = 2;
        
        public Action<PlayerSkillData> OnSkillSelected { get; internal set; }

        public void Initialize(List<PlayerSkillData> skills)
        {
            if (skills.Count == 0)
            {
                Debug.Log($"No more Skills");
                return;
            }
            
            _skill1.Initialize(skills[0]);
            _skill2.Initialize(skills.Count == 1 ? skills[0] : skills[1]);
            _skill1.OnSkillSelected += SkillSelectionButton_OnSkillSelected;
            _skill2.OnSkillSelected += SkillSelectionButton_OnSkillSelected;
            Show();
        }

        private void SkillSelectionButton_OnSkillSelected(PlayerSkillData skillData)
        {
            _skill1.OnSkillSelected -= SkillSelectionButton_OnSkillSelected;
            _skill2.OnSkillSelected -= SkillSelectionButton_OnSkillSelected;

            OnSkillSelected?.Invoke(skillData);

            Hide();
        }

        public override void Show()
        {
            base.Show();
            StartCoroutine(ShowCoroutine());
        }

        private IEnumerator ShowCoroutine()
        {
            transform.DOScale(GrowMaxScale, _appearAnimationDuration).OnComplete(() => transform.DOScale(Vector3.one, 0.1f));
            yield return new WaitForSeconds(_appearSkillDelayDuration);
            _skill1Holder.transform.DOScale(GrowMaxScaleForSkills, _appearSkillAnimationDuration).OnComplete(() => _skill1Holder.transform.DOScale(Vector3.one, 0.05f));
            yield return new WaitForSeconds(_appearSkillDelayDuration);
            _skill2Holder.transform.DOScale(GrowMaxScaleForSkills, _appearSkillAnimationDuration).OnComplete(() => _skill2Holder.transform.DOScale(Vector3.one, 0.05f));
            yield return new WaitForSeconds(_appearSkillDelayDuration);
        }

        public override void Hide()
        {
            transform.DOScale(Vector3.zero, _hideAnimationDuration).OnComplete(() =>
            {
                base.Hide();
                _skill1Holder.transform.localScale = Vector3.zero;
                _skill2Holder.transform.localScale = Vector3.zero;
            });
        }

        public float TimeBetweenSkillSelection()
        {
            return _appearAnimationDuration + _hideAnimationDuration + _appearSkillAnimationDuration + _appearSkillDelayDuration;
        }

        public int GetAmountOfSkillsToChoose()
        {
            return AmountOfSkillsToChoose;
        }
    }
}