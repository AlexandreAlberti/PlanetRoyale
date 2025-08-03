using System.Collections.Generic;
using Game.BaseUnit;
using Game.PlayerUnit;
using UnityEngine;

namespace Game.Detectors
{
    public class PlayerDetector : UnitDetector
    {
        [SerializeField] private float _detectionDistance;
        
        public Player GetClosestPlayer()
        {
            if (!_closestUnit)
            {
                return null;
            }

            return _closestUnit.GetComponent<Player>();
        }

        public GameObject GetClosestPlayerGameObject()
        {
            if (!_closestUnit)
            {
                return null;
            }

            return _closestUnit.GetComponent<Player>().gameObject;
        }

        public override float GetDetectionDistance()
        {
            return _detectionDistance;
        }

        protected override List<Unit> GetUnitsList()
        {
            List<Unit> units = new List<Unit>();

            foreach (Player player in PlayerManager.Instance.GetPlayersList())
            {
                if (!player)
                {
                    continue;
                }

                units.Add(player.GetUnit());
            }

            return units;
        }
    }
}