using System;
using UnityEngine;

namespace Game.Drop
{
    [Serializable]
    public struct HpDropConfig
    {
        public HpDrop HpDropPrefab;
        public HpDropCollected HpDropCollectedPrefab;
        public GameObject HpDropCollectedVfx;
        public int HpDropPrewarmCount;
    }
}