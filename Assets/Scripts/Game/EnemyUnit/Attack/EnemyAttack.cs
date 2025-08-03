using Game.Enabler;
using UnityEngine;

namespace Game.EnemyUnit.Attack
{
    public abstract class EnemyAttack : EnablerMonoBehaviour
    {
        public abstract void Initialize();
    }
}