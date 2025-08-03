using System;
using UnityEngine;

namespace Game.UI
{
    [Serializable]
    public class ColorPercentage
    {
        [SerializeField] private Color _color;
        [SerializeField] private float _threshold;

        public Color Color => _color;
        public float Threshold => _threshold;
    }
}