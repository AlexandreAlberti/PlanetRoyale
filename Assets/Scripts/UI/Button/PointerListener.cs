using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Button
{
    public class PointerListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Action OnButtonDown { get; set; }
        public Action OnButtonUp { get; set; }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            OnButtonDown?.Invoke();
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            OnButtonUp?.Invoke();
        }
    }
}