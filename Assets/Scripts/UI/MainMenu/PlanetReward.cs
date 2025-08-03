using Application.Data.Planet;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class PlanetReward : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        
        public void Initialize(PlanetData planetData)
        {
            _icon.sprite = planetData.Sprite;
        }
    }
}
