using UnityEngine;

namespace Application.Data.Planet
{
    [CreateAssetMenu(fileName = "PlanetData", menuName = "ScriptableObjects/PlanetData", order = 0)]
    public class PlanetData : ScriptableObject
    {
        [field: SerializeField] public PlanetType Type { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
    }
}