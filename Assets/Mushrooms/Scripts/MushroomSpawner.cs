using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using MioritzaGame.Game; // Access to PolygonDeadZone

namespace MioritzaGame
{
    public class MushroomSpawner : MonoBehaviour
    {
        [Header("Prefabs & Data")]
        [SerializeField] private Mushroom _mushroomPrefab;
        [SerializeField] private List<MushroomSO> _goodMushrooms;
        [SerializeField] private List<MushroomSO> _badMushrooms;

        [Header("Spawn Settings")]
        [SerializeField] private int _minGoodPerRoom = 1;
        [SerializeField] private int _maxGoodPerRoom = 4;
        [SerializeField] private int _minBadPerRoom = 0;
        [SerializeField] private int _maxBadPerRoom = 3;

        [Space]
        [Header("Walkable Area (Overrides Area Size)")]
        [Tooltip("If assigned, mushrooms will spawn randomly inside this polygon zone.")]
        [SerializeField] private PolygonDeadZone _spawnZone;

        [Space]
        // Offset of 48 so mushrooms spawn strictly within [-48, 48] local from room center
        [SerializeField] private float _spawnAreaHalfSize = 48f;
        [SerializeField] private float _spawnYOffset = 0.5f;
        [SerializeField] private ActiveEffects sceneEffects;

        public void SpawnMushroomsInRoom(Vector3 roomCenter, Transform parent = null, PolygonDeadZone spawnZone = null)
        {
            int goodCount = Random.Range(_minGoodPerRoom, _maxGoodPerRoom + 1);
            for (int i = 0; i < goodCount; i++)
            {
                SpawnMushroom(roomCenter, _goodMushrooms, parent, true, spawnZone);
            }

            int badCount = Random.Range(_minBadPerRoom, _maxBadPerRoom + 1);
            for (int i = 0; i < badCount; i++)
            {
                SpawnMushroom(roomCenter, _badMushrooms, parent, false, spawnZone);
            }
        }

        public void SpawnMushroomsAtPoints(List<Transform> spawnPoints, Transform parent = null)
        {
            if (_mushroomPrefab == null) return;
            if (spawnPoints == null || spawnPoints.Count == 0) return;

            var available = new List<Transform>(spawnPoints);
            var count = Random.Range(0, available.Count + 1);

            for (int i = 0; i < count; i++)
            {
                int pickIndex = Random.Range(0, available.Count);
                var point = available[pickIndex];
                available.RemoveAt(pickIndex);

                bool isGood = (_goodMushrooms != null && _goodMushrooms.Count > 0)
                    && (_badMushrooms == null || _badMushrooms.Count == 0 || Random.value < 0.6f);
                var pool = isGood ? _goodMushrooms : _badMushrooms;
                if (pool == null || pool.Count == 0) continue;

                var mushroomSO = pool[Random.Range(0, pool.Count)];
                var spawnPos = point.position;
                spawnPos.y = _spawnYOffset;

                var rotation = Quaternion.Euler(90f, 0f, 0f);
                var instance = Instantiate(_mushroomPrefab, spawnPos, rotation, parent);
                instance.Initialize(mushroomSO, sceneEffects);
                if (isGood == true) instance.gameObject.tag = "GoodMushroom";
            }
        }

        private void SpawnMushroom(Vector3 roomCenter, List<MushroomSO> options, Transform parent, bool isGood, PolygonDeadZone spawnZone)
        {
            if (_mushroomPrefab == null || options == null || options.Count == 0) return;

            var mushroomSO = options[Random.Range(0, options.Count)];
            Vector3 spawnPos = Vector3.zero;

            // Use the room's spawn zone if passed, otherwise fallback to spawner's assigned zone
            PolygonDeadZone activeZone = spawnZone != null ? spawnZone : _spawnZone;

            // If a Spawn Zone is assigned, pick a point inside the polygon
            if (activeZone != null && activeZone.LocalPoints != null && activeZone.LocalPoints.Length > 2)
            {
                spawnPos = GetRandomPointInPolygon(activeZone);
            }
            else
            {
                // Fallback to original square size logic
                var randomOffset = new Vector3(
                    Random.Range(-_spawnAreaHalfSize, _spawnAreaHalfSize),
                    _spawnYOffset,
                    Random.Range(-_spawnAreaHalfSize, _spawnAreaHalfSize)
                );
                spawnPos = roomCenter + randomOffset;
            }

            var rotation = Quaternion.Euler(90f, 0f, 0f);
            var instance = Instantiate(_mushroomPrefab, spawnPos, rotation, parent);
            instance.Initialize(mushroomSO, sceneEffects);

            if (isGood)
            {
                instance.gameObject.tag = "GoodMushroom";
            }
        }

        private Vector3 GetRandomPointInPolygon(PolygonDeadZone zone)
        {
            Vector3[] localPoints = zone.LocalPoints;
            if (localPoints.Length < 3) return zone.transform.position;

            // Find the bounding box of the polygon
            float minX = float.MaxValue, maxX = float.MinValue;
            float minZ = float.MaxValue, maxZ = float.MinValue;

            foreach (var point in localPoints)
            {
                if (point.x < minX) minX = point.x;
                if (point.x > maxX) maxX = point.x;
                if (point.z < minZ) minZ = point.z;
                if (point.z > maxZ) maxZ = point.z;
            }

            Vector3 randomLocalPoint = Vector3.zero;
            bool inside = false;
            int maxAttempts = 100; // Prevent infinite loops

            for (int i = 0; i < maxAttempts; i++)
            {
                float testX = Random.Range(minX, maxX);
                float testZ = Random.Range(minZ, maxZ);

                if (IsPointInPolygon(testX, testZ, localPoints))
                {
                    randomLocalPoint = new Vector3(testX, 0, testZ);
                    inside = true;
                    break;
                }
            }

            if (!inside)
            {
                // Fallback to average center if couldn't find a point
                var fallback = Vector3.zero;
                foreach (var p in localPoints) fallback += p;
                randomLocalPoint = (fallback / localPoints.Length);
                randomLocalPoint.y = 0;
            }

            // Convert local point to world space using the zone's transform
            Vector3 worldPos = zone.transform.TransformPoint(randomLocalPoint);
            worldPos.y = _spawnYOffset; // Force fixed height in world space
            return worldPos;
        }

        private bool IsPointInPolygon(float x, float z, Vector3[] polygon)
        {
            bool isInside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if (((polygon[i].z > z) != (polygon[j].z > z)) &&
                    (x < (polygon[j].x - polygon[i].x) * (z - polygon[i].z) / (polygon[j].z - polygon[i].z) + polygon[i].x))
                {
                    isInside = !isInside;
                }
            }
            return isInside;
        }
    }
}
