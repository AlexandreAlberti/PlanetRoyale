using Application.Data.Hazard;
using UnityEngine;

namespace Game.Hazard
{
    public abstract class HazardGameManager : GameManager
    {
        [SerializeField] protected HazardData _hazardData;
    }
}