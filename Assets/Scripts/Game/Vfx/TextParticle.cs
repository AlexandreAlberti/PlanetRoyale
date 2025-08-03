using TMPro;
using UnityEngine;

namespace Game.Vfx
{
    public class TextParticle : MonoBehaviour
    {
        [SerializeField] protected TextMeshPro _text;
        [SerializeField] private Animator _animator;
        
        private static readonly int TextParticleState = Animator.StringToHash("TextParticle");
        private const float VerticalOffset = 0.8f;

        public void Initialize(Vector3 position, Color color, string text)
        {
            transform.position = position + Vector3.up * VerticalOffset;
            _text.color = color;
            _text.text = text;
            _animator.Play(TextParticleState);
        }
    }
}