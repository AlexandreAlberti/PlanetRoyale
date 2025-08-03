using UnityEngine;

namespace Game.EnemyUnit
{
    public class Boss : MonoBehaviour
    {
        [SerializeField] private string _name;
        [SerializeField] private bool _isMiniBoss;

        public string GetName()
        {
            return _name;
        }

        public bool IsMiniBoss()
        {
            return _isMiniBoss;
        }
    }
}