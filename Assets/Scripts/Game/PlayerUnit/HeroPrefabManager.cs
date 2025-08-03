using Game.Equipment;
using UnityEngine;

namespace Game.PlayerUnit
{
    public class HeroPrefabManager : MonoBehaviour
    {
        [SerializeField] private GameObject _heroBowPrefab;
        [SerializeField] private GameObject _heroGreatSwordPrefab;
        [SerializeField] private GameObject _heroFireBallStaffPrefab;
        [SerializeField] private GameObject _heroTridentPrefab;
        [SerializeField] private GameObject _heroPoisonBombPrefab;

        public GameObject GetHeroPrefabByWeaponType(WeaponType weaponType)
        {
            switch (weaponType)
            {
                default:
                case WeaponType.None:
                case WeaponType.Bow:
                    return _heroBowPrefab;
                case WeaponType.GreatSword:
                    return _heroGreatSwordPrefab;
                case WeaponType.FireBallStaff:
                    return _heroFireBallStaffPrefab;
                case WeaponType.Trident:
                    return _heroTridentPrefab;
                case WeaponType.PoisonBomb:
                    return _heroPoisonBombPrefab;
            }
        }

        public void DisableAllPrefabs()
        {
            _heroBowPrefab.SetActive(false);
            _heroGreatSwordPrefab.SetActive(false);
            _heroFireBallStaffPrefab.SetActive(false);
            _heroTridentPrefab.SetActive(false);
            _heroPoisonBombPrefab.SetActive(false);
        }
    }
}