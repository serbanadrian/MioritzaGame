using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
        // Offset of 48 so mushrooms spawn strictly within [-48, 48] local from room center
        [SerializeField] private float _spawnAreaHalfSize = 48f;
        [SerializeField] private float _spawnYOffset = 0.5f;
        [SerializeField] private ActiveEffects sceneEffects;

        public void SpawnMushroomsInRoom(Vector3 roomCenter, Transform parent = null)
        {
            int goodCount = Random.Range(_minGoodPerRoom, _maxGoodPerRoom + 1);
            for (int i = 0; i < goodCount; i++)
            {
                SpawnMushroom(roomCenter, _goodMushrooms, parent);
            }

            int badCount = Random.Range(_minBadPerRoom, _maxBadPerRoom + 1);
            for (int i = 0; i < badCount; i++)
            {
                SpawnMushroom(roomCenter, _badMushrooms, parent);
            }
        }

        private void SpawnMushroom(Vector3 roomCenter, List<MushroomSO> options, Transform parent)
        {
            if (_mushroomPrefab == null || options == null || options.Count == 0) return;

            var mushroomSO = options[Random.Range(0, options.Count)];

            var randomOffset = new Vector3(
                Random.Range(-_spawnAreaHalfSize, _spawnAreaHalfSize),
                _spawnYOffset,
                Random.Range(-_spawnAreaHalfSize, _spawnAreaHalfSize)
            );

            var spawnPos = roomCenter + randomOffset;
            var rotation = Quaternion.Euler(0f, -45f, 0f);
            var instance = Instantiate(_mushroomPrefab, spawnPos, rotation, parent);
            instance.Initialize(mushroomSO, sceneEffects);
        }
    }
}
