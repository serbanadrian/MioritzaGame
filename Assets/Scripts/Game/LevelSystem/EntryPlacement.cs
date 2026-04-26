using System;
using UnityEngine;

namespace MioritzaGame.Game
{
    [Serializable]
    public struct EntryPlacement
    {
        public GameObject _prefab;
        public Vector3[] _positions;
        public Vector3 _spawnOffset;
        public EntryFacing _facing;
        public Direction _direction;
        public int _sortingOrder;
    }
}
