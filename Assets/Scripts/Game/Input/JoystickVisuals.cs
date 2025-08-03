using UnityEngine;

namespace Game.Input
{
    public class JoystickVisuals : MonoBehaviour
    {
        [field: SerializeField] public RectTransform RectTransform { get; private set; }
        [field: SerializeField] public RectTransform Handle { get; private set; }
    }
}