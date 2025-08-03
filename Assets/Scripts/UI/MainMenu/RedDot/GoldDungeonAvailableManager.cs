using Application.Data.Progression;
using Application.Utils;
using UnityEngine;

namespace UI.MainMenu.RedDot
{
    public class GoldDungeonAvailableManager : MonoBehaviour
    {
        [SerializeField] private GameObject _indicator;

        private void Start()
        {
            RefreshIndicator();
        }

        private void RefreshIndicator()
        {
            _indicator.SetActive(DungeonTicketsDataManager.Instance.GetGoldDungeonTickets() >= NumberConstants.One);
        }
    }
}