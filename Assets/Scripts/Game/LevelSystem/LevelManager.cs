using System.Collections.Generic;
using UnityEngine;

namespace MioritzaGame.Game
{
    public sealed class LevelManager : MonoBehaviour
    {
        [SerializeField] private LevelSystemConfiguration _configuration;
        [SerializeField] private Transform _roomsParent;
        [SerializeField, Min(0.1f)] private float _cellSize = 101f;
        [SerializeField] private MushroomSpawner _mushroomSpawner;
        [SerializeField] private EntrySpawner _entrySpawner;
        [SerializeField] private Transform _player;

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
            var startingRoomEntries = new List<PlayerSpawnPoint>();
            var isFirstRoom = true;

            foreach (var cell in cells)
            {
                var room = PickWeightedRoom();
                if (room._roomPrefab == null)
                    continue;

                var position = new Vector3(cell.x * _cellSize, 0f, cell.y * _cellSize);
                var roomInstance = Instantiate(room._roomPrefab, position, room._roomPrefab.transform.rotation, parent);

                if (_mushroomSpawner != null)
                {
                    // Pass 'parent' to avoid inheriting the room's weird scale (100, 0.01, 100)
                    _mushroomSpawner.SpawnMushroomsInRoom(position, parent);
                }

                if (_entrySpawner != null)
                {
                    var spawned = _entrySpawner.SpawnEntriesInRoom(roomInstance.transform, parent);
                    if (isFirstRoom == true && spawned != null) startingRoomEntries.AddRange(spawned);
                }

                isFirstRoom = false;
            }

            if (_player != null && startingRoomEntries.Count > 0)
            {
                var spawnPoint = startingRoomEntries[Random.Range(0, startingRoomEntries.Count)];
                var spawnPosition = spawnPoint._position;
                spawnPosition.y = _player.position.y;

                var controller = _player.GetComponent<PlayerController>();
                if (controller != null) controller.Spawn(spawnPosition, spawnPoint._facing);
                else _player.position = spawnPosition;
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
