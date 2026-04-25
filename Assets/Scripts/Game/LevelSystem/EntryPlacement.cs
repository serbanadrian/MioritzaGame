using System;
using UnityEngine;

namespace MioritzaGame.Game
{
    [Serializable]
    public struct EntryPlacement
    {
        public Sprite _sprite;
        public Vector3[] _positions;
        public Vector3 _spawnOffset;
        public EntryFacing _facing;
        public int _sortingOrder;
    }
}
