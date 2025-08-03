using UnityEngine;

namespace Game.PlayerUnit
{
    [CreateAssetMenu(fileName = "PlayerNpc_data", menuName = "ScriptableObjects/PlayerNpc/NpcConfigData", order = 1)]
    public class PlayerNpcConfigData : ScriptableObject
    {
        [field: SerializeField] public Material[] ColorMeshMaterials { get; set; }
    }
}
