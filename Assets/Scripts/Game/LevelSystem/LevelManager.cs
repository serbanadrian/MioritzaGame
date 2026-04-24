using UnityEngine;

namespace MioritzaGame.Game
{
    public sealed class LevelManager : MonoBehaviour
    {
        [SerializeField] private LevelSystemConfiguration _configuration;
        [SerializeField] private Transform _roomsParent;
        [SerializeField, Min(0.1f)] private float _cellSize = 101f;
        [SerializeField] private MushroomSpawner _mushroomSpawner;

        private void Start()
        {
            SpawnRooms();
        }

        private void SpawnRooms()
        {
            if (_configuration == null || _configuration.RoomPool.Count == 0)
            {
                Debug.LogError($"{nameof(LevelManager)} missing configuration {nameof(LevelSystemConfiguration)}. or no rooms has been assigned. in {nameof(LevelSystemConfiguration)}.");
                return;
            }

            var count = Random.Range(_configuration.MinRooms, _configuration.MaxRooms + 1);
            var parent = _roomsParent != null ? _roomsParent : transform;
            var cells = RoomLayoutGenerator.Generate(count);

            foreach (var cell in cells)
            {
                var room = PickWeightedRoom();
                if (room._roomPrefab == null)
                    continue;

                var position = new Vector3(cell.x * _cellSize, 0f, cell.y * _cellSize);
                var roomInstance = Instantiate(room._roomPrefab, position, Quaternion.identity, parent);

                if (_mushroomSpawner != null)
                {
                    // Pass 'parent' to avoid inheriting the room's weird scale (100, 0.01, 100)
                    _mushroomSpawner.SpawnMushroomsInRoom(position, parent);
                }
            }
        }

        private RoomData PickWeightedRoom()
        {
            var pool = _configuration.RoomPool;

            var totalWeight = 0f;
            foreach (var entry in pool)
                totalWeight += entry._spawnWeight;

            var roll = Random.Range(0f, totalWeight);
            var accumulated = 0f;
            foreach (var t in pool)
            {
                accumulated += t._spawnWeight;
                if (roll <= accumulated) return t;
            }
            return pool[^1];
        }
    }
}
