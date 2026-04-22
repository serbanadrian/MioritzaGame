using System.Collections.Generic;
using UnityEngine;

namespace MioritzaGame.Game
{
    [CreateAssetMenu(
        fileName = "NewLevelConfig",
        menuName = "MioritzaGame/Level Config",
        order = 0)]
    public sealed class LevelSystemConfiguration : ScriptableObject
    {
        [SerializeField] private List<RoomData> _roomPool = new List<RoomData>();
        [SerializeField, Min(1)] private int _minRooms = 3;
        [SerializeField, Min(1)] private int _maxRooms = 6;

        internal IReadOnlyList<RoomData> RoomPool => _roomPool;
        internal int MinRooms => _minRooms;
        internal int MaxRooms => _maxRooms;

        private void OnValidate()
        {
            if (_maxRooms < _minRooms) _maxRooms = _minRooms;
        }
    }
}
