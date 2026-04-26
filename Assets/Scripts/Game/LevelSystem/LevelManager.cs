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

        private static readonly Direction[] AllDirections =
        {
            MioritzaGame.Game.Direction.North,
            MioritzaGame.Game.Direction.South,
            MioritzaGame.Game.Direction.East,
            MioritzaGame.Game.Direction.West,
        };

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
            var occupied = new HashSet<Vector2Int>(cells);

            var roomCenters = new Dictionary<Vector2Int, Vector3>();
            var roomInstances = new Dictionary<Vector2Int, GameObject>();
            var doors = new Dictionary<(Vector2Int cell, MioritzaGame.Game.Direction direction), SpawnedEntry>();
            var deadEnds = new List<(Vector2Int cell, MioritzaGame.Game.Direction direction)>();
            var startSpawn = default(SpawnedEntry);
            var hasStartSpawn = false;

            foreach (var cell in cells)
            {
                var room = PickWeightedRoom();
                if (room._roomPrefab == null) continue;

                var position = new Vector3(cell.x * _cellSize, 0f, cell.y * _cellSize);
                var roomInstance = Instantiate(room._roomPrefab, position, room._roomPrefab.transform.rotation, parent);
                roomCenters[cell] = position;
                roomInstances[cell] = roomInstance;

                if (_mushroomSpawner != null)
                {
                    var points = new List<Transform>();
                    foreach (Transform child in roomInstance.transform)
                        if (child.name.StartsWith("MushroomSpawn") == true) points.Add(child);
                    if (points.Count > 0) _mushroomSpawner.SpawnMushroomsAtPoints(points, parent);
                    else _mushroomSpawner.SpawnMushroomsInRoom(position, parent);
                }

                if (_entrySpawner == null) continue;

                foreach (var direction in AllDirections)
                {
                    var neighbor = cell + direction.ToCellOffset();
                    if (occupied.Contains(neighbor) == false)
                    {
                        deadEnds.Add((cell, direction));
                        continue;
                    }

                    var spawned = _entrySpawner.SpawnEntryForDirection(roomInstance.transform, parent, direction);
                    if (spawned._door == null) continue;

                    doors[(cell, direction)] = spawned;
                    if (cell == Vector2Int.zero && hasStartSpawn == false)
                    {
                        startSpawn = spawned;
                        hasStartSpawn = true;
                    }
                }
            }

            var exitCell = SpawnLevelExit(deadEnds, roomInstances, parent);
            SpawnSheep(cells, exitCell, roomCenters, roomInstances, doors, parent);

            foreach (var entry in doors)
            {
                var (cell, direction) = entry.Key;
                var neighborCell = cell + direction.ToCellOffset();
                var neighborKey = (neighborCell, direction.Opposite());
                if (doors.TryGetValue(neighborKey, out var neighborDoor) == false) continue;

                var cameraTarget = roomCenters.TryGetValue(neighborCell, out var center) ? center : entry.Value._spawnPosition;
                entry.Value._door.SetTarget(neighborDoor._door, neighborDoor._spawnPosition, neighborDoor._facing, cameraTarget);
            }

            if (_player != null && hasStartSpawn == true)
            {
                var spawnPosition = startSpawn._spawnPosition;
                spawnPosition.y = _player.position.y;
                var controller = _player.GetComponent<PlayerController>();
                if (controller != null) controller.Spawn(spawnPosition, startSpawn._facing);
                else _player.position = spawnPosition;
                if (startSpawn._door != null) startSpawn._door.SuppressUntil(Time.time + 1f);
            }
        }

        private Vector2Int? SpawnLevelExit(List<(Vector2Int cell, MioritzaGame.Game.Direction direction)> deadEnds, Dictionary<Vector2Int, GameObject> roomInstances, Transform parent)
        {
            var exitScene = _configuration.ExitSceneName;
            if (string.IsNullOrEmpty(exitScene) == true) return null;
            if (_entrySpawner == null) return null;
            if (deadEnds.Count == 0) return null;

            var candidates = new List<(Vector2Int cell, MioritzaGame.Game.Direction direction)>();
            foreach (var deadEnd in deadEnds)
                if (deadEnd.cell != Vector2Int.zero) candidates.Add(deadEnd);
            if (candidates.Count == 0) candidates = deadEnds;

            var pick = candidates[Random.Range(0, candidates.Count)];
            if (roomInstances.TryGetValue(pick.cell, out var roomInstance) == false) return null;

            var spawned = _entrySpawner.SpawnEntryForDirection(roomInstance.transform, parent, pick.direction);
            if (spawned._door == null) return null;

            var doorGameObject = spawned._door.gameObject;
            doorGameObject.name = $"LevelExit_{exitScene}";
            Destroy(spawned._door);

            var transition = doorGameObject.AddComponent<LevelTransitionDoor>();
            transition.SetTargetScene(exitScene);

            doorGameObject.transform.localScale *= _configuration.ExitScale;
            foreach (var renderer in doorGameObject.GetComponentsInChildren<SpriteRenderer>())
                renderer.color = _configuration.ExitTint;

            return pick.cell;
        }

        private void SpawnSheep(List<Vector2Int> cells, Vector2Int? exitCell, Dictionary<Vector2Int, Vector3> roomCenters, Dictionary<Vector2Int, GameObject> roomInstances, Dictionary<(Vector2Int cell, Direction direction), SpawnedEntry> doors, Transform parent)
        {
            var prefab = _configuration.SheepPrefab;
            if (prefab == null) return;

            var candidates = new List<Vector2Int>();
            foreach (var cell in cells)
            {
                if (cell == Vector2Int.zero) continue;
                if (exitCell.HasValue == true && cell == exitCell.Value) continue;
                candidates.Add(cell);
            }
            if (candidates.Count == 0) return;

            var pick = candidates[Random.Range(0, candidates.Count)];
            if (roomCenters.TryGetValue(pick, out var center) == false) return;

            var spawnPosition = center;
            if (roomInstances.TryGetValue(pick, out var roomInstance) == true)
            {
                var marker = roomInstance.transform.Find("SheepSpawn");
                if (marker != null) spawnPosition = marker.position;
            }
            spawnPosition.y = 0.01f;
            Instantiate(prefab, spawnPosition, prefab.transform.rotation, parent);
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
