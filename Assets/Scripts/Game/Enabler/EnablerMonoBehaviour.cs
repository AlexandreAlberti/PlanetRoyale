using UnityEngine;

namespace Game.Enabler
{
    public class EnablerMonoBehaviour : MonoBehaviour
    {
        protected bool _isEnabled;

        public virtual void Enable()
        {
            _isEnabled = true;
        }
        
        public virtual void Disable()
        {
            _isEnabled = false;
        }
    }
}