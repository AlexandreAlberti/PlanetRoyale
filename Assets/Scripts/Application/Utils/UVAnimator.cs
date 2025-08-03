using UnityEngine;

namespace Application.Utils
{
    public class UVAnimator : MonoBehaviour
    {
        // Variables to control the speed of the UV animation
        public Vector2 uvSpeed = new Vector2(0.1f, 0.1f);
        private Renderer rend;

        void Start()
        {
            // Get the Renderer component attached to the object
            rend = GetComponent<Renderer>();
        }

        void Update()
        {
            // Calculate the new texture offset
            Vector2 offset = Time.time * uvSpeed;

            // Apply the offset to the material's main texture
            rend.material.mainTextureOffset = offset;
        }
    }
}
