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
        [SerializeField] private string _exitSceneName;
        [SerializeField] private Color _exitTint = new Color(1f, 0.78f, 0.25f, 1f);
        [SerializeField, Min(0.1f)] private float _exitScale = 1.4f;
        [SerializeField] private GameObject _sheepPrefab;

        internal IReadOnlyList<RoomData> RoomPool => _roomPool;
        internal int MinRooms => _minRooms;
        internal int MaxRooms => _maxRooms;
        internal string ExitSceneName => _exitSceneName;
        internal Color ExitTint => _exitTint;
        internal float ExitScale => _exitScale;
        internal GameObject SheepPrefab => _sheepPrefab;

        private void OnValidate()
        {
            if (_maxRooms < _minRooms) _maxRooms = _minRooms;
        }
    }
}
