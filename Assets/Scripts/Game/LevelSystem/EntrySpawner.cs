using System.Collections.Generic;
using UnityEngine;

namespace MioritzaGame.Game
{
    public sealed class EntrySpawner : MonoBehaviour
    {
        [SerializeField] private EntryConfiguration _configuration;

        public EntryConfiguration Configuration => _configuration;

        public SpawnedEntry SpawnEntryForDirection(Transform mapTransform, Transform parent, Direction direction)
        {
            var empty = default(SpawnedEntry);
            if (_configuration == null)
            {
                Debug.LogError($"{nameof(EntrySpawner)} missing {nameof(_configuration)}.");
                return empty;
            }
            if (mapTransform == null)
            {
                Debug.LogError($"{nameof(EntrySpawner)}.{nameof(SpawnEntryForDirection)} missing {nameof(mapTransform)}.");
                return empty;
            }

            var placements = _configuration.Placements;
            if (placements == null || placements.Length == 0) return empty;

            var matches = new List<int>();
            for (var i = 0; i < placements.Length; i++)
                if (placements[i]._direction == direction && placements[i]._prefab != null) matches.Add(i);
            if (matches.Count == 0) return empty;

            var placement = placements[matches[Random.Range(0, matches.Count)]];
            if (placement._positions == null || placement._positions.Length == 0) return empty;

            var localPosition = placement._positions[Random.Range(0, placement._positions.Length)];
            var worldPosition = mapTransform.TransformPoint(localPosition);
            worldPosition.y += _configuration.YOffset;

            var entry = Object.Instantiate(placement._prefab, worldPosition, Quaternion.identity, parent);
            entry.transform.localScale = Vector3.one * _configuration.Scale;

            var renderer = entry.GetComponentInChildren<SpriteRenderer>();
            if (renderer != null) renderer.sortingOrder = placement._sortingOrder;

            foreach (var collider in entry.GetComponentsInChildren<Collider>())
                collider.isTrigger = true;

            var door = entry.GetComponent<EntryDoor>();
            if (door == null) door = entry.AddComponent<EntryDoor>();

            var spawnPosition = mapTransform.TransformPoint(localPosition + placement._spawnOffset);
            spawnPosition.y += _configuration.YOffset;

            return new SpawnedEntry
            {
                _door = door,
                _spawnPosition = spawnPosition,
                _facing = placement._facing,
            };
        }
    }

    public struct SpawnedEntry
    {
        public EntryDoor _door;
        public Vector3 _spawnPosition;
        public EntryFacing _facing;
    }
}
