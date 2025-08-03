using System;
using UnityEngine;

namespace Game.Drop
{
    [Serializable]
    public struct XpDropConfig
    {
        public XpDropType XpDropType;
        public XpDrop XpDropPrefab;
        public XpDropCollected XpDropCollectedPrefab;
        public GameObject XpDropCollectedVfx;
        public int XpDropPrewarmCount;
    }
}