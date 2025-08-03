using UnityEngine;

namespace Game.Map
{
    public class Planet : MonoBehaviour
    {
        [SerializeField] private float _planetRadius;
        
        public float GetPlanetRadius()
        {
            return _planetRadius;
        }
    }
}
