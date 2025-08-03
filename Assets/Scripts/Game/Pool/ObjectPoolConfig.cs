using System;
using UnityEngine;

namespace Game.Pool
{
    [Serializable]
    struct PoolConfigObject
    {
        public GameObject Prefab;
        public int PrewarmCount;
    }
}