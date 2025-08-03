using UnityEngine;

namespace Application.Data.Hazard
{
    public abstract class HazardData : ScriptableObject
    {
        [field: SerializeField] public HazardType Type { get; private set; }
    }
}