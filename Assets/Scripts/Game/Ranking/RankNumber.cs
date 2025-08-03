using System;
using TMPro;
using UnityEngine;

namespace Game.Ranking
{
    public class RankNumber : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private TextMeshProUGUI _rankText;
        [SerializeField] private Color _firstColor;
        [SerializeField] private Color _middleColor;
        [SerializeField] private Color _lastColor;
    
        private static readonly int Change = Animator.StringToHash("Change");

        private const int FirstRank = 0;
        private const int SecondRank = 1;
        private const int ThirdRank = 2;
        private const int FourthRank = 3;
    
        private const string FirstRankText = "1st";
        private const string SecondRankText = "2nd";
        private const string ThirdRankText = "3rd";
        private const string FourthRankText = "4th";
    
        public Action OnRankChanged { get; internal set; }
    
        private int _newRank;
        private Color _newColor;

        private string GetRankTextFromRank(int rank)
        {
            switch (rank)
            {
                case FirstRank:
                    return FirstRankText;
                case SecondRank:
                    return SecondRankText;
                case ThirdRank:
                    return ThirdRankText;
                case FourthRank:
                    return FourthRankText;
            }

            return FirstRankText;
        }

        public Color GetColorFromRankAndNumberOfPlayers(int rank, int numberOfPlayers)
        {
            switch (numberOfPlayers)
            {
                case 4:
                    if (rank == FirstRank)
                    {
                        return _firstColor;
                    }

                    if (rank == SecondRank || rank == ThirdRank)
                    {
                        return _middleColor;
                    }

                    if (rank == FourthRank)
                    {
                        return _lastColor;
                    }
                    break;
                case 3:
                    if (rank == FirstRank)
                    {
                        return _firstColor;
                    }

                    if (rank == SecondRank)
                    {
                        return _middleColor;
                    }

                    if (rank == ThirdRank)
                    {
                        return _lastColor;
                    }
                    break;
                case 2:
                    if (rank == FirstRank)
                    {
                        return _firstColor;
                    }

                    if (rank == SecondRank)
                    {
                        return _lastColor;
                    }
                    break;
                default:
                    return _firstColor;
            }
        
            return _firstColor;
        }

        public void OnRankChangeAnimationTriggered()
        {
            SetRank(_newRank);
            SetColor(_newColor);
            OnRankChanged?.Invoke();
        }

        private void ResetTriggers()
        {
            _animator.ResetTrigger(Change);
        }

        public void SetRank(int rank)
        {
            _rankText.text = GetRankTextFromRank(rank);
        }

        public void SetColor(Color color)
        {
            _rankText.color = color;
        }

        public void ChangeRank(int newRank, int numberOfPlayers)
        {
            SetRank(newRank);
            SetColor(GetColorFromRankAndNumberOfPlayers(newRank, numberOfPlayers));
        }
    
        public void ChangeRankAnimation(int newRank, int numberOfPlayers)
        {
            _newRank = newRank;
            _newColor = GetColorFromRankAndNumberOfPlayers(newRank, numberOfPlayers);
            ResetTriggers();
            _animator.SetTrigger(Change);
        }
    }
}
