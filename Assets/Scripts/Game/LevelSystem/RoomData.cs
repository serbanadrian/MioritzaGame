using System;
using UnityEngine;

namespace MioritzaGame.Game
{
    [Serializable]
    public struct RoomData
    {
        public GameObject _roomPrefab;
        [Min(0.0001f)] public float _spawnWeight;
    }
}
