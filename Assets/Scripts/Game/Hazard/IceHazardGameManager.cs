using System.Collections;
using Application.Data.Hazard;
using UnityEngine;

namespace Game.Hazard
{
    [RequireComponent(typeof(IceHazardManager))]
    public class IceHazardGameManager : HazardGameManager
    {
        protected override IEnumerator InitialZoomOut()
        {
            if (_hazardData is IceHazardData iceHazardData)
            {
                IceHazardManager.Instance.ApplyPlayerRangeHazard(iceHazardData, _sphereRadius);
            }

            return base.InitialZoomOut();
        }
    }
}