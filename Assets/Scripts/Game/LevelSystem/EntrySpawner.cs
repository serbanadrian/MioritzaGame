using System.Collections.Generic;
using UnityEngine;

namespace MioritzaGame.Game
{
    public sealed class EntrySpawner : MonoBehaviour
    {
        [SerializeField] private EntryConfiguration _configuration;

        public List<PlayerSpawnPoint> SpawnEntriesInRoom(Transform mapTransform, Transform parent)
        {
            var spawned = new List<PlayerSpawnPoint>();

            if (_configuration == null)
            {
                Debug.LogError($"{nameof(EntrySpawner)} missing {nameof(_configuration)}.");
                return spawned;
            }

            if (mapTransform == null)
            {
                Debug.LogError($"{nameof(EntrySpawner)}.{nameof(SpawnEntriesInRoom)} missing {nameof(mapTransform)}.");
                return spawned;
            }

            var placements = _configuration.Placements;
            if (placements == null || placements.Length == 0)
            {
                Debug.LogError($"{nameof(EntrySpawner)} has empty placements in {nameof(EntryConfiguration)}.");
                return spawned;
            }

            var yOffset = _configuration.YOffset;
            var scale = _configuration.Scale;

            var maxAvailable = placements.Length;
            var minCount = Mathf.Min(_configuration.MinCount, maxAvailable);
            var maxCount = Mathf.Min(_configuration.MaxCount, maxAvailable);
            var spawnCount = Random.Range(minCount, maxCount + 1);

            var indices = new int[maxAvailable];
            for (var i = 0; i < maxAvailable; i++) indices[i] = i;
            for (var i = maxAvailable - 1; i > 0; i--)
            {
                var j = Random.Range(0, i + 1);
                (indices[i], indices[j]) = (indices[j], indices[i]);
            }

            for (var i = 0; i < spawnCount; i++)
            {
                var placement = placements[indices[i]];
                if (placement._sprite == null) continue;
                if (placement._positions == null || placement._positions.Length == 0) continue;

                var localPosition = placement._positions[Random.Range(0, placement._positions.Length)];
                var worldPosition = mapTransform.TransformPoint(localPosition);
                worldPosition.y += yOffset;

                var entry = new GameObject(placement._sprite.name);
                entry.transform.SetParent(parent, worldPositionStays: false);
                entry.transform.position = worldPosition;
                entry.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                entry.transform.localScale = Vector3.one * scale;

                var renderer = entry.AddComponent<SpriteRenderer>();
                renderer.sprite = placement._sprite;
                renderer.sortingOrder = placement._sortingOrder;

                var spriteSize = placement._sprite.bounds.size;
                var box = entry.AddComponent<BoxCollider>();
                box.size = new Vector3(spriteSize.x, spriteSize.y, 2f);
                box.center = Vector3.zero;

                var spawnPosition = mapTransform.TransformPoint(localPosition + placement._spawnOffset);
                spawnPosition.y += yOffset;
                spawned.Add(new PlayerSpawnPoint
                {
                    _position = spawnPosition,
                    _facing = placement._facing,
                });
            }

            return spawned;
        }
    }
}
