using UnityEngine;

namespace Application.Data.Hazard
{
    [CreateAssetMenu(fileName = "FireHazardData", menuName = "ScriptableObjects/FireHazardData", order = 0)]
    public class FireHazardData : HazardData
    {
        [field: SerializeField] public int MeteoriteSpawnAmount { get; private set; }
        [field: SerializeField] public float MeteoriteSpawnCooldown { get; private set; }
        [field: SerializeField] public int MeteoriteExplosionBaseDamage { get; private set; }
        [field: SerializeField] public int MeteoriteAreaOfEffectBaseDamage { get; private set; }
        [field: SerializeField] public int MeteoriteAreaOfEffectDuration { get; private set; }
        [field: SerializeField] public GameObject MeteoritePrefab { get; private set; }
        [field: SerializeField] public GameObject MeteoriteDestroyVfxPrefab { get; private set; }
        [field: SerializeField] public GameObject MeteoriteExplosionPrefab { get; private set; }
        [field: SerializeField] public GameObject MeteoriteAreaOfEffectPrefab { get; private set; }
    }
}