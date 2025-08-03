using UnityEngine;

namespace Application.Data.Hazard
{
    [CreateAssetMenu(fileName = "IceHazardData", menuName = "ScriptableObjects/IceHazardData", order = 0)]
    public class IceHazardData : HazardData
    {
        [field: SerializeField] public float PlayerRangeMultiplier { get; private set; }
        [field: SerializeField] public float PlayerAreaOfViewDistance { get; private set; }
    }
}