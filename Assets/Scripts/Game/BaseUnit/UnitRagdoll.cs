using UnityEngine;

namespace Game.BaseUnit
{
    public abstract class UnitRagdoll : MonoBehaviour
    {
        public virtual void Initialize()
        {
        }

        public abstract void PlayDeathAnimation();

        public virtual void PlaySpawnAnimation()
        {
        }
    }
}