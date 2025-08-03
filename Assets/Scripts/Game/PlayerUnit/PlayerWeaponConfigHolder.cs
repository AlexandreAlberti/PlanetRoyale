using UnityEngine;

namespace Game.PlayerUnit
{
    public class PlayerWeaponConfigHolder : MonoBehaviour
    {
        [SerializeField] private WeaponConfigData _weaponConfigData;

        public WeaponConfigData GetWeaponConfigData()
        {
            return _weaponConfigData;
        }
    }
}