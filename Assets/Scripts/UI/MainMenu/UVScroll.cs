using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class UVScroll : MonoBehaviour
    {
        [SerializeField] private RawImage _rawImage;
        [SerializeField] private Vector2 _scrollSpeed;

        private Vector2 _currentUVOffset = Vector2.zero;

        void Update()
        {
            _currentUVOffset += _scrollSpeed * Time.deltaTime;
            _rawImage.uvRect = new Rect(_currentUVOffset.x, _currentUVOffset.y, _rawImage.uvRect.width, _rawImage.uvRect.height);
        }
    }
}
