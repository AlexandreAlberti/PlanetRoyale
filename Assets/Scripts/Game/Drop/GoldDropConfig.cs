using System;
using UnityEngine;

namespace Game.Drop
{
    [Serializable]
    public struct GoldDropConfig
    {
        public GoldDropType GoldDropType;
        public GoldDrop GoldDropPrefab;
        public GoldDropCollected GoldDropCollectedPrefab;
        public GameObject GoldDropCollectedVfx;
        public int GoldDropPrewarmCount;
    }
}